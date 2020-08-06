using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers
{
    public class CardsApi
    {
        private ModelDbContext _model = new ModelDbContext();

        public void WriteIncrease(IncreaseFromChanger model) 
        {
            
        }

        public void GetBalance(string cardNum) { }

        public void UpdatePhone() { }

        public void WriteNewCard() { }

        public void GetCardsByPhone(string phone) { }
    }
}
