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
    
    public partial class EventChangerCard
    {
        public int IDEventChangerCard { get; set; }
        public int IDEventChanger { get; set; }
        public System.DateTime DTime { get; set; }
        public string CardNum { get; set; }
        public string Phone { get; set; }
    
        public virtual EventChanger EventChanger { get; set; }
    }
}