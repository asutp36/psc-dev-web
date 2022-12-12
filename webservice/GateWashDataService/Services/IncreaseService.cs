using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Services
{
    public class IncreaseService
    {
        private readonly GateWashDbContext _context;
        private readonly WashesRepository _washesRepository;
        private readonly ProgramService _programService;
        private readonly EventKindService _ekService;

        public IncreaseService(GateWashDbContext context, WashesRepository washesRepository, ProgramService programService, EventKindService ekService)
        {
            _context = context;
            _washesRepository = washesRepository;
            _programService = programService;
            _ekService = ekService;
        }

        public async Task<IncreaseFilters> GetFilters(IEnumerable<string> washes)
        {
            IncreaseFilters filters = new IncreaseFilters();

            List<string> terminalTypes = new List<string>() { "pay" };
            filters.Terminals = await _washesRepository.GetTerminalsForFilters(washes, terminalTypes);

            filters.Programs = await _programService.GetProgramsAsFiltersAsync(washes);

            filters.Types = await _ekService.GetAsFilters(washes);

            return filters;
        }
    }
}
