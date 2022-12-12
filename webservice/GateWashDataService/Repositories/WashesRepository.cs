using GateWashDataService.Models.DTOs;
using GateWashDataService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class WashesRepository
    {
        private readonly GateWashDbContext _model;

        public WashesRepository(GateWashDbContext model)
        {
            _model = model;
        }

        /// <summary>
        /// Получить объекты термналов по списку кодо моек
        /// </summary>
        /// <param name="washCodes">Коды моек</param>
        /// <returns>Список объектов терминалов</returns>
        public async Task<List<TerminalDto>> GetTerminalsByWashesAsync(IEnumerable<string> washCodes)
        {
            return await _model.Terminals.Where(t => washCodes.Contains(t.IdwashNavigation.Code))
                            .Include(t => t.IddeviceNavigation)
                            .Select(t => new TerminalDto 
                            {
                                Code = t.IddeviceNavigation.Code
                            })
                            .ToListAsync();
        }

        public async Task<IEnumerable<short>> GetRegionCodessByWashCodes(IEnumerable<string> washCodes)
        {
            var regions = _model.Washes.Include(o => o.IdregionNavigation)
                .Where(o => washCodes.Contains(o.Code)).Select(o => o.IdregionNavigation.Code).Distinct();
            return regions;
        }

        /// <summary>
        /// Получить коды термналов по списку кодов моек
        /// </summary>
        /// <param name="washCodes">Коды моек</param>
        /// <returns>Список кодов терминалов</returns>
        public async Task<List<string>> GetTerminalCodesByWashesAsync(IEnumerable<string> washCodes)
        {
            return await _model.Terminals.Where(t => washCodes.Contains(t.IdwashNavigation.Code))
                            .Include(t => t.IddeviceNavigation)
                            .Select(t => t.IddeviceNavigation.Code)
                            .ToListAsync();
        }
    }
}
