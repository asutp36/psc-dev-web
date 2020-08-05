using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangerSynchronization_framework.Controllers.Supplies
{
    /// <summary>
    /// Модель записи выпущенной карты разменника
    /// </summary>
    public class NewCard
    {
        /// <summary>
        /// Код разменника
        /// </summary>
        [Required]
        public string changer { get; set; }

        /// <summary>
        /// Номер выпущенной карты
        /// </summary>
        [Required]
        public string cardNum { get; set; }


        /// <summary>
        /// Телефон владельца
        /// </summary>
        [Required]
        public string phone { get; set; }

        /// <summary>
        /// ID в локальной базе
        /// </summary>
        [Required]
        public int localizedID { get; set; }
    }
}