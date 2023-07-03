using System;
using System.Collections.Generic;

namespace altenar_test_webapi.Data;

public partial class SportsEvent
{
    public Guid Id { get; set; }

    public DateTime? DateEvent { get; set; }

    public string? NameEvent { get; set; }

    public double? CoeffFirstTeam { get; set; }

    public double? CoeffDraw { get; set; }

    public double? CoeffSecondTeam { get; set; }

    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
