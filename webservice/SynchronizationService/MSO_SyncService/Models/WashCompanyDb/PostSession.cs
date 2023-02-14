using System;
using System.Collections.Generic;

namespace MSO_SyncService.Models.WashCompanyDb;

public partial class PostSession
{
    public int IdpostSession { get; set; }

    public int Idpost { get; set; }

    public int IdsessionOnPost { get; set; }

    public DateTime StartDtime { get; set; }

    public string Qr { get; set; } = null!;

    public string? FiscalError { get; set; }

    public DateTime StopDtime { get; set; }

    public int AmountCash { get; set; }

    public int AmountBank { get; set; }

    public virtual ICollection<EventIncrease> EventIncreases { get; } = new List<EventIncrease>();
}
