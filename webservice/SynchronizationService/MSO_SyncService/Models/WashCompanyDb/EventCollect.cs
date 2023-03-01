using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class EventCollect
{
    public int Idevent { get; set; }

    public int? Amount { get; set; }

    public int? M10 { get; set; }

    public int? B10 { get; set; }

    public int? B50 { get; set; }

    public int? B100 { get; set; }

    public int? B200 { get; set; }

    public virtual Event IdeventNavigation { get; set; } = null!;
}
