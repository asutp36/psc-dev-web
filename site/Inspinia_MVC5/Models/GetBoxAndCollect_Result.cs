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
    
    public partial class GetBoxAndCollect_Result
    {
        public Nullable<System.DateTime> reportdate { get; set; }
        public string Wash { get; set; }
        public string WashAddress { get; set; }
        public string data1DataPostname { get; set; }
        public Nullable<int> data1DataBoxm10 { get; set; }
        public Nullable<int> data1DataBoxb10 { get; set; }
        public Nullable<int> data1DataBoxb50 { get; set; }
        public Nullable<int> data1DataBoxb100 { get; set; }
        public Nullable<int> data1DataBoxb500 { get; set; }
        public int data1DataBoxb1k { get; set; }
        public int data1DataM10 { get; set; }
        public int data1DataB10 { get; set; }
        public int data1DataB50 { get; set; }
        public int data1DataB100 { get; set; }
        public int data1DataB500 { get; set; }
        public int data1DataB1k { get; set; }
        public Nullable<int> s { get; set; }
        public int sumofm
        {
            get { return data1DataM10 * 10; }
        }
        public int sumofb
        {
            get { return (int)(data1DataB10 * 10 + data1DataB50 * 50 + data1DataB100 * 100 + data1DataB500 * 200); }
        }
        public int sum
        {
            get { return sumofb + sumofm; }
        }
    }
}