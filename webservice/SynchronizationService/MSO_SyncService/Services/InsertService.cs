using Microsoft.EntityFrameworkCore;
using MSO.SyncService.Exceptions;
using MSO.SyncService.Models;
using MSO.SyncService.Models.WashCompanyDb;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MSO.SyncService.Services
{
    public class InsertService
    {
        private readonly WashCompanyDbContext _context;
        private readonly ILogger<InsertService> _logger;
        private readonly DeviceService _deviceService;
        private readonly EventKindService _eventKindService;
        private readonly ModeService _modeService;

        public InsertService
            (
            WashCompanyDbContext context, 
            ILogger<InsertService> logger, 
            DeviceService deviceService, 
            EventKindService eventKindService, 
            ModeService modeService
            )
        {
            _context = context;
            _logger = logger;
            _deviceService = deviceService;
            _eventKindService = eventKindService;
            _modeService = modeService;
        }

        public async Task<int> InsertEventIncreaseAsync(EventIncreaseDto eventIncreaseDto)
        {
            try
            {
                if (!(await _deviceService.IsExistsAsync(eventIncreaseDto.Device)))
                {
                    throw new CustomStatusCodeException($"Не найден девайс {eventIncreaseDto.Device}", 404);
                }

                if(!(await _eventKindService.IsExistsAsync(eventIncreaseDto.Kind)))
                {
                    throw new CustomStatusCodeException($"Не найден тип события {eventIncreaseDto.Kind}", 404);
                }

                EventIncrease eventIncrease = new EventIncrease()
                {
                    Amount = eventIncreaseDto.Amount,
                    M10 = eventIncreaseDto.m10,
                    B10 = eventIncreaseDto.b10,
                    B50 = eventIncreaseDto.b50,
                    B100 = eventIncreaseDto.b100,
                    B200 = eventIncreaseDto.b200
                };

                Event e = new Event()
                {
                    Dtime = eventIncreaseDto.DTime,
                    Idpost = await _deviceService.GetPostIdByDeviceCode(eventIncreaseDto.Device),
                    IdeventKind = await _eventKindService.GetIdByCodeAsync(eventIncreaseDto.Kind),
                    EventIncrease = eventIncrease
                };

                await _context.Events.AddAsync(e);
                await _context.SaveChangesAsync();

                if(eventIncreaseDto.Kind == "cardincrease" && eventIncreaseDto.CardNum != null)
                {
                    int updatedWashings = await UpdateMobileSendingsAsync(eventIncreaseDto);
                    _logger.LogInformation($"InsertEventIncrease: обновлены в MobileSendings {updatedWashings} записей");
                }

                return e.IdeventKind;
            }
            catch(Exception e)
            {
                _logger.LogError($"SyncService.InsertEventIncrease: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("Не удалось вставить event increase", 513);
            }
        }

        public async Task<int> InsertEventModeAsync(EventModeDto eventModeDto)
        {
            try
            {
                if (!(await _deviceService.IsExistsAsync(eventModeDto.Device)))
                {
                    throw new CustomStatusCodeException($"Не найден девайс {eventModeDto.Device}", 404);
                }

                DateTime? finish = DateTime.Compare(eventModeDto.DTimeFinish, DateTime.Parse("2000-01-01 00:00:00")) <= 0 ?
                            null :
                            eventModeDto.DTimeFinish;

                EventMode eventMode = new EventMode()
                {
                    DtimeStart = eventModeDto.DTimeStart,
                    DtimeFinish = finish,
                    Idmode = await _modeService.GetIdByCode(eventModeDto.Mode),
                    Duration = eventModeDto.Duration,
                    PaymentSign = eventModeDto.PaymentSign,
                    Cost = eventModeDto.Cost,
                    CardTypeCode = eventModeDto.CardTypeCode,
                    CardNum = eventModeDto.CardNum,
                    Discount = eventModeDto.Discount
                };

                Event e = new Event()
                {
                    Dtime = eventModeDto.DTimeStart,
                    Idpost = await _deviceService.GetPostIdByDeviceCode(eventModeDto.Device),
                    IdeventKind = await _eventKindService.GetIdByCodeAsync("mode"),
                    EventMode = eventMode
                };

                await _context.Events.AddAsync(e);
                await _context.SaveChangesAsync();

                return e.IdeventKind;
            }
            catch (Exception e)
            {
                _logger.LogError($"SyncService.InsertEventMode: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("Не удалось вставить event mode", 513);
            }
        }

        public async Task<int> InsertEventCollectAsync(EventCollectDto eventCollectDto)
        {
            try
            {
                if (!(await _deviceService.IsExistsAsync(eventCollectDto.Device)))
                {
                    throw new CustomStatusCodeException($"Не найден девайс {eventCollectDto.Device}", 404);
                }

                EventCollect eventCollect = new EventCollect()
                {
                    Amount = eventCollectDto.Amount,
                    M10 = eventCollectDto.m10,
                    B10= eventCollectDto.b10,
                    B50 = eventCollectDto.b50,
                    B100= eventCollectDto.b100,
                    B200= eventCollectDto.b200
                };

                Event e = new Event()
                {
                    Dtime = eventCollectDto.DTime,
                    Idpost = await _deviceService.GetPostIdByDeviceCode(eventCollectDto.Device),
                    IdeventKind = await _eventKindService.GetIdByCodeAsync("collect"),
                    EventCollect = eventCollect
                };

                await _context.Events.AddAsync(e);
                await _context.SaveChangesAsync();

                return e.Idevent;
            }
            catch (Exception e)
            {
                _logger.LogError($"SyncService.InsertEventCollect: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException("Не удалось вставить event collect", 513);
            }
        }

        private async Task<int> UpdateMobileSendingsAsync(EventIncreaseDto eventIncreaseDto)
        {
            try
            {
                var currentWash = await _context.MobileSendings.Include(p => p.IdpostNavigation).ThenInclude(d => d.IddeviceNavigation)
                            .Include(c => c.IdcardNavigation)
                            .Where(ms => ms.IdpostNavigation.IddeviceNavigation.Code == eventIncreaseDto.Device &&
                                         ms.IdcardNavigation.CardNum == eventIncreaseDto.CardNum &&
                                         ms.DtimeEnd == null &&
                                         ms.DtimeStart <= eventIncreaseDto.DTime.AddSeconds(2))
                            .OrderByDescending(ms => ms.DtimeStart)
                            .FirstOrDefaultAsync();
                
                if(currentWash == null)
                {
                    return 0;
                }
                else
                {
                    int updatedRows = 0;
                    currentWash.DtimeEnd = DateTime.Now;
                    currentWash.Amount = eventIncreaseDto.Amount;
                    _context.Update(currentWash);
                    updatedRows = await _context.SaveChangesAsync();

                    var otherUnstoppedWashings = await _context.MobileSendings.Include(p => p.IdpostNavigation).ThenInclude(d => d.IddeviceNavigation)
                            .Include(c => c.IdcardNavigation)
                            .Where(ms => ms.IdpostNavigation.IddeviceNavigation.Code == eventIncreaseDto.Device &&
                                         ms.IdcardNavigation.CardNum == eventIncreaseDto.CardNum &&
                                         ms.DtimeEnd == null &&
                                         ms.DtimeStart <= eventIncreaseDto.DTime.AddSeconds(2))
                            .ToListAsync();

                    if (otherUnstoppedWashings.Count > 0)
                    {
                        foreach (var unstoppedWashing in otherUnstoppedWashings)
                        {
                            unstoppedWashing.DtimeEnd = DateTime.Now;
                            unstoppedWashing.Amount = 0;
                        }

                        _context.UpdateRange(otherUnstoppedWashings);
                        updatedRows += await _context.SaveChangesAsync();
                    }

                    return updatedRows;
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"UpdateMobileSendingsAsync: {e.GetType()}: {e.Message}");
                return 0;
            }
        }
    }
}
