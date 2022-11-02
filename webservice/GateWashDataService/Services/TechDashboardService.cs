using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Repositories;
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
        private readonly WashesRepository _washesRepository;

        public TechDashboardService(GateWashDbContext context, WashesRepository washesRepository)
        {
            _context = context;
            _washesRepository = washesRepository;
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

            washes = await _context.Washes.Where(o => washCodes.Contains(o.Code))
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

        public async Task<List<RegionWithWashTerminalActions>> GetRegionsWithWashesTerminalAction(IEnumerable<string> washCodes)
        {
            var regionCodes = await _washesRepository.GetRegionCodessByWashCodes(washCodes);

            var result = await _context.Regions.Include(o => o.Washes.Where(e => washCodes.Contains(e.Code) && e.Terminals.Count() > 0))
                .ThenInclude(o => o.Terminals)
                .ThenInclude(o => o.IddeviceNavigation)
                .ThenInclude(o => o.IddeviceTypeNavigation)
                .ThenInclude(o => o.DeviceTypeAction)
                .Where(o => regionCodes.Contains(o.Code))
                .Select(o => new RegionWithWashTerminalActions
                {
                    IdRegion = o.Idregion,
                    Code = o.Code,
                    Name = o.Name,
                    Washes = o.Washes.Select(e => new WashWithTerminalsActions 
                    {
                        IdWash = e.Idwash,
                        Code = e.Code,
                        Name = e.Name
                    })
                }).ToListAsync();
            return result;
        }

        public async Task<WashWithTerminalsActions> GetWashWithTerminalsActionsByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("bad value");
            }
            if (!(await _context.Washes.AnyAsync(o => o.Code == code)))
            {
                throw new Exception("not found");
            }

            var wash = await _context.Washes.Where(o => o.Code == code)
                .Include(o => o.Terminals)
                .ThenInclude(o => o.IddeviceNavigation)
                .ThenInclude(o => o.IddeviceTypeNavigation)
                .ThenInclude(o => o.DeviceTypeAction)
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
                }).FirstOrDefaultAsync();

            return wash;
        }
    }

}
