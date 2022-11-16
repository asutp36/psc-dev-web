using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Services
{
    public class PayoutService
    {
        private readonly ILogger<PayoutService> _logger;
        private readonly GateWashDbContext _model;

        public PayoutService(ILogger<PayoutService> logger, GateWashDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        /// <summary>
        /// Выборка всех сдач
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<PayoutEventKind>> GetAsync()
        {
            IQueryable<PayoutEventKind> payouts = _model.EventPayouts.Include(o => o.IdpayEventNavigation).ThenInclude(o => o.IdeventKindNavigation)
                                                                     .Include(o => o.IdpayEventNavigation).ThenInclude(o => o.IddeviceNavigation)
                                                             .Select(o => new PayoutEventKind
                                                             {
                                                                 Terminal = o.IdpayEventNavigation.IddeviceNavigation.Code,
                                                                 DTime = o.IdpayEventNavigation.Dtime,
                                                                 Amount = o.Amount,
                                                                 M10 = o.M10,
                                                                 B50 = o.B50,
                                                                 B100 = o.B100,
                                                                 EventKind = o.IdpayEventNavigation.IdeventKindNavigation.Code
                                                             });
            return payouts;
        }

        /// <summary>
        /// Все сдачи, сгруппированные по часам
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<Payout>> GetGroupByHourAsync()
        {
            IQueryable<Payout> payouts = await GetAsync();

            payouts = payouts.GroupBy(key => new { key.DTime.Date, key.DTime.Hour },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new Payout
                                      {
                                          DTime = key.Date.AddHours(key.Hour),
                                          Amount = val.Sum(o => o.Amount),
                                          M10 = val.Sum(o => o.M10),
                                          B50 = val.Sum(o => o.B50),
                                          B100 = val.Sum(o => o.B100)
                                      });

            return payouts;
        }

        /// <summary>
        /// Все сдачи, сгркпированные по дням
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<Payout>> GetGroupByDayAsync()
        {
            IQueryable<Payout> payouts = await GetAsync();

            payouts = payouts.GroupBy(key => key.DTime.Date,
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new Payout
                                      {
                                          DTime = key.Date,
                                          Amount = val.Sum(o => o.Amount),
                                          M10 = val.Sum(o => o.M10),
                                          B50 = val.Sum(o => o.B50),
                                          B100 = val.Sum(o => o.B100)
                                      });

            return payouts;
        }

        /// <summary>
        /// Все сдачи, сгруппированные по месяцам
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<Payout>> GetGroupedByMonthAsync()
        {
            IQueryable<Payout> payouts = await GetAsync();

            payouts = payouts.GroupBy(key => new { key.DTime.Year, key.DTime.Month },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new Payout
                                      {
                                          DTime = new DateTime(key.Year, key.Month, 1),
                                          Amount = val.Sum(o => o.Amount),
                                          M10 = val.Sum(o => o.M10),
                                          B50 = val.Sum(o => o.B50),
                                          B100 = val.Sum(o => o.B100)
                                      });

            return payouts;
        }
    }
}
