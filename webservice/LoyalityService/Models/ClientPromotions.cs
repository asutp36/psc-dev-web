using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Models
{
    public class ClientPromotions
    {
        public ClientEachNWashStatus EachNWash { get; set; }
        public HolidayPromotion Holiday { get; set; }
        public HappyHourPromotion HappyHour { get; set; }
    }
}
