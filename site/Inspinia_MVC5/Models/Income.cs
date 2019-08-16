namespace Inspinia_MVC5.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Income")]
    public partial class Income
    {
        [Key]
        public int CarWashID { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Region { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Address { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Incomes { get; set; }
    }
}
