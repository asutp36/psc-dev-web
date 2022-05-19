using GateWashDataService.Models;
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
                                          Bank = s.PayEvents.Where(e => bankIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Cash = s.PayEvents.Where(e => cashIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                          Cheque = s.Qr != null && s.Qr != "",
                                          Note = s.Details,
                                          Type = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Name
                                      })
                                      .Where(i => (!param.OnlyBank || i.Bank != 0) && (!param.OnlyCash || i.Cash != 0) && (!param.OnlyCheque || i.Cheque))
                                      .OrderByDescending(s => s.DTime);
            return que;
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
