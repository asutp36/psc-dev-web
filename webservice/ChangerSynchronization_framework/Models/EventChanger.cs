//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChangerSynchronization_framework.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventChanger
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EventChanger()
        {
            this.EventChangerAcquiring = new HashSet<EventChangerAcquiring>();
            this.EventChangerCard = new HashSet<EventChangerCard>();
            this.EventChangerIncrease = new HashSet<EventChangerIncrease>();
            this.EventChangerOut = new HashSet<EventChangerOut>();
        }
    
        public int IDEventChanger { get; set; }
        public int IDChanger { get; set; }
        public int IDEventChangerKind { get; set; }
        public System.DateTime DTime { get; set; }
    
        public virtual Changers Changers { get; set; }
        public virtual EventChangerKind EventChangerKind { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerAcquiring> EventChangerAcquiring { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerCard> EventChangerCard { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerIncrease> EventChangerIncrease { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerOut> EventChangerOut { get; set; }
    }
}
