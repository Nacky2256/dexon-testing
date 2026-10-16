using Microsoft.EntityFrameworkCore;
using Piping.Api.Domain.Entities;

namespace Piping.Api.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task SeedReferenceDataAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PipingDbContext>();
        var existingSizes = (await db.PipeOdLookups.AsNoTracking().Select(x => x.PipeSizeInch).ToListAsync()).ToHashSet();
        var odRows = new (decimal Size, decimal Od)[]
        {
            (0.125m,10.3m),(0.25m,13.7m),(0.357m,17.1m),(0.5m,21.3m),(0.75m,26.7m),
            (1m,33.4m),(1.25m,42.2m),(1.5m,48.3m),(2m,60.3m),(2.5m,73m),(3m,88.9m),
            (3.5m,101.6m),(4m,114.3m),(5m,141.3m),(6m,168.3m),(8m,219.1m),(10m,273m),
            (12m,323.8m),(14m,355.6m),(16m,406.4m),(18m,457m)
        };
        db.PipeOdLookups.AddRange(odRows.Where(x => !existingSizes.Contains(x.Size))
            .Select(x => new PipeOdLookup { PipeSizeInch = x.Size, ActualOdMm = x.Od }));

        var existingRules = (await db.StructuralThicknessRules.AsNoTracking().Select(x => x.RuleLabel).ToListAsync()).ToHashSet(StringComparer.Ordinal);
        var rules = new[]
        {
            new StructuralThicknessRule { MinPipeSizeInch = 0m, MaxPipeSizeInch = 2m, StructuralThicknessMm = 1.8m, RuleLabel = "<= 2 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 3m, MaxPipeSizeInch = 3m, StructuralThicknessMm = 2m, RuleLabel = "3 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 4m, MaxPipeSizeInch = 4m, StructuralThicknessMm = 2.3m, RuleLabel = "4 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 6m, MaxPipeSizeInch = 18m, StructuralThicknessMm = 2.8m, RuleLabel = "6 to 18 inch" },
            new StructuralThicknessRule { MinPipeSizeInch = 20m, MaxPipeSizeInch = null, StructuralThicknessMm = 3.1m, RuleLabel = ">= 20 inch" }
        };
        db.StructuralThicknessRules.AddRange(rules.Where(x => !existingRules.Contains(x.RuleLabel)));
        await db.SaveChangesAsync();
    }
}
