using System;
using System.Collections.Generic;

#nullable disable

namespace AppWasteSenderService.Models.WashCompanyDb
{
    public partial class Post
    {
        public Post()
        {
            MobileSendings = new HashSet<MobileSending>();
        }

        public int Idpost { get; set; }
        public int Idwash { get; set; }
        public int? Iddevice { get; set; }
        public string Qrcode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
        public virtual ICollection<MobileSending> MobileSendings { get; set; }
    }
}
