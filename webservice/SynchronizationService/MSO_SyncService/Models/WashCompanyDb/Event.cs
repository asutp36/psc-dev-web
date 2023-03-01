using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class Event
{
    public int Idevent { get; set; }

    public int Idpost { get; set; }

    public int IdeventKind { get; set; }

    public DateTime Dtime { get; set; }

    public int? IdeventPost { get; set; }

    public virtual EventCollect? EventCollect { get; set; }

    public virtual EventIncrease? EventIncrease { get; set; }

    public virtual EventMode? EventMode { get; set; }

    public virtual EventSimple? EventSimple { get; set; }

    public virtual EventKind IdeventKindNavigation { get; set; } = null!;

    public virtual Post IdpostNavigation { get; set; } = null!;
}
