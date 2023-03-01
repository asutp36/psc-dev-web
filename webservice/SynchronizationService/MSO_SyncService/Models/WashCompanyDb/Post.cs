using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class Post
{
    public int Idpost { get; set; }

    public int Idwash { get; set; }

    public int? Iddevice { get; set; }

    public string? Qrcode { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Event> Events { get; } = new List<Event>();

    public virtual Device? IddeviceNavigation { get; set; }

    public virtual Wash IdwashNavigation { get; set; } = null!;

    public virtual ICollection<MobileSending> MobileSendings { get; } = new List<MobileSending>();
}
