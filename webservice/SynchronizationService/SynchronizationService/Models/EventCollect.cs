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
    
    public partial class EventCollect
    {
        public int IDEvent { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<int> m10 { get; set; }
        public Nullable<int> b10 { get; set; }
        public Nullable<int> b50 { get; set; }
        public Nullable<int> b100 { get; set; }
        public Nullable<int> b200 { get; set; }
    
        public virtual Event Event { get; set; }
    }
}
