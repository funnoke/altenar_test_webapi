using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace altenar_test_webapi.Data;

public partial class Player : IdentityUser<Guid>
{
    [Column("Id")]
    [Key]
    public override Guid Id { get; set; }

    [Column("LoginPl")]
    public override string? UserName { get; set; }
    [Column("PasswordPl")]
    public override string? PasswordHash { get; set; }

    public double? Balance { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
    [NotMapped]
     public override int AccessFailedCount { get; set; }
    [NotMapped]
     public override bool LockoutEnabled { get; set; }
    
      public override string? NormalizedUserName { get; set; }
    [NotMapped]
      public override string? Email { get; set; }
    [NotMapped]
      public override string? NormalizedEmail { get; set; }
    [NotMapped]
      public override bool EmailConfirmed { get; set; }
    [NotMapped]
       public override string? SecurityStamp { get; set; }
    [NotMapped]
       public override string? ConcurrencyStamp { get; set; }
    [NotMapped]
       public override string? PhoneNumber { get; set; }
    [NotMapped]
       public override bool PhoneNumberConfirmed { get; set; }
    [NotMapped]
       public override bool TwoFactorEnabled { get; set; }
    [NotMapped]
        public override DateTimeOffset? LockoutEnd { get; set; }
}
