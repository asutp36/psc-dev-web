using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Repositories;
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
        private readonly WashesRepository _washesRepository;

        public PayoutService(ILogger<PayoutService> logger, GateWashDbContext model, WashesRepository washesRepository)
        {
            _logger = logger;
            _model = model;
            _washesRepository = washesRepository;
        }

        public async Task<PayoutFilters> GetFiltersAsync(IEnumerable<string> washes)
        {
            PayoutFilters filters = new PayoutFilters();

            List<string> terminalTypes = new List<string>() { "pay" };
            filters.Terminals = await _washesRepository.GetTerminalsForFilters(washes, terminalTypes);

            return filters;
        }

        /// <summary>
        /// Выборка всех сдач
        /// </summary>
        /// <returns></returns>
        private async Task<IQueryable<PayoutEventKind>> GetAsync(string eventKind, string terminal, DateTime dtimeStart, DateTime dtimeEnd)
        {
            IQueryable<PayoutEventKind> payouts = _model.EventPayouts.Include(o => o.IdpayEventNavigation).ThenInclude(o => o.IdeventKindNavigation)
                                                                     .Include(o => o.IdpayEventNavigation).ThenInclude(o => o.IddeviceNavigation)
                                                                     .Where(o => o.IdpayEventNavigation.IdeventKindNavigation.Code == eventKind &&
                                                                                 (terminal == null || o.IdpayEventNavigation.IddeviceNavigation.Code == terminal) &&
                                                                                 o.IdpayEventNavigation.Dtime >= dtimeStart && 
                                                                                 o.IdpayEventNavigation.Dtime <= dtimeEnd)
                                                             .Select(o => new PayoutEventKind
                                                             {
                                                                 Terminal = o.IdpayEventNavigation.IddeviceNavigation.Name,
                                                                 TerminalCode = o.IdpayEventNavigation.IddeviceNavigation.Code,
                                                                 DTime = o.IdpayEventNavigation.Dtime,
                                                                 Amount = o.Amount,
                                                                 M10 = o.M10,
                                                                 B50 = o.B50,
                                                                 B100 = o.B100,
                                                                 login = o.Login,
                                                                 EventKind = o.IdpayEventNavigation.IdeventKindNavigation.Name
                                                             });
            return payouts;
        }

        /// <summary>
        /// Все сдачи, сгруппированные по часам
        /// </summary>
        /// <returns></returns>
        private async Task<IQueryable<PayoutEventKind>> GetGroupByHourAsync(string eventKind, string terminal, DateTime dtimeStart, DateTime dtimeEnd)
        {
            IQueryable<PayoutEventKind> payouts = await GetAsync(eventKind, terminal, dtimeStart, dtimeEnd);

            payouts = payouts.GroupBy(key => new { key.DTime.Date, key.DTime.Hour, key.EventKind },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new PayoutEventKind
                                      {
                                          EventKind = key.EventKind,
                                          DTime = new DateTime(key.Date.Year, key.Date.Month, key.Date.Day, key.Hour, 0, 0),
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
        private async Task<IQueryable<PayoutEventKind>> GetGroupByDayAsync(string eventKind, string terminal, DateTime dtimeStart, DateTime dtimeEnd)
        {
            IQueryable<PayoutEventKind> payouts = await GetAsync(eventKind, terminal, dtimeStart, dtimeEnd);

            payouts = payouts.GroupBy(key => new { key.DTime.Date, key.EventKind },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new PayoutEventKind
                                      {
                                          EventKind = key.EventKind,
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
        private async Task<IQueryable<PayoutEventKind>> GetGroupedByMonthAsync(string eventKind, string terminal, DateTime dtimeStart, DateTime dtimeEnd)
        {
            IQueryable<PayoutEventKind> payouts = await GetAsync(eventKind, terminal, dtimeStart, dtimeEnd);

            payouts = payouts.GroupBy(key => new { key.DTime.Year, key.DTime.Month, key.EventKind },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new PayoutEventKind
                                      {
                                          EventKind = key.EventKind,
                                          DTime = new DateTime(key.Year, key.Month, 1),
                                          Amount = val.Sum(o => o.Amount),
                                          M10 = val.Sum(o => o.M10),
                                          B50 = val.Sum(o => o.B50),
                                          B100 = val.Sum(o => o.B100)
                                      });

            return payouts;
        }

        /// <summary>
        /// Все сдачи, сгруппированные по часам и терминалам
        /// </summary>
        /// <returns></returns>
        private async Task<IQueryable<PayoutEventKind>> GetGroupByHourSplitTerminalsAsync(string eventKind, string terminal, DateTime dtimeStart, DateTime dtimeEnd)
        {
            IQueryable<PayoutEventKind> payouts = await GetAsync(eventKind, terminal, dtimeStart, dtimeEnd);

            payouts = payouts.GroupBy(key => new { key.DTime.Date, key.DTime.Hour, key.EventKind, key.Terminal, key.TerminalCode },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new PayoutEventKind
                                      {
                                          Terminal = key.Terminal,
                                          TerminalCode = key.TerminalCode,
                                          EventKind = key.EventKind,
                                          DTime = new DateTime(key.Date.Year, key.Date.Month, key.Date.Day, key.Hour, 0, 0),
                                          Amount = val.Sum(o => o.Amount),
                                          M10 = val.Sum(o => o.M10),
                                          B50 = val.Sum(o => o.B50),
                                          B100 = val.Sum(o => o.B100)
                                      });

            return payouts;
        }

        /// <summary>
        /// Все сдачи, сгркпированные по дням и терминалам
        /// </summary>
        /// <returns></returns>
        private async Task<IQueryable<PayoutEventKind>> GetGroupByDaySplitTerminalsAsync(string eventKind, string terminal, DateTime dtimeStart, DateTime dtimeEnd)
        {
            IQueryable<PayoutEventKind> payouts = await GetAsync(eventKind, terminal, dtimeStart, dtimeEnd);

            payouts = payouts.GroupBy(key => new { key.DTime.Date, key.EventKind, key.Terminal, key.TerminalCode },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new PayoutEventKind
                                      {
                                          Terminal = key.Terminal,
                                          TerminalCode = key.TerminalCode,
                                          EventKind = key.EventKind,
                                          DTime = key.Date,
                                          Amount = val.Sum(o => o.Amount),
                                          M10 = val.Sum(o => o.M10),
                                          B50 = val.Sum(o => o.B50),
                                          B100 = val.Sum(o => o.B100)
                                      });

            return payouts;
        }

        /// <summary>
        /// Все сдачи, сгруппированные по месяцам и терминалам
        /// </summary>
        /// <returns></returns>
        private async Task<IQueryable<PayoutEventKind>> GetGroupedByMonthSplitTerminalsAsync(string eventKind, string terminal, DateTime dtimeStart, DateTime dtimeEnd)
        {
            IQueryable<PayoutEventKind> payouts = await GetAsync(eventKind, terminal, dtimeStart, dtimeEnd);

            payouts = payouts.GroupBy(key => new { key.DTime.Year, key.DTime.Month, key.EventKind, key.Terminal, key.TerminalCode },
                                      val => new { val.Amount, val.M10, val.B50, val.B100 },
                                      (key, val) => new PayoutEventKind
                                      {
                                          Terminal = key.Terminal,
                                          TerminalCode = key.TerminalCode,
                                          EventKind = key.EventKind,
                                          DTime = new DateTime(key.Year, key.Month, 1),
                                          Amount = val.Sum(o => o.Amount),
                                          M10 = val.Sum(o => o.M10),
                                          B50 = val.Sum(o => o.B50),
                                          B100 = val.Sum(o => o.B100)
                                      });

            return payouts;
        }

        /// <summary>
        /// Список записей о выдаче сдач
        /// </summary>
        /// <returns></returns>
        public async Task<List<Payout>> GetPayoutsAsync(GetPayoutsParameters parameters)
        {
            IQueryable<Payout> payouts = null;
            switch (parameters.GroupBy)
            {
                case "hour":
                    payouts = await GetGroupByHourAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                case "day":
                    payouts = await GetGroupByDayAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                case "month":
                    payouts = await GetGroupedByMonthAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                default:
                    payouts = await GetAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
            }

            //payouts = payouts.ToList().AsQueryable();

            //IQueryable<Payout> result = payouts.Where(o => o.EventKind == "payout" &&
            //                                                   o.DTime >= parameters.StartDate && o.DTime <= parameters.EndDate &&
            //                                                   (parameters.Terminal == null || o.TerminalCode == parameters.Terminal));

            return await payouts.ToListAsync();
        }

        /// <summary>
        /// Список записей о выдаче сдач по терминалам
        /// </summary>
        /// <returns></returns>
        public async Task<List<Payout>> GetPayoutsSplitTerminalsAsync(GetPayoutsParameters parameters)
        {
            IQueryable<Payout> payouts = null;
            payouts = parameters.GroupBy switch
            {
                "hour" => await GetGroupByHourSplitTerminalsAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate),
                "day" => await GetGroupByDaySplitTerminalsAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate),
                "month" => await GetGroupedByMonthSplitTerminalsAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate),
                _ => await GetAsync("payout", parameters.Terminal, parameters.StartDate, parameters.EndDate),
            };

            //IQueryable<Payout> result = payouts.Where(o => o.EventKind == "payout" &&
            //                                               o.DTime >= parameters.StartDate && o.DTime <= parameters.EndDate &&
            //                                               (parameters.Terminal == null || o.TerminalCode == parameters.Terminal));

            return await payouts.ToListAsync();
        }

        /// <summary>
        /// Список записей о пополнении сдач
        /// </summary>
        /// <returns></returns>
        public async Task<List<PayoutInsertion>> GetPayoutInsertionsAsync(GetPayoutsParameters parameters)
        {
            IQueryable<PayoutInsertion> payouts = null;
            switch (parameters.GroupBy)
            {
                case "hour":
                    payouts = await GetGroupByHourAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                case "day":
                    payouts = await GetGroupByDayAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                case "month":
                    payouts = await GetGroupedByMonthAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                default:
                    payouts = await GetAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
            }

            //IQueryable<PayoutInsertion> result = payouts.Where(o => o.EventKind == "payoutinsertion" && 
            //                                                   o.DTime >= parameters.StartDate && o.DTime <= parameters.EndDate && 
            //                                                   (parameters.Terminal == null || o.TerminalCode == parameters.Terminal));

            return payouts.ToList();
        }

        /// <summary>
        /// Список записей о пополнении сдач по терминалам
        /// </summary>
        /// <returns></returns>
        public async Task<List<PayoutInsertion>> GetPayoutInsertionsSplitTerminalAsync(GetPayoutsParameters parameters)
        {
            IQueryable<PayoutInsertion> payouts;
            switch (parameters.GroupBy)
            {
                case "hour":
                    payouts = await GetGroupByHourSplitTerminalsAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                case "day":
                    payouts = await GetGroupByDaySplitTerminalsAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                case "month":
                    payouts = await GetGroupedByMonthSplitTerminalsAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
                default:
                    payouts = await GetAsync("payoutinsertion", parameters.Terminal, parameters.StartDate, parameters.EndDate);
                    break;
            }

            return payouts.ToList();
        }
    }
}
