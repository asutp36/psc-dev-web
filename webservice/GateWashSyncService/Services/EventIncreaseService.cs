using GateWashSyncService.Controllers.BindingModels;
using GateWashSyncService.Models.GateWash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Services
{
    public class EventIncreaseService
    {
        private readonly ILogger<EventIncreaseService> _logger;
        private readonly GateWashDbContext _context;
        private readonly PaySessionsService _paySessionsService;
        private readonly EventKindsService _eventKindsService;
        private readonly DevicesService _devicesSevice;

        public EventIncreaseService(ILogger<EventIncreaseService> logger, GateWashDbContext context, EventKindsService eventKindsService, DevicesService devicesService, 
            PaySessionsService paySessionsService)
        {
            _logger = logger;
            _context = context;
            _eventKindsService = eventKindsService;
            _devicesSevice = devicesService;
            _paySessionsService = paySessionsService;
        }

        public async Task<int> InsertAsync(EventIncreaseBindingModel model)
        {
            try
            {
                if(!(await _devicesSevice.CheckIfExistsByCodeAsync(model.deviceCode)))
                {
                    _logger.LogError($"EventIncreaseService.InsertAsync: не найден девайс {model.deviceCode}");
                    return -1;
                }

                if(!(await _eventKindsService.CheckIfExistsByCodeAsync(model.eventKindCode)))
                {
                    _logger.LogError($"EventIncreaseService.InsertAsync: не найден тип внесения {model.eventKindCode}");
                    return -1;
                }

                if(!(await _paySessionsService.CheckIfExistsAsync(model.idSessionOnPost, model.deviceCode)))
                {
                    _logger.LogError($"EventIncreaseService.InsertAsync: сессия с id={model.idSessionOnPost} на девайсе {model.deviceCode} не записана");
                    return -1;
                }

                if(await this.CheckIfExistsAsync(model.idEventOnPost, model.deviceCode))
                {
                    _logger.LogError($"EventIncreaseService.InsertAsync: уже записано событие id={model.idEventOnPost} на девайсе {model.deviceCode}");
                    return -1;
                }

                EventIncrease ei = new EventIncrease
                {
                    Amount = model.amount,
                    Profit = model.amount * (100 - await this.GetFee(model.eventKindCode, model.deviceCode)) / 100,
                    M10 = model.m10,
                    B50 = model.b50,
                    B100 = model.b100,
                    B200 = model.b200,
                    B500 = model.b500,
                    B1000 = model.b1000,
                    B2000 = model.b2000
                };

                PayEvent pe = new PayEvent 
                {
                    IdpaySession = await _paySessionsService.GetIdyLocalIdAndDevice(model.idSessionOnPost, model.deviceCode),
                    IdeventOnPost = model.idEventOnPost,
                    Iddevice = await _devicesSevice.GetIdByCode(model.deviceCode),
                    IdeventKind = await _eventKindsService.GetIdByCode(model.eventKindCode),
                    Dtime = DateTime.Parse(model.dtime),
                    EventIncrease = ei
                };

                await _context.PayEvent.AddAsync(pe);
                await _context.SaveChangesAsync();

                return pe.IdpayEvent;
            }
            catch (Exception e) 
            {
                _logger.LogError($"EventIncreaseService.InsertAsync: {e.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idEventOnPost">ID события на посту</param>
        /// <param name="deviceCode">Код девайса</param>
        /// <returns>bool</returns>
        public async Task<bool> CheckIfExistsAsync(int idEventOnPost, string deviceCode)
        {
            if (idEventOnPost < 0 || string.IsNullOrEmpty(deviceCode))
                return false;

            return await _context.Event.AnyAsync(o => o.IdeventOnPost == idEventOnPost && o.IddeviceNavigation.Code == deviceCode);
        }

        /// <summary>
        /// Получить комиссию по коду девайса и коду типа события
        /// </summary>
        /// <param name="eventKindCode">Код типа события</param>
        /// <param name="deviceCode">Код девайса</param>
        /// <returns>Комиссия в целых процентах</returns>
        public async Task<int> GetFee(string eventKindCode, string deviceCode)
        {
            var result =  await _context.EventKindWashFee.Include(o => o.IdwashNavigation)
                                                    .ThenInclude(o => o.Terminals).Where(o => o.IdwashNavigation.Terminals.Any(t => t.IddeviceNavigation.Code == deviceCode))
                                                  .Include(o => o.IdeventKindNavigation).Where(o => o.IdeventKindNavigation.Code == eventKindCode)
                                                  .Select(o => o.Fee).FirstOrDefaultAsync();
            return result;
        }
    }
}
