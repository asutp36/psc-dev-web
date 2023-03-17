using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class RobotSession
{
    public int IdrobotSession { get; set; }

    public int Idpost { get; set; }

    public DateTime Dtime { get; set; }

    public int IdrobotProgram { get; set; }

    public int IdsessionPost { get; set; }

    public DateTime? StartDtime { get; set; }

    public string? Qr { get; set; }

    public string? FiscalError { get; set; }

    public DateTime? StopDtime { get; set; }

    public int? AmountCash { get; set; }

    public int? AmountBank { get; set; }

    public virtual RobotProgram IdrobotProgramNavigation { get; set; } = null!;
}
