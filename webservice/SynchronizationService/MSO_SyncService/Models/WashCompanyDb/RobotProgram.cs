using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class RobotProgram
{
    public int IdrobotProgram { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Cost { get; set; }

    public int DisplayOrder { get; set; }

    public int? Button { get; set; }

    public string? AltName { get; set; }

    public virtual ICollection<RobotSession> RobotSessions { get; } = new List<RobotSession>();
}
