using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class RobotEventIncrease
{
    public int IdrobotEvent { get; set; }

    public int? Amount { get; set; }

    public int? M10 { get; set; }

    public int? B50 { get; set; }

    public int? B100 { get; set; }

    public int? B200 { get; set; }

    public int? B500 { get; set; }

    public int? B1000 { get; set; }

    public int? B2000 { get; set; }

    public int? Balance { get; set; }

    public virtual RobotEvent IdrobotEventNavigation { get; set; } = null!;
}
