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
    
    public partial class GetCardList_Result
    {
        public string Phone { get; set; }
        public string CardNum { get; set; }
        public string CardTypeName { get; set; }
        public string CardStatusName { get; set; }
        public int Balance { get; set; }
        public System.DateTime ActivationDate { get; set; }
        public int ActivationBy { get; set; }
        public System.DateTime LastOperationDate { get; set; }
        public int LastOperationBy { get; set; }
        public int IncreaseSum { get; set; }
        public int DecreaseSum { get; set; }
        public Nullable<int> CountOperation { get; set; }
    }
}
