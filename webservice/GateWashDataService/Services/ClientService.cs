using GateWashDataService.Models;
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
        public ClientService(ILogger<ClientService> logger)
        {
            _logger = logger;
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
