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

        [Column(TypeName = "text")]
        [Required]
        public string WashName { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Region { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Adress { get; set; }

        public int Incomes { get; set; }
    }
}
