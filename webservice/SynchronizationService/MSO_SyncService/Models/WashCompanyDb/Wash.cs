using System;
using System.Collections.Generic;

namespace MSO_SyncService.Models.WashCompanyDb;

public partial class Wash
{
    public int Idwash { get; set; }

    public string Code { get; set; } = null!;

    public string? Name { get; set; }

    public string? Address { get; set; }

    public int Idregion { get; set; }

    public virtual ICollection<Post> Posts { get; } = new List<Post>();
}
