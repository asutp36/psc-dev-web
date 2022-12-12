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
    public class CardService
    {
        private readonly ILogger<CardService> _logger;
        private readonly GateWashDbContext _context;

        public CardService(ILogger<CardService> logger, GateWashDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<CardIssuance>> GetCardIssuanceAsync(GetCardsRefillParameters parameters, IEnumerable<string> terminals)
        {
            List<CardIssuance> issuances = await _context.CardCounters.Include(o => o.IddeviceNavigation).Include(o => o.IdeventKindNavigation)
                .Where(o => o.Dtime >= parameters.StartDate && o.Dtime <= parameters.EndDate
                        && terminals.Contains(o.IddeviceNavigation.Code)
                        && (parameters.Terminal == null || o.IddeviceNavigation.Code == parameters.Terminal)
                        && o.IdeventKindNavigation.Code == "cardissuance")
                .Select(o => new CardIssuance
                {
                    Terminal = o.IddeviceNavigation.Name,
                    DTime = o.Dtime,
                    Dispenser1 = o.Dispenser1,
                    Dispenser2 = o.Dispenser2,
                    Count1 = o.Count1,
                    Count2 = o.Count2,
                }).ToListAsync();

            return issuances;
        }

        public async Task<List<CardsRefill>> GetCardCountIncreaseAsync(GetCardsRefillParameters parameters, IEnumerable<string> terminals)
        {
            List<CardsRefill> refills = await _context.CardCounters.Include(o => o.IddeviceNavigation).Include(o => o.IdeventKindNavigation)
                .Where(o => o.Dtime >= parameters.StartDate && o.Dtime <= parameters.EndDate 
                        && terminals.Contains(o.IddeviceNavigation.Code)
                        && (parameters.Terminal == null || o.IddeviceNavigation.Code == parameters.Terminal) 
                        && o.IdeventKindNavigation.Code == "cardcountincrease")
                .Select(o => new CardsRefill
                {
                    Terminal = o.IddeviceNavigation.Name,
                    DTime = o.Dtime,
                    Dispenser1 = o.Dispenser1,
                    Dispenser2 = o.Dispenser2,
                    Amount = o.Dispenser1 + o.Dispenser2,
                    Count1 = o.Count1,
                    Count2 = o.Count2,
                    Login = o.Login
                }).ToListAsync();

            return refills;
        }
    }
}
