namespace Inspinia_MVC5.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Card
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Card()
        {
            Operations = new HashSet<Operation>();
        }

        public int CardID { get; set; }

        public int OwnerID { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string CardNum { get; set; }

        public int CardStatusID { get; set; }

        public int CardTypeID { get; set; }

        public virtual CardStatus CardStatus { get; set; }

        public virtual CardType CardType { get; set; }

        public virtual Owner Owner { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Operation> Operations { get; set; }
    }
}
