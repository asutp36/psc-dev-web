using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
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
                                                  Revenue = s.PayEvents.Sum(e => e.EventIncrease.Amount),
                                                  Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                                  Cheque = s.Qr != null && s.Qr != "",
                                                  Note = s.Details,
                                                  Type = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Name
                                              })
                                              .Where(i => (!param.OnlyBank || i.Bank != 0)
                                                            && (!param.OnlyCash || i.Cash != 0)
                                                            && (!param.OnlyCheque || i.Cheque)
                                                            && terminals.Contains(i.TerminalCode));

            return result;
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByDay(GateWashDbContext context, GetIncreaseParameters param, List<string> washes)
        {
            var terminals = GetTerminalsByWashes(context, washes);

            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => (p.DTime >= param.StartDate) && (p.DTime <= param.EndDate)
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate)
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate)
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date, p.ProgramCode, p.ProgramName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new UsedProgramDto 
                                         {
                                            DTime = key.Date,
                                            ProgramCode = key.ProgramCode,
                                            ProgramName = key.ProgramName,
                                            DisplayOrder = key.DisplayOrder,
                                            Value = val.Sum()
                                         }).ToList();

            var tps = types.GroupBy(t => new { t.DTime.Date, t.TypeCode, t.TypeName, t.DisplayOrder },
                                    v => v.Value,
                                    (key, val) => new IncreaseTypeDto 
                                    {
                                        DTime = key.Date,
                                        TypeCode = key.TypeCode,
                                        TypeName = key.TypeName,
                                        DisplayOrder = key.DisplayOrder,
                                        Value = val.Sum()
                                    }).ToList();

            var pts = payouts.GroupBy(p => p.DTime.Date, v => v.Value, 
                                      (key, val) => new IncreaseTypeDto { DTime = key.Date, Value = val.Sum() }).ToList();

            var pprogs = progs.GroupBy(p => p.DTime.Date).ToList();
            var ttps = tps.GroupBy(t => t.DTime.Date).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime == t.Key);
                if (payout != null)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var groupedIncreases = pprogs.Join(ttps, p => p.Key, t => t.Key,
                                              (p, t) => new GroupedIncreaseDto
                                              {
                                                  DTime = p.Key,
                                                  Programs = p.Select(p => new UsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                                                  Types = t.Select(t => new IncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                                              });

            return groupedIncreases.AsQueryable();
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByDaySplitTerminals(GateWashDbContext context, GetIncreaseParameters param, List<string> washes)
        {
            var terminals = GetTerminalsByWashes(context, washes);
            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => (p.DTime >= param.StartDate) && (p.DTime <= param.EndDate) 
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();

            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate) 
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate)
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime, p.TerminalCode, p.TerminalName });
            var tps = types.GroupBy(t => new { t.DTime, t.TerminalCode, t.TerminalName });

            foreach (var t in tps)
            {
                var payout = payouts.Find(p => p.DTime == t.Key.DTime && p.TerminalCode == t.Key.TerminalCode);
                if (payout != null && payout.Value != 0)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var grouppp = progs.Join(tps, p => p.Key, t => t.Key,
                (p, t) => new GroupedIncreaseDto
                {
                    DTime = p.Key.DTime,
                    Terminal = p.Key.TerminalName,
                    TerminalCode = p.Key.TerminalCode,
                    Programs = p.Select(p => new UsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                    Types = t.Select(t => new IncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                });

            return grouppp.AsQueryable();
        }

        private static List<string> GetTerminalsByWashes(GateWashDbContext context, List<string> washes)
        {
            var terminalCodes = context.Devices.Where(d => d.IddeviceTypeNavigation.Code == "pay" && washes.Contains(d.Posts.First().IdwashNavigation.Code)).Select(d => d.Code).ToList();
            return terminalCodes;
        }

        private static IQueryable<UsedProgramDto> GetUsedProgramsByDayWithTerminals(GateWashDbContext context, List<string> terminals)
        {
            var programs = context.PaySessions.Include(d => d.IddeviceNavigation)
                                              .Include(p => p.IdprogramNavigation)
                                              .Where(d => terminals.Contains(d.IddeviceNavigation.Code))
                                              .GroupBy(k => new { k.IddeviceNavigation.Iddevice, DeviceCode = k.IddeviceNavigation.Code, DeviceName = k.IddeviceNavigation.Name, k.DtimeBegin.Date, k.IdprogramNavigation.Code, k.IdprogramNavigation.Name, k.IdprogramNavigation.DisplayOrder },
                                                       v => v.IdpaySession,
                                                       (key, val) => new UsedProgramDto
                                                       {
                                                           DTime = key.Date,
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

        private static IQueryable<IncreaseTypeDto> GetIncreaseTypesByDayWithTerminals(GateWashDbContext context, List<string> terminals)
        {
            var types = context.EventIncreases.Include(pe => pe.IdpayEventNavigation).ThenInclude(d => d.IddeviceNavigation)
                                              .Include(t => t.IdpayEventNavigation.IdeventKindNavigation)
                                              .Where(d => terminals.Contains(d.IdpayEventNavigation.IddeviceNavigation.Code))
                                              .GroupBy(k => new { k.IdpayEventNavigation.Dtime.Date, k.IdpayEventNavigation.Iddevice, DeviceCode = k.IdpayEventNavigation.IddeviceNavigation.Code, DeviceName = k.IdpayEventNavigation.IddeviceNavigation.Name, k.IdpayEventNavigation.IdeventKindNavigation.Code, k.IdpayEventNavigation.IdeventKindNavigation.Name, k.IdpayEventNavigation.IdeventKindNavigation.DisplayOrder },
                                                       v => v.Amount,
                                                       (key, val) => new IncreaseTypeDto
                                                       {
                                                           DTime = key.Date,
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

        private static IQueryable<IncreaseTypeDto> GetPayoutsByDaySplitTerminals(GateWashDbContext context, List<string> terminals)
        {
            var payouts = context.EventPayouts.Include(pe => pe.IdpayEventNavigation).ThenInclude(d => d.IddeviceNavigation)
                                              .Include(t => t.IdpayEventNavigation.IdeventKindNavigation)
                                              .Where(d => terminals.Contains(d.IdpayEventNavigation.IddeviceNavigation.Code))
                                              .GroupBy(k => new { k.IdpayEventNavigation.Dtime.Date, k.IdpayEventNavigation.Iddevice, DeviceCode = k.IdpayEventNavigation.IddeviceNavigation.Code, DeviceName = k.IdpayEventNavigation.IddeviceNavigation.Name, k.IdpayEventNavigation.IdeventKindNavigation.Code, k.IdpayEventNavigation.IdeventKindNavigation.Name },
                                                       v => v.Amount,
                                                       (key, val) => new IncreaseTypeDto
                                                       {
                                                           DTime = key.Date,
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
            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => (p.DTime >= param.StartDate) && (p.DTime <= param.EndDate)
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate)
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate)
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date.Month, p.DTime.Date.Year, p.ProgramCode, p.ProgramName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new UsedProgramDto
                                         {
                                             DTime = new DateTime(key.Year, key.Month, 1),
                                             ProgramCode = key.ProgramCode,
                                             ProgramName = key.ProgramName,
                                             DisplayOrder = key.DisplayOrder,
                                             Value = val.Sum()
                                         }).ToList();

            var tps = types.GroupBy(t => new { t.DTime.Date.Month, t.DTime.Date.Year, t.TypeCode, t.TypeName, t.DisplayOrder },
                                    v => v.Value,
                                    (key, val) => new IncreaseTypeDto
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
                                      (key, val) => new IncreaseTypeDto { DTime = new DateTime(key.Year, key.Month, 1), Value = val.Sum() }).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime == t.Key);
                if (payout != null)
                {
                    t.First(q => q.TypeCode == "cashincrease").Value -= payout.Value;
                }

            }

            var groupedIncreases = pprogs.Join(ttps, p => p.Key, t => t.Key,
                                              (p, t) => new GroupedIncreaseDto
                                              {
                                                  DTime = p.Key,
                                                  Programs = p.Select(p => new UsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                                                  Types = t.Select(t => new IncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                                              });

            return groupedIncreases.AsQueryable();
        }

        public static IQueryable<GroupedIncreaseDto> GetGroupedByMonthSplitTerminals(GateWashDbContext context, GetIncreaseParameters param, List<string> washes) 
        {
            var terminals = GetTerminalsByWashes(context, washes);
            var programs = GetUsedProgramsByDayWithTerminals(context, terminals).Where(p => (p.DTime >= param.StartDate) && (p.DTime <= param.EndDate)
                                                                             && (param.Terminal == null || p.TerminalCode == param.Terminal))
                                                                      .ToList();
            var types = GetIncreaseTypesByDayWithTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate)
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();
            var payouts = GetPayoutsByDaySplitTerminals(context, terminals).Where(t => (t.DTime >= param.StartDate) && (t.DTime <= param.EndDate)
                                                                           && (param.Terminal == null || t.TerminalCode == param.Terminal))
                                                                    .ToList();

            var progs = programs.GroupBy(p => new { p.DTime.Date.Month, p.DTime.Date.Year, p.ProgramCode, p.ProgramName, p.TerminalCode, p.TerminalName, p.DisplayOrder },
                                         v => v.Value,
                                         (key, val) => new UsedProgramDto
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
                                    (key, val) => new IncreaseTypeDto
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
                                      (key, val) => new IncreaseTypeDto { DTime = new DateTime(key.Year, key.Month, 1), Value = val.Sum() }).ToList();

            foreach (var t in ttps)
            {
                var payout = pts.Find(p => p.DTime == t.Key.DTime && p.TerminalCode == t.Key.TerminalCode);
                if (payout != null)
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
                                                  Programs = p.Select(p => new UsedProgramDto { ProgramCode = p.ProgramCode, ProgramName = p.ProgramName, Value = p.Value, DisplayOrder = p.DisplayOrder }),
                                                  Types = t.Select(t => new IncreaseTypeDto { TypeCode = t.TypeCode, TypeName = t.TypeName, Value = t.Value, DisplayOrder = t.DisplayOrder })
                                              });

            return groupedIncreases.AsQueryable();
        }
    }
}
