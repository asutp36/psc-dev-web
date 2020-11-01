using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Filters
{
    public class CardsFilters
    {
        public List<CardTypeViewModel> cardTypes { get; set; }
        public List<CardStatusViewModel> cardStatuses { get; set; }
        public List<ActivationDeviceViewModel> activationDevices { get; set; }
        public List<LastOperationDeviceViewModel> lastOperationDevices { get; set; }

        private ModelDbContext _model = new ModelDbContext();

        public CardsFilters()
        {
            cardTypes = GetCardTypes();
            cardStatuses = GetCardStatuses();
        }

        private List<CardTypeViewModel> GetCardTypes()
        {
            List<CardTypeViewModel> result = new List<CardTypeViewModel>();

            List<CardTypes> types = _model.CardTypes.ToList();
            foreach(CardTypes ct in types)
            {
                result.Add(new CardTypeViewModel { code = ct.Code, name = ct.Name });
            }

            return result;
        }

        private List<CardStatusViewModel> GetCardStatuses()
        {
            List<CardStatusViewModel> result = new List<CardStatusViewModel>();

            List<CardStatuses> statuses = _model.CardStatuses.ToList();
            foreach(CardStatuses cs in statuses)
            {
                result.Add(new CardStatusViewModel { code = cs.Code, name = cs.Name });
            }

            return result;
        }
    }
}
