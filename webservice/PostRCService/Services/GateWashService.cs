using Microsoft.EntityFrameworkCore;
using PostRCService.Models.GateWash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostRCService.Services
{
    public class GateWashService
    {
        private readonly GateWashDbContext _context;

        public GateWashService(GateWashDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsWashExistsAsync(string code)
        {
            return await _context.Washes.AnyAsync(o => o.Code == code);
        }

        public async Task<bool> IsDeviceExists(string code)
        {
            return await _context.Devices.AnyAsync(o => o.Code == code);
        }

        public async Task<string> GetDeviceIp(string code)
        {
            return await _context.Devices.Where(o => o.Code == code).Select(o => o.IpAddress).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetPostCodes(string washCode)
        {
            return await _context.Posts.Include(p => p.IddeviceNavigation).Include(p => p.IdwashNavigation).Where(w => w.IdwashNavigation.Code == washCode).Select(x => x.IddeviceNavigation.Code).ToListAsync();
        }
    }
}
