using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers.BindingModels
{
    public class CardBindingModel
    {
        [Required]
        public string cardNum { get; set; }
    }
}
