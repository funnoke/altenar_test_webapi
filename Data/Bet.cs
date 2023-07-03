﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace altenar_test_webapi.Data;

public partial class Bet
{
    public Guid Id { get; set; }

    public Guid? IdPlayer { get; set; }

    public Guid? IdEvent { get; set; }

    public DateTime? CreateDataBet { get; set; }

    public int? CoeffType { get; set; }

    public int? Coeff { get; set; }

    public double? BetAmount { get; set; }

    public virtual SportsEvent? IdEventNavigation { get; set; }

    public virtual Player? IdPlayerNavigation { get; set; }
}
