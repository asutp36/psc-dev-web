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
    
    public partial class GetIncreaseByPosts_Result
    {
        public short RegionCode { get; set; }
        public string RegionName { get; set; }
        public string WashCode { get; set; }
        public string WashAddress { get; set; }
        public string PostCode { get; set; }
        public string PostName { get; set; }
        public Nullable<int> m10 { get; set; }
        public int b10 { get; set; }
        public Nullable<int> b50 { get; set; }
        public Nullable<int> b100 { get; set; }
        public Nullable<int> b200 { get; set; }
        public int sumofm
        {
            get { return (int)(m10 * 10); }
        }
        public int sumofb
        {
            get { return (int)(b10 * 10 + b50 * 50 + b100 * 100 + b200 * 200); }
        }
        public int sum
        {
            get { return sumofb + sumofm; }
        }
    }
}
