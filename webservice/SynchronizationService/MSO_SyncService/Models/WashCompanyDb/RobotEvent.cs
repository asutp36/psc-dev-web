using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class RobotEvent
{
    public int IdrobotEvent { get; set; }

    public int Idpost { get; set; }

    public int IdeventKind { get; set; }

    public DateTime Dtime { get; set; }

    public int IdeventPost { get; set; }

    public int IdrobotSession { get; set; }

    public virtual EventKind IdeventKindNavigation { get; set; } = null!;

    public virtual Post IdpostNavigation { get; set; } = null!;

    public virtual RobotEventIncrease? RobotEventIncrease { get; set; }

    public virtual RobotEventPayout? RobotEventPayout { get; set; }
}
