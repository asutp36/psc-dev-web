using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
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

        public static List<IncreaseModel> GetIncomes(GateWashDbContext context, DateTime startDate, DateTime endDate, bool isBank, bool isCash, bool isNotes, string terminal, string program)
        {
            return context.PaySessions.Where(s => s.Dtime >= startDate && s.Dtime <= endDate && (!isNotes || s.ClientSessionErrors.Count != 0)
                                            && (terminal == null || s.IddeviceNavigation.Code == terminal)
                                            && (program == null || s.IdfunctionNavigation.Code == program))
                                      .Select(s => new IncreaseModel
                                      {
                                          DTime = s.Dtime,
                                          Terminal = s.IddeviceNavigation.Code,
                                          Program = s.IdfunctionNavigation.Code,
                                          Bank = s.PayEvents.Where(e => bankIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Cash = s.PayEvents.Where(e => cashIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                          Note = s.ClientSessionErrors.Count == 0 ? null : String.Join(", ", s.ClientSessionErrors.Select(err => err.Message))
                                      })
                                      .Where(i => (!isBank || i.Bank != 0) && (!isCash || i.Cash != 0))
                                      .ToList();
        }
    }
}
