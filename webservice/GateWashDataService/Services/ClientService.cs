using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Services
{
    public class ClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly WashesRepository _washRepository;
        private readonly ProgramService _programService;
        public ClientService(ILogger<ClientService> logger, WashesRepository washRepository, ProgramService programService)
        {
            _logger = logger;
            _washRepository = washRepository;
            _programService = programService;
        }

        public async Task<ClientFilters> GetFiltersAsync(IEnumerable<string> washes)
        {
            ClientFilters filters = new ClientFilters();

            List<string> terminalTypes = new List<string>() { "typedEntry" };
            filters.EnterTerminals = await _washRepository.GetTerminalsForFilters(washes, terminalTypes);

            terminalTypes = new List<string>() { "typedExit", "stop" };
            filters.ExitTerminals = await _washRepository.GetTerminalsForFilters(washes, terminalTypes);

            filters.Programs = await _programService.GetProgramsAsFiltersAsync(washes);

            return filters;
        }
        
        public async Task<List<ClientEntrance>> GetEntrancesAsync()
        {
            List<ClientEntrance> entrances = new List<ClientEntrance>();

            Random rnd = new Random();

            string[] cards = new string[] { "BC56EBDD", "FC95EDDD", "CCAEEFDD" };

            for (int i = 0; i < 50; i++)
            {
                entrances.Add(new ClientEntrance
                {
                    Terminal = "Въездной тест",
                    TerminalCode = "test-entrance",
                    DTime = DateTime.Now.AddHours(-rnd.Next(0, 24)),
                    Card = cards[rnd.Next(0, 3)],
                    Cost = 0,
                    Program = "Постоплата"
                });
            }

            return entrances;
        }

        public async Task<List<ClientExit>> GetExitsAsync()
        {
            List<ClientExit> exits = new List<ClientExit>();

            Random rnd = new Random();

            string[] cards = new string[] { "BC56EBDD", "FC95EDDD", "CCAEEFDD" };

            for (int i = 0; i < 50; i++)
            {
                exits.Add(new ClientExit
                {
                    Terminal = "Выездной тест",
                    TerminalCode = "test-exit",
                    DTime = DateTime.Now.AddHours(-rnd.Next(0, 24)),
                    Card = cards[rnd.Next(0, 3)],
                    Cost = 0,
                    Program = "Постоплата",
                    PayTerminal = "Платёжный тест",
                    PayTerminalCode = "test-pay",
                    PayType = "наличными"
                });
            }

            return exits;
        }
    }
}
