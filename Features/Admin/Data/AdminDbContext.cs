using GalacticaBotAPI.Features.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace GalacticaBotAPI.Features.Admin.Data;

public sealed class AdminDbContext(DbContextOptions<AdminDbContext> options) : DbContext(options)
{
    public DbSet<AdminUser> AdminUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.Property(e => e.Username).HasMaxLength(32);

            entity.Property(e => e.Email).HasMaxLength(320);

            entity.Property(e => e.AvatarHash).HasMaxLength(32);

            entity.Property(e => e.Role).HasConversion<int>();

            entity.Property(e => e.IsProfileComplete).HasDefaultValue(false);
        });
    }
}
