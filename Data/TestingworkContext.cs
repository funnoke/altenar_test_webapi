
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace altenar_test_webapi.Data;
public class TestingworkContext : IdentityDbContext<Player, IdentityRole<Guid>, Guid>
{
    public TestingworkContext()
    {
		//Database.EnsureCreated();
		//Database.EnsureDeleted();
    }

    public TestingworkContext(DbContextOptions<TestingworkContext> options)
        : base(options)
    {
        
		Database.EnsureCreated();
		//Database.EnsureDeleted();
    }


    public virtual DbSet<Bet> Bets { get; set; }

    public virtual DbSet<SportsEvent> SportsEvents { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public override DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }

    public override DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }

    public override DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-NL3TB6V;Database=testingwork;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bets__3214EC0708EE0C59");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateDateBet).HasColumnType("datetime");

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.Bets)
                .HasForeignKey(d => d.IdEvent)
                .HasConstraintName("FK__Bets__IdEvent__29572725");

            entity.HasOne(d => d.IdPlayerNavigation).WithMany(p => p.Bets)
                .HasForeignKey(d => d.IdPlayer)
                .HasConstraintName("FK__Bets__IdPlayer__286302EC");
        });
    modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
    {
        entity.HasKey(e => e.UserId);
    });
    modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
    {
        entity.HasKey(e => e.RoleId);
    });
    modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
    {
        entity.HasNoKey();
    });
        // modelBuilder.Entity<Player>(entity =>
        // {
        //     entity.HasKey(e => e.Id);

        //     entity.Property(e => e.Id).HasMaxLength(255);
        //     entity.Property(e => e.UserName).HasMaxLength(255);
        //     entity.Property(e => e.PasswordHash).HasMaxLength(255);
        // });

        modelBuilder.Entity<SportsEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SportsEv__3214EC07294371E9");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateEvent).HasColumnType("datetime");
            entity.Property(e => e.NameEvent).HasMaxLength(255);
        });

    }

}
