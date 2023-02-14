using System;
using System.Collections.Generic;

namespace MSO_SyncService.Models.WashCompanyDb;

public partial class Card
{
    public int Idcard { get; set; }

    public int Idowner { get; set; }

    public string CardNum { get; set; } = null!;

    public int IdcardStatus { get; set; }

    public int IdcardType { get; set; }

    public int? Balance { get; set; }

    public int LocalizedBy { get; set; }

    public int LocalizedId { get; set; }

    public virtual ICollection<MobileSending> MobileSendings { get; } = new List<MobileSending>();
}
