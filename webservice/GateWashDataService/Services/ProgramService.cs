using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Services
{
    public class ProgramService
    {
        private readonly GateWashDbContext _context;

        public ProgramService(GateWashDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить программы для фильтров
        /// </summary>
        /// <param name="washes">Список моек</param>
        /// <returns></returns>
        public async Task<List<ProgramModel>> GetProgramsAsFiltersAsync(IEnumerable<string> washes)
        {
            List<ProgramModel> programs = await _context.ProgramWashes.Include(p => p.IdprogramNavigation)
                                                                      .Include(w => w.IdwashNavigation)
                                               .Where(o => washes.Contains(o.IdwashNavigation.Code))
                                               .Select(o => new ProgramModel
                                               {
                                                   Code = o.IdprogramNavigation.Code,
                                                   Name = o.IdprogramNavigation.Name
                                               })
                                               .Distinct()
                                               .OrderBy(o => o.Name)
                                               .ToListAsync();
            return programs;
        }
    }
}
