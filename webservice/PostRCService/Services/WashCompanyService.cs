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

        public async Task GetParameterByWashAsync<T>(string washCode)
        {
            if (!await this.IsWashExistsAsync(washCode))
            {
                _logger.LogError($"Не найдена мойка с кодом {washCode}");
                throw new CustomStatusCodeException(HttpStatusCode.NotFound, $"Не найдена мойка {washCode}", "");
            }

            WashParameter<T> washParameter = new WashParameter<T>();
            washParameter.washCode = washCode;
            washParameter.posts = new List<PostParameter<T>>();

            IEnumerable<string> postCodes = await this.GetPostCodesAsync(washCode);
            foreach(string p in postCodes)
            {
                PostParameter<T> postParameter = new PostParameter<T>();

                string ip = await this.GetDeviceIpAsync(p);
                if (ip == null)
                {
                    _logger.LogError($"Не найден ip поста {p}");
                    continue;
                }

                HttpResponse response = new HttpResponse();
                if(typeof(T) == typeof(RatesModel))
                {
                    response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");
                }
                
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if(response.StatusCode == 0)
                    {
                        _logger.LogError($"Нет связи с постом {p}");
                        washParameter.posts.Add(postParameter);
                        continue;
                    }

                    _logger.LogError($"Ответ поста {p}: {response.ResultMessage}");
                    washParameter.posts.Add(postParameter);
                    continue;
                }

                if(typeof(T) == typeof(RatesModel))
                { 
                    .rates = JsonConvert.DeserializeObject<List<FunctionRate>>(response.ResultMessage);
                }
            }
        }
    }
}
