﻿using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Models.GateWashContext.StoredProcedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class IncreasesRepository
    {
        public static IQueryable<IncreaseModel> GetAll(GateWashDbContext context, GetIncreaseParameters param, List<string> washes) 
        {
            var terminals = GetTerminalsByWashes(context, washes);                                                   

            var result = context.PaySessions.Where(s => (s.DtimeBegin >= param.StartDate) && (s.DtimeBegin <= param.EndDate)
                                                    && (!param.OnlyNotes || (s.Details != null && s.Details != ""))
                                                    && (param.Terminal == null || s.IddeviceNavigation.Code == param.Terminal)
                                                    && (param.Program == null || s.IdprogramNavigation.Code == param.Program)
                                                    && (param.EventKind == null || s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Code == param.EventKind))
                                              .Select(s => new IncreaseModel
                                              {
                                                  DTime = s.DtimeBegin,
                                                  Terminal = s.IddeviceNavigation.Name,
                                                  TerminalCode = s.IddeviceNavigation.Code,
                                                  Program = s.IdprogramNavigation.Name,
                                                  Revenue = (int)(s.PayEvents.Sum(e => e.EventIncrease.Amount) * (1f - (s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Fee ?? 0f))),
                                                  Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                                  Cheque = s.Qr != null && s.Qr != "",
                                                  Note = s.Details,
                                                  Type = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Name,
                                                  Fee = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Fee ?? 0f
                                              })
                                              .Where(i => (!param.OnlyBank || i.Bank != 0)
                                                            && (!param.OnlyCash || i.Cash != 0)
                                                            && (!param.OnlyCheque || i.Cheque)
                                                            && terminals.Contains(i.TerminalCode));

            return result;
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByHour(GateWashDbContext context, GetIncreaseParameters param, List<string> washes)
        {
            var terminals = GetTerminalsByWashes(context, washes);

            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => ((p.DTime > param.StartDate.Date) || (p.DTime == param.StartDate.Date && p.Hour >= param.StartDate.Hour)) && ((p.DTime < param.EndDate.Date) || (p.DTime == param.EndDate.Date && p.Hour <= param.EndDate.Hour))
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date, p.Hour, p.ProgramCode, p.ProgramName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new GroupedUsedProgramDto
                                         {
                                             DTime = key.Date.AddHours(key.Hour),
                                             ProgramCode = key.ProgramCode,
                                             ProgramName = key.ProgramName,
                                             DisplayOrder = key.DisplayOrder,
                                             Value = val.Sum()
                                         }).ToList();

            var tps = types.GroupBy(t => new { t.DTime.Date, t.Hour, t.TypeCode, t.TypeName, t.DisplayOrder },
                                    v => v.Value,
                                    (key, val) => new GroupedIncreaseTypeDto
                                    {
                                        DTime = key.Date.AddHours(key.Hour),
                                        TypeCode = key.TypeCode,
                                        TypeName = key.TypeName,
                                        DisplayOrder = key.DisplayOrder,
                                        Value = val.Sum()
                                    }).ToList();

            var pts = payouts.GroupBy(p => new { p.DTime.Date, p.Hour }, v => v.Value,
                                      (key, val) => new GroupedIncreaseTypeDto { DTime = key.Date.AddHours(key.Hour), Value = val.Sum() }).ToList();

            var pprogs = progs.GroupBy(p => new { p.DTime, p.Hour }).ToList();
            var ttps = tps.GroupBy(t => new { t.DTime, t.Hour }).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime.AddHours(p.Hour) == t.Key.DTime.AddHours(t.Key.Hour));
                if (payout != null && payout.Value != 0)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var groupedIncreases = pprogs.Join(ttps, p => p.Key, t => t.Key,
                                              (p, t) => new GroupedIncreaseDto
                                              {
                                                  DTime = p.Key.DTime.AddHours(p.Key.Hour),
                                                  Programs = p.Select(p => new GroupedUsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                                                  Types = t.Select(t => new GroupedIncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                                              });

            return groupedIncreases.AsQueryable();
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByHourSplitTerminals(GateWashDbContext context, GetIncreaseParameters param, List<string> washes)
        {
            var terminals = GetTerminalsByWashes(context, washes);
            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => ((p.DTime > param.StartDate.Date) || (p.DTime == param.StartDate.Date && p.Hour >= param.StartDate.Hour)) && ((p.DTime < param.EndDate.Date) || (p.DTime == param.EndDate.Date && p.Hour <= param.EndDate.Hour))
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime, p.Hour, p.TerminalCode, p.TerminalName });
            var tps = types.GroupBy(t => new { t.DTime, t.Hour, t.TerminalCode, t.TerminalName });

            foreach (var t in tps)
            {
                var payout = payouts.Find(p => p.DTime.AddHours(p.Hour) == t.Key.DTime.AddHours(t.Key.Hour) && p.TerminalCode == t.Key.TerminalCode);
                if (payout != null && payout.Value != 0)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var grouppp = progs.Join(tps, p => p.Key, t => t.Key,
                (p, t) => new GroupedIncreaseDto
                {
                    DTime = p.Key.DTime.AddHours(p.Key.Hour),
                    Terminal = p.Key.TerminalName,
                    TerminalCode = p.Key.TerminalCode,
                    Programs = p.Select(p => new GroupedUsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                    Types = t.Select(t => new GroupedIncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                });

            return grouppp.AsQueryable();
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByDay(GateWashDbContext context, GetIncreaseParameters param, List<string> washes)
        {
            var terminals = GetTerminalsByWashes(context, washes);

            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => ((p.DTime > param.StartDate.Date) || (p.DTime == param.StartDate.Date && p.Hour >= param.StartDate.Hour)) && ((p.DTime < param.EndDate.Date) || (p.DTime == param.EndDate.Date && p.Hour <= param.EndDate.Hour))
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date, p.ProgramCode, p.ProgramName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new GroupedUsedProgramDto 
                                         {
                                            DTime = key.Date,
                                            ProgramCode = key.ProgramCode,
                                            ProgramName = key.ProgramName,
                                            DisplayOrder = key.DisplayOrder,
                                            Value = val.Sum()
                                         }).ToList();

            var tps = types.GroupBy(t => new { t.DTime.Date, t.TypeCode, t.TypeName, t.DisplayOrder },
                                    v => v.Value,
                                    (key, val) => new GroupedIncreaseTypeDto 
                                    {
                                        DTime = key.Date,
                                        TypeCode = key.TypeCode,
                                        TypeName = key.TypeName,
                                        DisplayOrder = key.DisplayOrder,
                                        Value = val.Sum()
                                    }).ToList();

            var pts = payouts.GroupBy(p => p.DTime.Date, v => v.Value, 
                                      (key, val) => new GroupedIncreaseTypeDto { DTime = key.Date, Value = val.Sum() }).ToList();

            var pprogs = progs.GroupBy(p => p.DTime.Date).ToList();
            var ttps = tps.GroupBy(t => t.DTime.Date).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime == t.Key);
                if (payout != null && payout.Value != 0)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var groupedIncreases = pprogs.Join(ttps, p => p.Key, t => t.Key,
                                              (p, t) => new GroupedIncreaseDto
                                              {
                                                  DTime = p.Key,
                                                  Programs = p.Select(p => new GroupedUsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                                                  Types = t.Select(t => new GroupedIncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                                              });

            return groupedIncreases.AsQueryable();
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByDaySplitTerminals(GateWashDbContext context, GetIncreaseParameters param, List<string> washes)
        {
            var terminals = GetTerminalsByWashes(context, washes);
            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => ((p.DTime > param.StartDate.Date) || (p.DTime == param.StartDate.Date && p.Hour >= param.StartDate.Hour)) && ((p.DTime < param.EndDate.Date) || (p.DTime == param.EndDate.Date && p.Hour <= param.EndDate.Hour))
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date, p.ProgramCode, p.ProgramName, p.TerminalCode, p.TerminalName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new GroupedUsedProgramDto
                                         {
                                             DTime = key.Date,
                                             TerminalCode = key.TerminalCode,
                                             TerminalName = key.TerminalName,
                                             ProgramCode = key.ProgramCode,
                                             ProgramName = key.ProgramName,
                                             DisplayOrder = key.DisplayOrder,
                                             Value = val.Sum()
                                         }).ToList();

            var tps = types.GroupBy(t => new { t.DTime.Date, t.TypeCode, t.TypeName, t.TerminalCode, t.TerminalName, t.DisplayOrder },
                                    v => v.Value,
                                    (key, val) => new GroupedIncreaseTypeDto
                                    {
                                        DTime = key.Date,
                                        TerminalCode = key.TerminalCode,
                                        TerminalName = key.TerminalName,
                                        TypeCode = key.TypeCode,
                                        TypeName = key.TypeName,
                                        DisplayOrder = key.DisplayOrder,
                                        Value = val.Sum()
                                    }).ToList();

            var pprogs = progs.GroupBy(p => new { p.DTime.Date, p.TerminalCode, p.TerminalName });
            var ttps = tps.GroupBy(t => new { t.DTime.Date, t.TerminalCode, t.TerminalName });
            var pts = payouts.GroupBy(k => new { k.DTime.Date, k.TerminalCode }, v => v.Value,
                                      (key, val) => new GroupedIncreaseTypeDto { DTime = key.Date, TerminalCode = key.TerminalCode, Value = val.Sum() }).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime.Date == t.Key.Date && p.TerminalCode == t.Key.TerminalCode);
                if (payout != null && payout.Value != 0)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var grouppp = pprogs.Join(ttps, p => p.Key, t => t.Key,
                (p, t) => new GroupedIncreaseDto
                {
                    DTime = p.Key.Date,
                    Terminal = p.Key.TerminalName,
                    TerminalCode = p.Key.TerminalCode,
                    Programs = p.Select(p => new GroupedUsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                    Types = t.Select(t => new GroupedIncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                });

            return grouppp.AsQueryable();
        }

        private static List<string> GetTerminalsByWashes(GateWashDbContext context, List<string> washes)
        {
            var terminalCodes = context.Terminals.Where(t => washes.Contains(t.IdwashNavigation.Code)).Include(t => t.IddeviceNavigation).Select(t => t.IddeviceNavigation.Code).ToList();
            return terminalCodes;
        }

        private static IQueryable<GroupedUsedProgramDto> GetUsedProgramsByDayWithTerminals(GateWashDbContext context, List<string> terminals)
        {
            var programs = context.PaySessions.Include(d => d.IddeviceNavigation)
                                              .Include(p => p.IdprogramNavigation)
                                              .Where(d => terminals.Contains(d.IddeviceNavigation.Code))
                                              .GroupBy(k => new 
                                                        { 
                                                            k.IddeviceNavigation.Iddevice, 
                                                            DeviceCode = k.IddeviceNavigation.Code, 
                                                            DeviceName = k.IddeviceNavigation.Name, 
                                                            k.DtimeBegin.Date, 
                                                            k.DtimeBegin.Hour, 
                                                            k.IdprogramNavigation.Code, 
                                                            k.IdprogramNavigation.Name, 
                                                            k.IdprogramNavigation.DisplayOrder 
                                                        },
                                                       v => v.IdpaySession,
                                                       (key, val) => new GroupedUsedProgramDto
                                                       {
                                                           DTime = key.Date,
                                                           Hour = key.Hour,
                                                            IdTerminal = key.Iddevice,
                                                           TerminalCode = key.DeviceCode,
                                                           TerminalName = key.DeviceName,
                                                           ProgramCode = key.Code,
                                                           ProgramName = key.Name,
                                                           DisplayOrder = key.DisplayOrder,
                                                           Value = val.Count()
                                                       });

            return programs;
        }

        private static IQueryable<GroupedIncreaseTypeDto> GetIncreaseTypesByDayWithTerminals(GateWashDbContext context, List<string> terminals)
        {
            var types = context.EventIncreases.Include(pe => pe.IdpayEventNavigation).ThenInclude(d => d.IddeviceNavigation)
                                              .Include(t => t.IdpayEventNavigation.IdeventKindNavigation)
                                              .Where(d => terminals.Contains(d.IdpayEventNavigation.IddeviceNavigation.Code))
                                              .GroupBy(k => new { k.IdpayEventNavigation.Dtime.Date, k.IdpayEventNavigation.Dtime.Hour, k.IdpayEventNavigation.Iddevice, DeviceCode = k.IdpayEventNavigation.IddeviceNavigation.Code, DeviceName = k.IdpayEventNavigation.IddeviceNavigation.Name, k.IdpayEventNavigation.IdeventKindNavigation.Code, k.IdpayEventNavigation.IdeventKindNavigation.Name, k.IdpayEventNavigation.IdeventKindNavigation.DisplayOrder },
                                                       v => v.Amount,
                                                       (key, val) => new GroupedIncreaseTypeDto
                                                       {
                                                           DTime = key.Date,
                                                           Hour = key.Hour,
                                                           IdTerminal = key.Iddevice,
                                                           TerminalCode = key.DeviceCode,
                                                           TerminalName = key.DeviceName,
                                                           TypeCode = key.Code,
                                                           TypeName = key.Name,
                                                           DisplayOrder = key.DisplayOrder,
                                                           Value = val.Sum()
                                                       });

            return types;
        }

        private static IQueryable<GroupedIncreaseTypeDto> GetPayoutsByDaySplitTerminals(GateWashDbContext context, List<string> terminals)
        {
            var payouts = context.EventPayouts.Include(pe => pe.IdpayEventNavigation).ThenInclude(d => d.IddeviceNavigation)
                                              .Include(t => t.IdpayEventNavigation.IdeventKindNavigation)
                                              .Where(d => terminals.Contains(d.IdpayEventNavigation.IddeviceNavigation.Code))
                                              .GroupBy(k => new { k.IdpayEventNavigation.Dtime.Date, k.IdpayEventNavigation.Dtime.Hour, k.IdpayEventNavigation.Iddevice, DeviceCode = k.IdpayEventNavigation.IddeviceNavigation.Code, DeviceName = k.IdpayEventNavigation.IddeviceNavigation.Name, k.IdpayEventNavigation.IdeventKindNavigation.Code, k.IdpayEventNavigation.IdeventKindNavigation.Name },
                                                       v => v.Amount,
                                                       (key, val) => new GroupedIncreaseTypeDto
                                                       {
                                                           DTime = key.Date,
                                                           Hour = key.Hour,
                                                           IdTerminal = key.Iddevice,
                                                           TerminalCode = key.DeviceCode,
                                                           TerminalName = key.DeviceName,
                                                           TypeCode = key.Code,
                                                           TypeName = key.Name,
                                                           Value = val.Sum()
                                                       });
            return payouts;
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByMonth(GateWashDbContext context, GetIncreaseParameters param, List<string> washes) 
        {
            var terminals = GetTerminalsByWashes(context, washes);
            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => ((p.DTime > param.StartDate.Date) || (p.DTime == param.StartDate.Date && p.Hour >= param.StartDate.Hour)) && ((p.DTime < param.EndDate.Date) || (p.DTime == param.EndDate.Date && p.Hour <= param.EndDate.Hour))
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date.Month, p.DTime.Date.Year, p.ProgramCode, p.ProgramName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new GroupedUsedProgramDto
                                         {
                                             DTime = new DateTime(key.Year, key.Month, 1),
                                             ProgramCode = key.ProgramCode,
                                             ProgramName = key.ProgramName,
                                             DisplayOrder = key.DisplayOrder,
                                             Value = val.Sum()
                                         }).ToList();

            var tps = types.GroupBy(t => new { t.DTime.Date.Month, t.DTime.Date.Year, t.TypeCode, t.TypeName, t.DisplayOrder },
                                    v => v.Value,
                                    (key, val) => new GroupedIncreaseTypeDto
                                    {
                                        DTime = new DateTime(key.Year, key.Month, 1),
                                        TypeCode = key.TypeCode,
                                        TypeName = key.TypeName,
                                        DisplayOrder = key.DisplayOrder,
                                        Value = val.Sum()
                                    }).ToList();

            var pprogs = progs.GroupBy(p => p.DTime).ToList();
            var ttps = tps.GroupBy(t => t.DTime).ToList();
            var pts = payouts.GroupBy(p => new { p.DTime.Month, p.DTime.Year }, v => v.Value,
                                      (key, val) => new GroupedIncreaseTypeDto { DTime = new DateTime(key.Year, key.Month, 1), Value = val.Sum() }).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime == t.Key);
                if (payout != null && payout.Value != 0)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var groupedIncreases = pprogs.Join(ttps, p => p.Key, t => t.Key,
                                              (p, t) => new GroupedIncreaseDto
                                              {
                                                  DTime = p.Key,
                                                  Programs = p.Select(p => new GroupedUsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                                                  Types = t.Select(t => new GroupedIncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                                              });

            return groupedIncreases.AsQueryable();
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByMonthSplitTerminals(GateWashDbContext context, GetIncreaseParameters param, List<string> washes) 
        {
            var terminals = GetTerminalsByWashes(context, washes);
            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => ((p.DTime > param.StartDate.Date) || (p.DTime == param.StartDate.Date && p.Hour >= param.StartDate.Hour)) && ((p.DTime < param.EndDate.Date) || (p.DTime == param.EndDate.Date && p.Hour <= param.EndDate.Hour))
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => ((t.DTime > param.StartDate.Date) || (t.DTime == param.StartDate.Date && t.Hour >= param.StartDate.Hour)) && ((t.DTime < param.EndDate.Date) || (t.DTime == param.EndDate.Date && t.Hour <= param.EndDate.Hour))
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date.Month, p.DTime.Date.Year, p.ProgramCode, p.ProgramName, p.TerminalCode, p.TerminalName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new GroupedUsedProgramDto
                                         {
                                             DTime = new DateTime(key.Year, key.Month, 1),
                                             TerminalCode = key.TerminalCode,
                                             TerminalName = key.TerminalName,
                                             ProgramCode = key.ProgramCode,
                                             ProgramName = key.ProgramName,
                                             DisplayOrder = key.DisplayOrder,
                                             Value = val.Sum()
                                         }).ToList();

            var tps = types.GroupBy(t => new { t.DTime.Date.Month, t.DTime.Date.Year, t.TypeCode, t.TypeName, t.TerminalCode, t.TerminalName, t.DisplayOrder },
                                    v => v.Value,
                                    (key, val) => new GroupedIncreaseTypeDto
                                    {
                                        DTime = new DateTime(key.Year, key.Month, 1),
                                        TerminalCode = key.TerminalCode,
                                        TerminalName = key.TerminalName,
                                        TypeCode = key.TypeCode,
                                        TypeName = key.TypeName,
                                        DisplayOrder = key.DisplayOrder,
                                        Value = val.Sum()
                                    }).ToList();

            var pprogs = progs.GroupBy(p => new { p.DTime, p.TerminalName, p.TerminalCode}).ToList();
            var ttps = tps.GroupBy(t => new { t.DTime, t.TerminalName, t.TerminalCode }).ToList();
            var pts = payouts.GroupBy(k => new { k.DTime.Month, k.DTime.Year, k.TerminalCode }, v => v.Value,
                                      (key, val) => new GroupedIncreaseTypeDto { DTime = new DateTime(key.Year, key.Month, 1), TerminalCode = key.TerminalCode, Value = val.Sum() }).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime == t.Key.DTime && p.TerminalCode == t.Key.TerminalCode);
                if (payout != null && payout.Value != 0)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var groupedIncreases = pprogs.Join(ttps, p => p.Key, t => t.Key,
                                              (p, t) => new GroupedIncreaseDto
                                              {
                                                  DTime = p.Key.DTime,
                                                  Terminal = p.Key.TerminalName,
                                                  TerminalCode = p.Key.TerminalCode,
                                                  Programs = p.Select(p => new GroupedUsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                                                  Types = t.Select(t => new GroupedIncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                                              });

            return groupedIncreases.AsQueryable();
        }

        public static IQueryable<IncreaseCommulativeTotalModel> GetCommulativeTotalSplitTerminals(GateWashDbContext context, GetIncreaseParameters param, List<string> washes)
        {
            string terminalCodes = "";
            if (string.IsNullOrEmpty(param.Terminal))
            {
                var terminals = GetTerminalsByWashes(context, washes);
                terminalCodes = string.Join(", ", terminals);
            }
            else
            {
                terminalCodes = param.Terminal;
            }

            SqlParameter p_DTimeBegin = new SqlParameter("@p_DTimeBegin", param.StartDate);
            SqlParameter p_DTimeEnd = new SqlParameter("@p_DTimeEnd", param.EndDate);
            SqlParameter p_TerminalCode = new SqlParameter("@p_TerminalCode", terminalCodes);
            SqlParameter p_IncreaseKind = new SqlParameter("@p_IncreaseKind", param.EventKind ?? "");

            var res = context.Set<spGetCommulativeTotalSplitTerminals_Result>()
                .FromSqlRaw("GetCommulativeIncreasesSplitTerminals @p_DTimeBegin, @p_DTimeEnd, @p_TerminalCode, @p_IncreaseKind",
                p_DTimeBegin, p_DTimeEnd, p_TerminalCode, p_IncreaseKind).AsEnumerable()
                .Select(i => new IncreaseCommulativeTotalModel
                {
                    DTime = i.DTimeBegin,
                    Terminal = i.TerminalName,
                    TerminalCode = i.Code,
                    Amount = i.Total,
                });
                //.Where(i => terminals.Contains(i.TerminalCode));

            return res.AsQueryable();
        }
    }
}
