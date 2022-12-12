using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Services
{
    public class EventKindService
    {
        private readonly GateWashDbContext _context;

        public EventKindService(GateWashDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventKindModel>> GetAsFilters(IEnumerable<string> washes)
        {
            List<EventKindModel> eventKinds = await _context.EventKindWashes.Include(w => w.IdwashNavigation)
                                                                            .Include(ek => ek.IdeventKindNavigation)
                                                    .Where(ek => washes.Contains(ek.IdwashNavigation.Code))
                                                    .Select(ek => new EventKindModel
                                                    {
                                                        Code = ek.IdeventKindNavigation.Code,
                                                        Name = ek.IdeventKindNavigation.Name
                                                    })
                                                    .Distinct()
                                                    .OrderBy(o => o.Name)
                                                    .ToListAsync();
            return eventKinds;
        }
    }
}
