//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SynchronizationService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Posts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Posts()
        {
            this.Event = new HashSet<Event>();
        }
    
        public int IDPost { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int IDWash { get; set; }
        public Nullable<int> IDDevice { get; set; }
    
        public virtual Device Device { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> Event { get; set; }
        public virtual Wash Wash { get; set; }
    }
}