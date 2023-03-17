using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class RobotEventPayout
{
    public int IdrobotEvent { get; set; }

    public int? Amount { get; set; }

    public int? B50 { get; set; }

    public int? B100 { get; set; }

    public int? Balance { get; set; }

    public int? StorageB50 { get; set; }

    public int? StorageB100 { get; set; }

    public virtual RobotEvent IdrobotEventNavigation { get; set; } = null!;
}
