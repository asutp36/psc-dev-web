//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CashIntegration.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Event
    {
        public int IDEvent { get; set; }
        public int IDPost { get; set; }
        public int IDEventKind { get; set; }
        public System.DateTime DTime { get; set; }
        public Nullable<int> IDEventPost { get; set; }
    
        public virtual Posts Posts { get; set; }
        public virtual EventIncrease EventIncrease { get; set; }
    }
}
