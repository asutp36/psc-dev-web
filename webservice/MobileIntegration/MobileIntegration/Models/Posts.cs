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
    
    public partial class Posts
    {
        public int IDPost { get; set; }
        public int IDWash { get; set; }
        public Nullable<int> IDDevice { get; set; }
        public string QRCode { get; set; }
    
        public virtual Device Device { get; set; }
    }
}
