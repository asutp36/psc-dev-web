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
    public class CollectService
    {
        private readonly ILogger<CollectService> _logger;
        private readonly GateWashDbContext _context;
        private readonly WashesRepository _washesRepository;

        public CollectService(ILogger<CollectService> logger, GateWashDbContext context, WashesRepository washesRepository)
        {
            _logger = logger;
            _context = context;
            _washesRepository = washesRepository;
        }

        public async Task<IQueryable<CollectModel>> GetAsync(GetCollectsParameters parameters, IEnumerable<string> terminals)
        {
            IQueryable<CollectModel> collects = _context.Collects.Where(c => (c.Dtime >= parameters.StartDate && c.Dtime <= parameters.EndDate)
                                                   && terminals.Contains(c.IddeviceNavigation.Code)
                                                   && (parameters.Terminal == null || c.IddeviceNavigation.Code == parameters.Terminal))
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
                                   });
            return collects;
        }

        public async Task<CollectFilters> GetFiltersAsync(IEnumerable<string> washes)
        {
            List<string> terminalTypes = new List<string>() { "pay" };
            CollectFilters filters = new CollectFilters();
            filters.Terminals = await _washesRepository.GetTerminalsForFilters(washes, terminalTypes);

            return filters;
        }
    }
}
