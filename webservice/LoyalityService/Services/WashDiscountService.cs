using LoyalityService.Models;
using LoyalityService.Models.WashLoyality;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data;

namespace LoyalityService.Services
{
    public class WashDiscountService : IAsyncDiscountManager
    {
        private readonly WashLoyalityDbContext _context;
        private readonly ILogger _logger;
        private readonly IPostRCCaller _postRCService;

        public WashDiscountService(WashLoyalityDbContext context, ILogger<WashDiscountService> logger, IPostRCCaller postRC)
        {
            _context = context;
            _logger = logger;
            _postRCService = postRC;
        }

        public async Task<Discount> CalculateDiscountAsync(string terminalCode, long phone)
        {
            // получаю группу, в которую входит терминал
            Group group = await GetWashGroupByTerminalCodeAsync(terminalCode);

            // все скидки, под которые попадает этот телефон (код скидки: величина)
            Dictionary<string, int> availibleDiscounts = new Dictionary<string, int>();
            var proms = await _context.Promotions.Where(o => o.Idgroup == group.Idgroup)
                                           .Include(o => o.EachNwashCondition)
                                           .Include(o => o.HappyHourCondition)
                                           .Include(o => o.HolidayCondition)
                                           .Include(o => o.VipCondition)
                                           .OrderBy(o => o.ApplyOrder)
                                           .ToListAsync();

            Discount discount = new Discount();

            foreach(var p in proms)
            {
                if (p.EachNwashCondition != null && CheckEachNWashCondition(p.EachNwashCondition, phone)) 
                {
                    _logger.LogInformation($"Клиенту {phone} подходит скидка \"каждая {p.EachNwashCondition.EachN} мойка за {p.EachNwashCondition.Days} дней\"");
                    discount.Percent = p.Discount ?? 0;
                    discount.Ruble = p.DiscountRub ?? 0;
                    discount.Programs = p.Programs;
                }

                if (p.HappyHourCondition != null && CheckHappyHourCondition(p.HappyHourCondition)) 
                {
                    _logger.LogInformation($"Клиент {phone} попал в счастливые часы с {p.HappyHourCondition.HourBegin} до {p.HappyHourCondition.HourEnd}");
                    discount.Percent = p.Discount ?? 0;
                    discount.Ruble = p.DiscountRub ?? 0;
                    discount.Programs = p.Programs;
                }

                if (p.HolidayCondition != null && DateTime.Now.Date == p.HolidayCondition.Date.Date) 
                {
                    _logger.LogInformation($"Клиент {phone} попал на праздничный день {p.HolidayCondition.Date}");
                    discount.Percent = p.Discount ?? 0;
                    discount.Ruble = p.DiscountRub ?? 0;
                    discount.Programs = p.Programs;
                }

                if (p.VipCondition != null && CheckVipCondition(p.VipCondition, phone)) 
                {
                    _logger.LogInformation($"Вип клиент {phone}");
                    discount.Percent = p.Discount ?? 0;
                    discount.Ruble = p.DiscountRub ?? 0;
                    discount.Programs = p.Programs;
                }

                if (discount.Percent > 0 || discount.Ruble > 0)
                {
                    break;
                }
            }

            if (discount.Ruble < 100 && discount.Percent == 0 && CheckIfTaxi(phone))
            {
                discount.Ruble = 100;
                discount.Percent = 0;
                discount.Programs = "program1, program2, program3, program4";
            }

            _logger.LogInformation($"Запуск поста со скидкой: {JsonConvert.SerializeObject(discount)}");
            return discount;
        }

        private bool CheckIfTaxi(long phone)
        {
            int weekWashingsCount = _context.Washings.Count(o => o.IdclientNavigation.Phone == phone
                                                                    && o.Dtime.Date >= DateTime.Now.Date.AddDays(-7));
            if(weekWashingsCount >= 3)
            {
                _logger.LogInformation($"Клиент {phone} таксист, потому что это его {weekWashingsCount} мойка за неделю");
                return true;
            }

            int clientWashingsCount = _context.Washings.Count(o => o.IdclientNavigation.Phone == phone);
            
            if(weekWashingsCount == clientWashingsCount)
            {
                _logger.LogInformation($"Клиент {phone} таксист, потому что это его {weekWashingsCount} за неделю при общем количестве моек = {clientWashingsCount}");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Проверить, подходит ли клиент под условия акции
        /// </summary>
        /// <param name="condition">Условие акции</param>
        /// <param name="clientPhone">Номер телефона клиента</param>
        /// <returns></returns>
        private bool CheckEachNWashCondition(EachNwashCondition condition, long clientPhone)
        {
            // посчитать все мойки клиента за последние Days (из условия скидки)
            int clientWashingsCount = _context.Washings.Count(o => o.IdclientNavigation.Phone == clientPhone 
                                                                && (condition.Days == 0 || o.Dtime.Date >= DateTime.Now.Date.AddDays(-condition.Days)));

            // если количество моек > 0 и эта мойка будет энной, скидка будет из условия или 0
            return (clientWashingsCount + 1) % condition.EachN == 0;
        }

        /// <summary>
        /// Проверить, действует ли сейчас скидка "счастливый час"
        /// </summary>
        /// <param name="condition">Условия скидки</param>
        /// <returns></returns>
        private bool CheckHappyHourCondition(HappyHourCondition condition)
        {
            int currentHour = DateTime.Now.Hour;

            return currentHour >= condition.HourBegin && currentHour < condition.HourEnd;
        }

        private bool CheckVipCondition(VipCondition condition, long clientPhone)
        {
            if (condition.Phone != clientPhone)
            {
                return false;
            }

            // посчитать все мойки клиента за последние Days (из условия скидки)
            int clientWashingsCount = _context.Washings.Count(o => o.IdclientNavigation.Phone == clientPhone
                                                                && (condition.Days == 0 || o.Dtime.Date >= DateTime.Now.Date.AddDays(-condition.Days)));
            
            _logger.LogInformation($"Определние вип скидки: телефон клиента: {clientPhone}, количество моек за {condition.Days} дней: {clientWashingsCount}, условие скидки: {condition.Amount}");

            return clientPhone == condition.Phone && clientWashingsCount <= condition.Amount;
        }

        private async Task<Group> GetWashGroupByTerminalCodeAsync(string terminalCode)
        {
            var group = await _context.Terminals.Where(o => o.IddeviceNavigation.Code == terminalCode)
                .Select(o => o.IdwashNavigation.IdgroupNavigation)
                .FirstOrDefaultAsync();

            return group;
        }

        public async Task<int> WriteWashingAsync(WashingModel washing)
        {
            Client client;
            try
            {
                // получаем клиента по его номеру
                client = await GetClientByPhoneAsync(washing.ClientPhone);
            }
            catch (KeyNotFoundException e)
            {
                // создаём клиента, если его не удалось найти
                client = await CreateClientAsync(washing.ClientPhone);
            }

            // если клиента всё равно нет, то кидаю исключение InvalidOperationException
            if(client == null)
            {
                throw new InvalidOperationException($"С номером телефона {washing.ClientPhone} не удалось ни найти пользователя, ни создать его");
            }

            // создаю объект (запуск мойки), который буду записывать в бд
            Washing toAdd = new Washing()
            {
                Idclient = client.Idclient,
                Dtime = DateTime.Parse(washing.DTime),
                Amount = washing.Amount,
                Discount = washing.Discount,
                DiscountRub = washing.DiscountRub,
                Iddevice = await GetDeviceIdByCode(washing.Device),
                Idprogram = await GetProgramIdByCode(washing.Program),
                Guid = washing.Guid
            };

            await _context.Washings.AddAsync(toAdd);

            try 
            {
                await _context.SaveChangesAsync();
                return toAdd.Idwashing;
            }
            catch(DbUpdateException e)
            {
                // если не удалось добавить запуск мойки, вернуть -1, что значит операия нормально не завершилась
                _logger.LogError($"При добавлении нового запуска мойки в базу данных произошла ошибка. {e.GetType()}: {e.Message}");
                throw new InvalidOperationException($"Не удалось записать в базу данных новый запуск мойки ({e.Message})");
            }
        }

        /// <summary>
        /// Получить клиента оп его номеру
        /// </summary>
        /// <param name="phone">Номер телефона клиента</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">Клиент не найден</exception>
        public async Task<Client> GetClientByPhoneAsync(long phone)
        {
            var client = await _context.Clients.Where(o => o.Phone == phone).FirstOrDefaultAsync();
            if (client == null)
                throw new KeyNotFoundException($"Клиент с номером {phone} не найден");

            return client;
        }

        /// <summary>
        /// Записать нового клиента
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>Клиент, если удалось создать</returns>
        private async Task<Client> CreateClientAsync(long phone)
        {
            try
            {
                Client client = new Client()
                {
                    Phone = phone
                };

                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                return client;
            }
            catch (DbUpdateException e)
            {
                _logger.LogError($"| WashDiscountService.CreateClient | Произошла ошибка при добавлении нового клиента в базу данных. {e.GetType()}: {e.Message}");
                return null;
            }
        }

        public async Task<string> GetTerminalCodeByPhoneAsync(long phone)
        {
            string code = await _context.Terminals.Where(t => t.Phone == phone)
                            .Select(t => t.IddeviceNavigation.Code)
                            .FirstOrDefaultAsync();

            // если терминал не найден - выкинуть KeyNotFoundException
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogError($"Не удалось найти терминал по номеру телефона {phone}");
                throw new KeyNotFoundException($"Не удалось найти терминал по номеру телефона {phone}");
            }

            return code;
        }

        public async Task<IEnumerable<WashingModel>> GetClientLast10WashingsAsync(long clientPhone)
        {
            var washing = _context.Washings.Where(o => o.IdclientNavigation.Phone == clientPhone)
                .OrderByDescending(o => o.Dtime)
                .Select(o => new WashingModel
                {
                    ClientPhone = o.IdclientNavigation.Phone,
                    DTime = o.Dtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Device = o.IddeviceNavigation.Name,
                    Program = o.IdprogramNavigation.Name,
                    Amount = o.Amount,
                    Discount = o.Discount,
                    DiscountRub = o.DiscountRub ?? 0
                })
                .Take(10)
                .AsEnumerable();

            return washing;
        }

        public async Task<bool> IsProgramExistsAsync(string programCode)
        {
            var program = await _context.Programs.Where(o => o.Code == programCode).FirstOrDefaultAsync();

            return program != null;
        }

        public async Task<bool> IsDeviceExistsAsync(string deviceCode)
        {
            var device = await _context.Devices.Where(o => o.Code == deviceCode).FirstOrDefaultAsync();

            return device != null;
        }

        /// <summary>
        /// Получить id девайса по его коду
        /// </summary>
        /// <param name="deviceCode">Код девайса</param>
        /// <returns>id девайса</returns>
        private async Task<int> GetDeviceIdByCode(string deviceCode)
        {
            var device = await _context.Devices.Where(o => o.Code == deviceCode).FirstOrDefaultAsync();

            return device.Iddevice;
        }

        /// <summary>
        /// Получить id программы по её коду
        /// </summary>
        /// <param name="programCode">Код программы</param>
        /// <returns>id программы</returns>
        private async Task<int> GetProgramIdByCode(string programCode)
        {
            var program = await _context.Programs.Where(o => o.Code == programCode).FirstOrDefaultAsync();

            return program.Idprogram;
        }

        public async Task<ClientPromotions> GetCurrentPromotions(long clientPhone)
        {
            ClientPromotions result = new ClientPromotions();

            result.EachNWash = await GetCurrentStatusAsync(clientPhone);

            result.Holiday = await GetHolidayPromotionAsync();

            result.HappyHour = await GetHappyHourAsync();

            return result;
        }

        private async Task<ClientEachNWashStatus> GetCurrentStatusAsync(long phone)
        {
            ClientEachNWashStatus currentStatus = new ClientEachNWashStatus();

            Promotion p = await _context.Promotions.Include(o => o.EachNwashCondition).Where(o => o.EachNwashCondition != null).FirstOrDefaultAsync();

            if (p == null)
                return null;

            currentStatus.CurrentWashCount = await _context.Washings.Where(o => o.IdclientNavigation.Phone == phone).CountAsync() % p.EachNwashCondition.EachN;
            currentStatus.N = p.EachNwashCondition.EachN;
            currentStatus.Discount = p.Discount;
            currentStatus.DiscountRub = p.DiscountRub;
            return currentStatus;
        }

        private async Task<HolidayPromotion> GetHolidayPromotionAsync()
        {
            Promotion p = await _context.Promotions.Include(o => o.HolidayCondition).Where(o => o.HolidayCondition != null && o.HolidayCondition.Date >= DateTime.Now.Date)
                                                         .OrderBy(o => o.HolidayCondition.Date).FirstOrDefaultAsync();
            if (p == null)
                return null;

            HolidayPromotion result = new HolidayPromotion()
            {
                Date = p.HolidayCondition.Date,
                Discount = p.Discount,
                DiscountRub = p.DiscountRub
            };

            return result;
        }

        private async Task<HappyHourPromotion> GetHappyHourAsync()
        {
            Promotion p = await _context.Promotions.Include(o => o.HappyHourCondition).Where(o => o.HappyHourCondition != null).FirstOrDefaultAsync();

            if (p == null)
                return null;

            HappyHourPromotion result = new HappyHourPromotion()
            {
                BeginHour = p.HappyHourCondition.HourBegin,
                EndHour = p.HappyHourCondition.HourEnd,
                Discount = p.Discount,
                DiscountRub = p.DiscountRub
            };

            return result;
        }
    }
}
