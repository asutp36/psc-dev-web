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
    
    public partial class Operations
    {
        public int IDOperation { get; set; }
        public int IDPsc { get; set; }
        public int IDOperationType { get; set; }
        public int IDCard { get; set; }
        public System.DateTime DTime { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedID { get; set; }
        public string Functions { get; set; }
        public string Details { get; set; }
    
        public virtual Cards Cards { get; set; }
        public virtual OperationTypes OperationTypes { get; set; }
        public virtual Psces Psces { get; set; }
    }
}
