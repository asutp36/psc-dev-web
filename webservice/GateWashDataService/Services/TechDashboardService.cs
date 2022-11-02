using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Services
{
    public class TechDashboardService
    {
        private readonly GateWashDbContext _context;

        public TechDashboardService(GateWashDbContext context)
        {
            _context = context;
        }

        public async Task InsertPayoutCash()
        {

        }

        public async Task<List<WashWithTerminalsActions>> GetWashesWithTerminalsActions(IEnumerable<string> washCodes)
        {
            List<WashWithTerminalsActions> washes = await _context.Washes.Where(o => washCodes.Contains(o.Code))
                .Include(o => o.Terminals).ThenInclude(o => o.IddeviceNavigation).ThenInclude(o => o.IddeviceTypeNavigation).ThenInclude(o => o.DeviceTypeAction)
                .Select(o => new WashWithTerminalsActions
                {
                    IdWash = o.Idwash,
                    Code = o.Code,
                    Name = o.Name,
                    Terminals = o.Terminals.Select(e => new TerminalWithActions
                    {
                        IdTerminal = e.Idterminal,
                        Code = e.IddeviceNavigation.Code,
                        Name = e.IddeviceNavigation.Name,
                        Type = new TerminalType() { Code = e.IddeviceNavigation.IddeviceTypeNavigation.Code, Name = e.IddeviceNavigation.IddeviceTypeNavigation.Name },
                        InsertAction = e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertPayoutCash ? "cash" : e.IddeviceNavigation.IddeviceTypeNavigation.DeviceTypeAction.InsertWashCards ? "cards" : null
                    })
                }).ToListAsync();

            return washes;
        }
    }
}
