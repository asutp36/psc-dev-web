﻿using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Helpers
{
    public class SqlHelper
    {
        private static readonly List<string> bankIncreaseEventkinds = new List<string>() { "cardincrease", "bankincrease", "qrincrease" };
        private static readonly List<string> cashIncreaseEventkinds = new List<string>() { "cashincrease" };

        public static IQueryable<IncreaseModel> GetIncreasesQueryable(GateWashDbContext context, GetIncreaseParameters param)
        {
            IQueryable<IncreaseModel> que = context.PaySessions.Where(s => (s.DtimeBegin >= param.StartDate) && (s.DtimeBegin <= param.EndDate) && (!param.OnlyNotes || (s.Details != null && s.Details != ""))
                                            && (param.Terminal == null || s.IddeviceNavigation.Code == param.Terminal)
                                            && (param.Program == null || s.IdprogramNavigation.Code == param.Program)
                                            && (param.EventKind == null || s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Code == param.EventKind))
                                      .Select(s => new IncreaseModel
                                      {
                                          DTime = s.DtimeBegin,
                                          Terminal = s.IddeviceNavigation.Name,
                                          TerminalCode = s.IddeviceNavigation.Code,
                                          Program = s.IdprogramNavigation.Name,
                                          //Bank = s.PayEvents.Where(e => bankIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          //Cash = s.PayEvents.Where(e => cashIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Revenue = s.PayEvents.Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                          Cheque = s.Qr != null && s.Qr != "",
                                          Note = s.Details,
                                          Type = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Name
                                      })
                                      .Where(i => (!param.OnlyBank || i.Bank != 0) && (!param.OnlyCash || i.Cash != 0) && (!param.OnlyCheque || i.Cheque))
                                      .OrderByDescending(s => s.DTime);
            return que;
        }

        public static IQueryable<IncreasesByDaysWithProgramsAndTypes> GetIncreasesProgramsByDay(GateWashDbContext context, GetIncreaseParameters param)
        {
            var usedPrograms = context.PaySessions.Include(d => d.IddeviceNavigation)
                                                  .Select(p => new 
                                                  {
                                                      DTime = p.DtimeBegin,
                                                      Terminal = p.IddeviceNavigation.Code,
                                                      TerminalName = p.IddeviceNavigation.Name,
                                                      Code = p.IdprogramNavigation.Code,
                                                      Name = p.IdprogramNavigation.Name,
                                                      Counter = p.IdpaySession
                                                  })
                                                  .GroupBy(k => new { k.DTime.Date, k.Terminal, k.TerminalName, k.Code, k.Name},
                                                           v => v.Counter,
                                                           (keys, values) => new
                                                           {
                                                               TerminalName = keys.TerminalName,
                                                               TerminalCode = keys.Terminal,
                                                               DTime = keys.Date,
                                                               Program = new IncrProgram { Code = keys.Code, Name = keys.Name, Value = values.Count() }
                                                           })
                                                  .ToList();

            var payTypesAmounts = context.PayEvents.Include(s => s.IdpaySessionNavigation).ThenInclude(d => d.IddeviceNavigation)
                                                   .Include(t => t.IdeventKindNavigation)
                                                   .Select(o => new 
                                                   {
                                                       DTime = o.IdpaySessionNavigation.DtimeBegin,
                                                       TerminalCode = o.IddeviceNavigation.Code,
                                                       EventKindCode = o.IdeventKindNavigation.Code,
                                                       EventKindName = o.IdeventKindNavigation.Name,
                                                       Increase = o.EventIncrease,
                                                       Payout = o.EventPayout
                                                   })
                                                   .GroupBy(k => new { k.DTime.Date, k.TerminalCode, k.EventKindCode, k.EventKindName },
                                                            v => new { v.Increase, v.Payout },
                                                            (key, value) => new
                                                            {
                                                                DTime = key.Date,
                                                                TerminalCode = key.TerminalCode,
                                                                IncrType = new IncrType { Code = key.EventKindCode, Name = key.EventKindName, Value = (value.Sum(v => v.Increase.Amount) ?? 0) - value.Sum(v => v.Payout.Amount) }
                                                            })
                                                   .ToList();

            var terminalTypeProgram = usedPrograms.Join(payTypesAmounts, p => new { p.TerminalCode, p.DTime }, t => new { t.TerminalCode, t.DTime },
                                           (p, t) => new
                                           {
                                               TerminalName = p.TerminalName,
                                               TerminalCode = p.TerminalCode,
                                               DTime = p.DTime,
                                               IncrType = t.IncrType,
                                               Program = p.Program
                                           }).ToList();

            var result = terminalTypeProgram.GroupBy(k => new { k.TerminalCode, k.TerminalName, k.DTime },
                                                     v => new { v.IncrType, v.Program },
                                                     (key, value) => new IncreasesByDaysWithProgramsAndTypes
                                                     {
                                                         Terminal = key.TerminalName,
                                                         TerminalCode = key.TerminalCode,
                                                         DTime = key.DTime,
                                                         Programs = value.Select(p => p.Program).Distinct().ToList(),
                                                         Types = value.Select(t => t.IncrType).Distinct().ToList()
                                                     }).Where(i => (i.DTime >= param.StartDate) && (i.DTime <= param.EndDate));
            return result.AsQueryable();
        }

        public static IQueryable<IncreasesByDaysWithProgramsAndTypes> GetIncreasesProgramsByMonth(GateWashDbContext context, GetIncreaseParameters param)
        {
            var usedPrograms = context.PaySessions.Include(d => d.IddeviceNavigation)
                                                  .Select(p => new
                                                  {
                                                      DTime = p.DtimeBegin,
                                                      Terminal = p.IddeviceNavigation.Code,
                                                      TerminalName = p.IddeviceNavigation.Name,
                                                      Code = p.IdprogramNavigation.Code,
                                                      Name = p.IdprogramNavigation.Name,
                                                      Counter = p.IdpaySession
                                                  })
                                                  .GroupBy(k => new { k.DTime.Date.Month, k.DTime.Date.Year, k.Terminal, k.TerminalName, k.Code, k.Name },
                                                           v => v.Counter,
                                                           (keys, values) => new
                                                           {
                                                               TerminalName = keys.TerminalName,
                                                               TerminalCode = keys.Terminal,
                                                               DTime = new DateTime(keys.Year, keys.Month, 1),
                                                               Program = new IncrProgram { Code = keys.Code, Name = keys.Name, Value = values.Count() }
                                                           })
                                                  .ToList();

            var payTypesAmounts = context.PayEvents.Include(s => s.IdpaySessionNavigation).ThenInclude(d => d.IddeviceNavigation)
                                                   .Include(t => t.IdeventKindNavigation)
                                                   .Select(o => new
                                                   {
                                                       DTime = o.IdpaySessionNavigation.DtimeBegin,
                                                       TerminalCode = o.IddeviceNavigation.Code,
                                                       EventKindCode = o.IdeventKindNavigation.Code,
                                                       EventKindName = o.IdeventKindNavigation.Name,
                                                       Increase = o.EventIncrease,
                                                       Payout = o.EventPayout
                                                   })
                                                   .GroupBy(k => new { k.DTime.Date.Month, k.DTime.Date.Year, k.TerminalCode, k.EventKindCode, k.EventKindName },
                                                            v => new { v.Increase, v.Payout },
                                                            (key, value) => new
                                                            {
                                                                DTime = new DateTime(key.Year, key.Month, 1),
                                                                TerminalCode = key.TerminalCode,
                                                                IncrType = new IncrType { Code = key.EventKindCode, Name = key.EventKindName, Value = (value.Sum(v => v.Increase.Amount) ?? 0) - value.Sum(v => v.Payout.Amount) }
                                                            })
                                                   .ToList();

            var terminalTypeProgram = usedPrograms.Join(payTypesAmounts, p => new { p.TerminalCode, p.DTime }, t => new { t.TerminalCode, t.DTime },
                                           (p, t) => new
                                           {
                                               TerminalName = p.TerminalName,
                                               TerminalCode = p.TerminalCode,
                                               DTime = p.DTime,
                                               IncrType = t.IncrType,
                                               Program = p.Program
                                           }).ToList();

            var result = terminalTypeProgram.GroupBy(k => new { k.TerminalCode, k.TerminalName, k.DTime },
                                                     v => new { v.IncrType, v.Program },
                                                     (key, value) => new IncreasesByDaysWithProgramsAndTypes
                                                     {
                                                         Terminal = key.TerminalName,
                                                         TerminalCode = key.TerminalCode,
                                                         DTime = key.DTime,
                                                         Programs = value.Select(p => p.Program).Distinct().ToList(),
                                                         Types = value.Select(t => t.IncrType).Distinct().ToList()
                                                     }).Where(i => (i.DTime >= param.StartDate) && (i.DTime <= param.EndDate));
            return result.AsQueryable();
        }

        public static List<IncreaseModel> GetIncreases(GateWashDbContext context, GetIncreaseParameters param)
        {
            IOrderedQueryable<IncreaseModel> que = context.PaySessions.Where(s => (s.DtimeBegin >= param.StartDate) && (s.DtimeBegin <= param.EndDate) && (!param.OnlyNotes || (s.Details != null && s.Details != ""))
                                            && (param.Terminal == null || s.IddeviceNavigation.Code == param.Terminal)
                                            && (param.Program == null || s.IdprogramNavigation.Code == param.Program)
                                            && (param.EventKind == null || s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Code == param.EventKind))
                                      .Select(s => new IncreaseModel
                                      {
                                          DTime = s.DtimeBegin,
                                          Terminal = s.IddeviceNavigation.Code,
                                          Program = s.IdprogramNavigation.Code,
                                          Bank = s.PayEvents.Where(e => bankIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Cash = s.PayEvents.Where(e => cashIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                          Cheque = s.Qr != null && s.Qr != "",
                                          Note = s.Details,
                                          Type = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Name
                                      })
                                      .Where(i => (!param.OnlyBank || i.Bank != 0) && (!param.OnlyCash || i.Cash != 0) && (!param.OnlyCheque || i.Cheque))
                                      .OrderByDescending(s => s.DTime);

            return context.PaySessions.Where(s => (s.DtimeBegin >= param.StartDate) && (s.DtimeBegin <= param.EndDate) && (!param.OnlyNotes || (s.Details != null && s.Details != ""))
                                            && (param.Terminal == null || s.IddeviceNavigation.Code == param.Terminal)
                                            && (param.Program == null || s.IdprogramNavigation.Code == param.Program)
                                            && (param.EventKind == null || s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Code == param.EventKind))
                                      .Select(s => new IncreaseModel
                                      {
                                          DTime = s.DtimeBegin,
                                          Terminal = s.IddeviceNavigation.Code,
                                          Program = s.IdprogramNavigation.Code,
                                          Bank = s.PayEvents.Where(e => bankIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Cash = s.PayEvents.Where(e => cashIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                          Cheque = s.Qr != null && s.Qr != "",
                                          Note = s.Details,
                                          Type = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Name
                                      })
                                      .Where(i => (!param.OnlyBank || i.Bank != 0) && (!param.OnlyCash || i.Cash != 0) && (!param.OnlyCheque || i.Cheque))
                                      .OrderByDescending(s => s.DTime)
                                      .ToList();
        }

        public static async Task<List<RegionModel>> GetRegions(GateWashDbContext context)
        {
            return context.Regions.Select(r => new RegionModel
            {
                IdRegion = r.Idregion,
                Code = r.Code,
                Name = r.Name
            }).ToList();
        }

        public static async Task<List<WashModel>> GetWashes(GateWashDbContext context)
        {
            return context.Washes.Select(w => new WashModel
            {
                IdWash = w.Idwash,
                Code = w.Code,
                Name = w.Name,
                IdRegion = w.Idregion
            }).ToList();
        }

        public static async Task<List<PayTerminalModel>> GetPayTerminals(GateWashDbContext context)
        {
            return context.Posts.Where(p => p.IddeviceNavigation.IddeviceTypeNavigation.Code == "pay").Select(t => new PayTerminalModel()
            {
                IdDevice = t.Iddevice,
                Code = t.IddeviceNavigation.Code,
                Name = t.IddeviceNavigation.Name,
                IdWash = t.Idwash
            }).ToList();
        }

        public static async Task<List<ProgramModel>> GetPrograms(GateWashDbContext context)
        {
            return context.Programs.Select(i => new ProgramModel
            {
                Code = i.Code,
                Name = i.Name,
                Cost = i.Cost
            }
            ).ToList();
        }

        public static List<CollectModel> GetCollects(GateWashDbContext context, GetCollectsParameters param)
        {
            return context.Collects.Where(c => (c.Dtime >= param.StartDate && c.Dtime <= param.EndDate)
                                               && (param.Terminal == null || c.IddeviceNavigation.Code == param.Terminal))
                                   .Select(c => new CollectModel
                                   {
                                       DTime = c.Dtime,
                                       Terminal = c.IddeviceNavigation.Code,
                                       m10 = c.M10,
                                       b50 = c.B50,
                                       b100 = c.B100,
                                       b200 = c.B200,
                                       b500 = c.B500,
                                       b1000 = c.B1000,
                                       b2000 = c.B2000
                                   }).ToList();
        }

        public static void WriteUser(AccountRequestModel model, GateWashDbContext context)
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

            string email = "'null'";
            if (model.email != null)
                email = $"'{model.email}'";

            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.BeginTransaction();
                    string command = "";

                    command = $"insert into Users (Login, Password, Name, Email, Phone, PhoneInt, IDRole) " +
                        $"values ('{model.login}', '{model.password}', '{model.name}', {email}, {formattedPhone.phone}, " +
                        $"{formattedPhone.phoneInt}, (select IDRole from Roles where Code = '{model.role}'))";
                    context.Database.ExecuteSqlRaw(command);

                    foreach (string w in model.washes)
                    {
                        command = $"insert into UserWash (IDUser, IDWash) values " +
                            $"((select IDUser from Users where Login = '{model.login}'), " +
                            $"(select IDWash from Wash where Code = '{w}'))";
                        context.Database.ExecuteSqlRaw(command);
                    }

                    context.Database.CommitTransaction();
                    context.Database.CloseConnection();
                }
                catch (Exception e)
                {
                    context.Database.RollbackTransaction();

                    if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        context.Database.CloseConnection();

                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static void DeleteUser(string login, GateWashDbContext context)
        {
            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
                try
                {
                    string command = $"delete from Users where Login = '{login}'";
                    context.Database.ExecuteSqlRaw(command);
                    context.Database.CloseConnection();
                }
                catch (Exception e)
                {
                    if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        context.Database.CloseConnection();

                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }

        public static void UpdateUser(UpdateAccountRequestModel model, GateWashDbContext context)
        {
            PhoneFormatter formattedPhone = new PhoneFormatter(model.phone);

            string email = "'null'";
            if (model.email != null)
                email = $"'{model.email}'";

            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.BeginTransaction();

                    string command = $"update Users set Login = '{model.login}', Name = '{model.name}', " +
                        $"Email = {email}, Phone = {formattedPhone.phone}, PhoneInt = {formattedPhone.phoneInt}, " +
                        $"IDRole = (select IDRole from Roles where Code = '{model.role}') where Login = '{model.oldLogin}';";
                    context.Database.ExecuteSqlRaw(command);

                    command = $"delete from UserWash where IDUser = (select IDUser from Users where Login = '{model.login}');";
                    context.Database.ExecuteSqlRaw(command);

                    foreach (string w in model.washes)
                    {
                        command = $"insert into UserWash (IDUser, IDWash) values " +
                            $"((select IDUser from Users where Login = '{model.login}'), " +
                            $"(select IDWash from Wash where Code = '{w}'))";
                        context.Database.ExecuteSqlRaw(command);
                    }

                    context.Database.CommitTransaction();
                    context.Database.CloseConnection();
                }
                catch (Exception e)
                {
                    context.Database.RollbackTransaction();

                    if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        context.Database.CloseConnection();

                    throw new Exception("command", e);
                }
            }
            else
                throw new Exception("connection");
        }
    }
}
