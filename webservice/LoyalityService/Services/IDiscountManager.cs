﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Services
{
    interface IDiscountManager
    {
        /// <summary>
        /// рассчитать скидку по коду терминала и номеру клиента
        /// </summary>
        /// <param name="terminalCode">Код терминала</param>
        /// <param name="phone">Номер клиента</param>
        /// <returns></returns>
        Task<int> CalculateDiscountAsync(string terminalCode, long phone);

        /// <summary>
        /// Записать новую мойку
        /// </summary>
        /// <param name="terminaCode">Код терминала</param>
        /// <param name="phone">Номер клиента</param>
        /// <returns></returns>
        Task WriteWashingAsync(string terminaCode, long phone);

        /// <summary>
        /// получить код ерминала по его номру телефона
        /// </summary>
        /// <param name="phone">Номер телефона терминала</param>
        /// <returns></returns>
        Task<string> GetTerminalCodeByPhoneAsync(long phone);
    }
}
