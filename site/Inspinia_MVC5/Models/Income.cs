namespace Inspinia_MVC5.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Income
    {
        [Key]
        public int IDCarwash { get; set; }

        [Required]
        [StringLength(50)]
        public string WashName { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string Address { get; set; }

        public int Incomes { get; set; }
    }
}
