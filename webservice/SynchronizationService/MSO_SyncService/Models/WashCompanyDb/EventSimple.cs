using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class EventSimple
{
    public int Idevent { get; set; }

    public int? Counter { get; set; }

    public virtual Event IdeventNavigation { get; set; } = null!;
}
