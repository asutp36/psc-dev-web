using System;
using System.Collections.Generic;

namespace MSO_SyncService.Models.WashCompanyDb;

public partial class MobileSending
{
    public int IdmobileSending { get; set; }

    public int Idcard { get; set; }

    public int Idpost { get; set; }

    public DateTime DtimeStart { get; set; }

    public DateTime? DtimeEnd { get; set; }

    public int? Amount { get; set; }

    public int? StatusCode { get; set; }

    public string? ResultMessage { get; set; }

    public string Guid { get; set; } = null!;

    public virtual Card IdcardNavigation { get; set; } = null!;

    public virtual Post IdpostNavigation { get; set; } = null!;
}
