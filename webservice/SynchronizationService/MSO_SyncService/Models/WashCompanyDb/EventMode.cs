using System;
using System.Collections.Generic;

namespace MSO_SyncService.Models.WashCompanyDb;

public partial class EventMode
{
    public int Idevent { get; set; }

    public int? Idmode { get; set; }

    public DateTime? DtimeStart { get; set; }

    public DateTime? DtimeFinish { get; set; }

    public int? Duration { get; set; }

    public int? PaymentSign { get; set; }

    public decimal? Cost { get; set; }

    public string? CardTypeCode { get; set; }

    public string? CardNum { get; set; }

    public int? Discount { get; set; }

    public virtual Event IdeventNavigation { get; set; } = null!;

    public virtual Mode? IdmodeNavigation { get; set; }
}
