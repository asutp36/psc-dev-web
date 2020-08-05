using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChangerSynchronization_framework.Controllers.Supplies
{
    /// <summary>
    /// Модель записи операции по карте с разменника
    /// </summary>
    public class OperationFromRequest
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
        /// Время совершения операции
        /// </summary>
        [Required]
        public DateTime dtime { get; set; }

        /// <summary>
        /// Код типа операции
        /// </summary>
        [Required]
        public string operationType { get; set; }

        /// <summary>
        /// Сумма операции
        /// </summary>
        [Required]
        public int amount { get; set; }

        /// <summary>
        /// Баланс карты после операции
        /// </summary>
        [Required]
        public int balance { get; set; }

        /// <summary>
        /// ID в локальной базе
        /// </summary>
        [Required]
        public int localizedID { get; set; }
    }
}