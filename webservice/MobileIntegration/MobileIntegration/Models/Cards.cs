//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MobileIntegration.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cards
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cards()
        {
            this.Operations = new HashSet<Operations>();
        }
    
        public int IDCard { get; set; }
        public int IDOwner { get; set; }
        public string CardNum { get; set; }
        public int IDCardStatus { get; set; }
        public int IDCardType { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedID { get; set; }
    
        public virtual CardStatuses CardStatuses { get; set; }
        public virtual CardTypes CardTypes { get; set; }
        public virtual Owners Owners { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Operations> Operations { get; set; }
    }
}
