using LoyalityService.Models;
using LoyalityService.Models.WashLoyality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Services
{
    public interface IAsyncDiscountManager
    {
        /// <summary>
        /// рассчитать скидку по коду терминала и номеру клиента
        /// </summary>
        /// <param name="terminalCode">Код терминала</param>
        /// <param name="phone">Номер клиента</param>
        /// <returns></returns>
        Task<Discount> CalculateDiscountAsync(string terminalCode, long phone);

        /// <summary>
        /// Записать новую мойку
        /// </summary>
        /// <param name="terminaCode">Код терминала</param>
        /// <param name="phone">Номер клиента</param>
        /// <returns></returns>
        Task<int> WriteWashingAsync(WashingModel washing);

        /// <summary>
        /// получить код ерминала по его номру телефона
        /// </summary>
        /// <param name="phone">Номер телефона терминала</param>
        /// <returns>Код терминала</returns>
        /// <exception cref="KeyNotFoundException">Если терминал не найден по номеру</exception>
        Task<string> GetTerminalCodeByPhoneAsync(long phone);

        /// <summary>
        /// Получить последнюю мойку клиента
        /// </summary>
        /// <param name="clientPhone">Телефон клиента</param>
        /// <returns>Данные о последней мойке</returns>
        Task<IEnumerable<WashingModel>> GetClientLast10WashingsAsync(long clientPhone);

        /// <summary>
        /// Проверить, что программа существует
        /// </summary>
        /// <param name="programCode">Код программы</param>
        /// <returns></returns>
        Task<bool> IsProgramExistsAsync(string programCode);

        /// <summary>
        /// Проверить, что девайс существует
        /// </summary>
        /// <param name="deviceCode">Код девайса</param>
        /// <returns></returns>
        Task<bool> IsDeviceExistsAsync(string deviceCode);

        Task<ClientPromotions> GetCurrentPromotions(long clientPhone);

        Task<Client> GetClientByPhoneAsync(long clientPhone);
    }
}
