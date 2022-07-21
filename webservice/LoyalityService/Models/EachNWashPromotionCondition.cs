using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class EachNWashPromotionCondition
    {
        /// <summary>
        /// Каждая какая мойка
        /// </summary>
        public int EachN { get; set; }
        /// <summary>
        /// За сколько дней
        /// </summary>
        public int Days { get; set; }
        /// <summary>
        /// Скидка (процентов)
        /// </summary>
        public int Discount { get; set; }
    }
}
