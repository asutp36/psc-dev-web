using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

/// <summary>
/// Типы событий
/// </summary>
public partial class EventKind
{
    public int IdeventKind { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> Events { get; } = new List<Event>();

    public virtual ICollection<RobotEvent> RobotEvents { get; } = new List<RobotEvent>();
}
