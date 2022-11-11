using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostRCService.Controllers.BindingModels;
using PostRCService.Controllers.Helpers;
using PostRCService.Models;
using PostRCService.Models.WashCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PostRCService.Services
{
    public class WashCompanyService
    {
        private readonly WashCompanyDbContext _context;
        private readonly ILogger<WashCompanyService> _logger;

        public WashCompanyService(WashCompanyDbContext context, ILogger<WashCompanyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsWashExistsAsync(string code)
        {
            return await _context.Washes.AnyAsync(o => o.Code == code);
        }

        public async Task<bool> IsDeviceExistsAsync(string code)
        {
            return await _context.Devices.AnyAsync(o => o.Code == code);
        }

        public async Task<string> GetDeviceIpAsync(string code)
        {
            return await _context.Devices.Where(o => o.Code == code).Select(o => o.IpAddress).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetPostCodesAsync(string washCode)
        {
            return await _context.Posts.Include(p => p.IddeviceNavigation).Include(p => p.IdwashNavigation).Where(w => w.IdwashNavigation.Code == washCode).Select(x => x.IddeviceNavigation.Code).ToListAsync();
        }

        public async Task SetParameterOnWashAsync<T>(SetParametersWash<T> parameter)
        {
            
        }

    }
}
