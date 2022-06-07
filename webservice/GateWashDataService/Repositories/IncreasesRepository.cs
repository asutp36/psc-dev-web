using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class IncreasesRepository
    {
        public static IQueryable<IncreaseModel> GetAll(GateWashDbContext context, GetIncreaseParameters param) 
        {
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
                                                  Revenue = s.PayEvents.Sum(e => e.EventIncrease.Amount) ?? 0,
                                                  Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                                  Cheque = s.Qr != null && s.Qr != "",
                                                  Note = s.Details,
                                                  Type = s.PayEvents.OrderBy(e => e.Dtime).FirstOrDefault().IdeventKindNavigation.Name
                                              })
                                              .Where(i => (!param.OnlyBank || i.Bank != 0) 
                                                            && (!param.OnlyCash || i.Cash != 0) 
                                                            && (!param.OnlyCheque || i.Cheque))
                                              .OrderByDescending(s => s.DTime);
            return result;
        }

        public void GetGroupedByDay() { }

        public void GetGroupedByDaySplitTerminals() { }

        public void GetGroupedByMonth() { }

        public void GetGroupedByMonthSplitTerminals() { }
    }
}
