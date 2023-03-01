using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class Device
{
    public int Iddevice { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int? ServerId { get; set; }

    public int? IddeviceType { get; set; }

    public string? IpAddress { get; set; }

    public virtual ICollection<Post> Posts { get; } = new List<Post>();
}
