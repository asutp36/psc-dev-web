//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inspinia_MVC5.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventChanger
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EventChanger()
        {
            this.EventChangerAcquirings = new HashSet<EventChangerAcquiring>();
            this.EventChangerCards = new HashSet<EventChangerCard>();
            this.EventChangerIncreases = new HashSet<EventChangerIncrease>();
            this.EventChangerOuts = new HashSet<EventChangerOut>();
        }
    
        public int IDEventChanger { get; set; }
        public int IDChanger { get; set; }
        public int IDEventChangerKind { get; set; }
        public System.DateTime DTime { get; set; }
    
        public virtual Changer Changer { get; set; }
        public virtual EventChangerKind EventChangerKind { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerAcquiring> EventChangerAcquirings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerCard> EventChangerCards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerIncrease> EventChangerIncreases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventChangerOut> EventChangerOuts { get; set; }
    }
}
