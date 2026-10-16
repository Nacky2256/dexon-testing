using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Piping.Api.Application.Contracts;
using Piping.Api.Application.Services;
using Piping.Api.Domain.Entities;
using Piping.Api.Infrastructure.Persistence;

namespace Piping.Api.Tests;

public sealed class PipeCalculationServiceTests
{
    [Fact]
    public async Task CalculateAsync_ReturnsLookupValuesForSupportedPipeSize()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PipingDbContext>().UseSqlite(connection).Options;
        await using var db = new PipingDbContext(options);
        await db.Database.EnsureCreatedAsync();
        db.PipeOdLookups.AddRange(
            new PipeOdLookup { PipeSizeInch = 6m, ActualOdMm = 168.3m },
            new PipeOdLookup { PipeSizeInch = 0.357m, ActualOdMm = 17.1m });
        db.StructuralThicknessRules.AddRange(
            new StructuralThicknessRule { MinPipeSizeInch = 6m, MaxPipeSizeInch = 18m, StructuralThicknessMm = 2.8m, RuleLabel = "6 to 18 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 0m, MaxPipeSizeInch = 2m, StructuralThicknessMm = 1.8m, RuleLabel = "<= 2 inch" });
        await db.SaveChangesAsync();

        var repository = (IPipeCalculationRepository)new PipingRepository(db);
        var result = await new PipeCalculationService(repository).CalculateAsync(6m);

        Assert.Equal(168.3m, result.Od);
        Assert.Equal(2.8m, result.Structural);
        var fractional = await new PipeCalculationService(repository).CalculateAsync(0.357m);
        Assert.Equal(17.1m, fractional.Od);
        Assert.Equal(1.8m, fractional.Structural);
    }

    [Fact]
    public async Task CalculateAsync_MatchesEveryStructuralCalcRangeAndLeavesGapsUnavailable()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PipingDbContext>().UseSqlite(connection).Options;
        await using var db = new PipingDbContext(options);
        await db.Database.EnsureCreatedAsync();
        db.StructuralThicknessRules.AddRange(
            new StructuralThicknessRule { MinPipeSizeInch = 0m, MaxPipeSizeInch = 2m, StructuralThicknessMm = 1.8m, RuleLabel = "<= 2 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 3m, MaxPipeSizeInch = 3m, StructuralThicknessMm = 2m, RuleLabel = "3 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 4m, MaxPipeSizeInch = 4m, StructuralThicknessMm = 2.3m, RuleLabel = "4 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 6m, MaxPipeSizeInch = 18m, StructuralThicknessMm = 2.8m, RuleLabel = "6 to 18 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 20m, MaxPipeSizeInch = null, StructuralThicknessMm = 3.1m, RuleLabel = ">= 20 inch" });
        await db.SaveChangesAsync();
        var service = new PipeCalculationService((IPipeCalculationRepository)new PipingRepository(db));

        foreach (var (size, expected) in new (decimal Size, decimal Expected)[]
        {
            (0.125m, 1.8m), (2m, 1.8m), (3m, 2m), (4m, 2.3m),
            (6m, 2.8m), (18m, 2.8m), (20m, 3.1m), (24m, 3.1m)
        })
        {
            var result = await service.CalculateAsync(size);
            Assert.Equal(expected, result.Structural);
        }

        foreach (var size in new[] { 2.5m, 3.5m, 5m, 19m })
        {
            var result = await service.CalculateAsync(size);
            Assert.Null(result.Structural);
        }
    }

    [Fact]
    public async Task CalculateAsync_ReturnsUnavailableValuesOutsideDefinedTables()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PipingDbContext>().UseSqlite(connection).Options;
        await using var db = new PipingDbContext(options);
        await db.Database.EnsureCreatedAsync();
        db.StructuralThicknessRules.Add(new StructuralThicknessRule
        { MinPipeSizeInch = 6m, MaxPipeSizeInch = 18m, StructuralThicknessMm = 2.8m, RuleLabel = "6 to 18 inch" });
        await db.SaveChangesAsync();

        var result = await new PipeCalculationService(
            (IPipeCalculationRepository)new PipingRepository(db)).CalculateAsync(5m);

        Assert.Null(result.Od);
        Assert.Null(result.Structural);
    }

    [Fact]
    public async Task PipingLine_PreservesDecimalPrecisionInSqlite()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PipingDbContext>().UseSqlite(connection).Options;
        await using var db = new PipingDbContext(options);
        await db.Database.EnsureCreatedAsync();
        db.PipingLines.Add(new PipingLine { LineNumber = "6-PL-TEST", PipeSizeInch = 0.357m, OriginalThicknessMm = 7.1m });
        await db.SaveChangesAsync();

        var stored = await db.PipingLines.AsNoTracking().SingleAsync();

        Assert.Equal(0.357m, stored.PipeSizeInch);
        Assert.Equal(7.1m, stored.OriginalThicknessMm);
    }
}
