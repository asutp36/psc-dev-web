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
    
    public partial class CountersTotalPre
    {
        public int IDCountersTotalPre { get; set; }
        public int IDPost { get; set; }
        public System.DateTime DTime { get; set; }
        public int b10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b500 { get; set; }
        public int b1k { get; set; }
        public int m10 { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<int> a_b10 { get; set; }
        public Nullable<int> a_b50 { get; set; }
        public Nullable<int> a_b100 { get; set; }
        public Nullable<int> a_b500 { get; set; }
        public Nullable<int> a_b1k { get; set; }
        public Nullable<int> a_m10 { get; set; }
    
        public virtual Post Post { get; set; }
    }
}