using Microsoft.EntityFrameworkCore;
using Piping.Api.Domain.Entities;

namespace Piping.Api.Infrastructure.Persistence;

public sealed class PipingDbContext(DbContextOptions<PipingDbContext> options) : DbContext(options)
{
    public DbSet<PipingLine> PipingLines => Set<PipingLine>();
    public DbSet<Cml> Cmls => Set<Cml>();
    public DbSet<TestPoint> TestPoints => Set<TestPoint>();
    public DbSet<ThicknessReading> ThicknessReadings => Set<ThicknessReading>();
    public DbSet<PipeOdLookup> PipeOdLookups => Set<PipeOdLookup>();
    public DbSet<StructuralThicknessRule> StructuralThicknessRules => Set<StructuralThicknessRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PipingLine>().HasIndex(x => x.LineNumber).IsUnique();
        modelBuilder.Entity<PipingLine>().Property(x => x.LineNumber).IsRequired();
        modelBuilder.Entity<Cml>().HasIndex(x => new { x.PipingLineId, x.CmlNumber }).IsUnique();
        modelBuilder.Entity<TestPoint>().HasIndex(x => new { x.CmlId, x.TpNumber }).IsUnique();
        modelBuilder.Entity<ThicknessReading>().HasIndex(x => new { x.TestPointId, x.InspectionDate });
        modelBuilder.Entity<StructuralThicknessRule>().HasIndex(x => x.RuleLabel).IsUnique();
        modelBuilder.Entity<PipeOdLookup>().HasKey(x => x.PipeSizeInch);

        // SQLite stores these exact values as TEXT. Decimal comparisons are intentionally performed in memory,
        // because SQLite does not provide fixed-precision decimal arithmetic.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties().Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                property.SetColumnType("TEXT");

        modelBuilder.Entity<PipingLine>().HasMany(x => x.Cmls).WithOne(x => x.PipingLine)
            .HasForeignKey(x => x.PipingLineId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Cml>().HasMany(x => x.TestPoints).WithOne(x => x.Cml)
            .HasForeignKey(x => x.CmlId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TestPoint>().HasMany(x => x.Readings).WithOne(x => x.TestPoint)
            .HasForeignKey(x => x.TestPointId).OnDelete(DeleteBehavior.Restrict);
    }
}
