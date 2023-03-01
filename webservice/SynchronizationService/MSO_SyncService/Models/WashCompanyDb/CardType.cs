using System;
using System.Collections.Generic;

namespace MSO.SyncService.Models.WashCompanyDb;

public partial class CardType
{
    public int IdcardType { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Card> Cards { get; } = new List<Card>();
}
