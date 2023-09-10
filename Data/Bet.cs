using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace altenar_test_webapi.Data;

public partial class Bet
{
    public Guid Id { get; set; }

    public Guid? IdPlayer { get; set; }

    public Guid? IdEvent { get; set; }

    public DateTime? CreateDateBet { get; set; }
//  0 - 1я команда победит
//  1 - ничья
//  2 - 2я команда победит
    public int? CoeffType { get; set; }

//  Коэффициент на момент создания ставки
    public double? Coeff { get; set; }

//  Количество поставленных денег
    public double? BetAmount { get; set; }

    public virtual SportsEvent? IdEventNavigation { get; set; }

    public virtual Player? IdPlayerNavigation { get; set; }
}
