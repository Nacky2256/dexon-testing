using Piping.Api.Application.Contracts;

namespace Piping.Api.Application.Services;

public sealed class PipeCalculationService(IPipeCalculationRepository repository, IPipingLineRepository lines) : IPipeCalculationService
{
    public async Task<(decimal? Od, decimal? Structural, double? DesignThickness, double? RequireThickness)> CalculateAsync(int id, decimal? pipeSize, CancellationToken cancellationToken = default)
    {
        if (pipeSize is null) return (null, null, null, null);
        var size = pipeSize.Value;

        // Materialize before comparing to keep exact decimal comparisons out of SQLite SQL translation.
        var odRows = await repository.GetPipeOdLookupsAsync(cancellationToken);
        var rules = await repository.GetStructuralThicknessRulesAsync(cancellationToken);
        var od = odRows.SingleOrDefault(x => x.PipeSizeInch == size)?.ActualOdMm;
        var structural = rules.SingleOrDefault(x => x.MinPipeSizeInch <= size &&
            (x.MaxPipeSizeInch is null || x.MaxPipeSizeInch >= size))?.StructuralThicknessMm;


        var line = await lines.GetWithMeasurementsAsync(id, cancellationToken);

        double? dt = null;    

        if(line != null)
        {
            dt = CalculateDesignThickness(line.DesignPressure, od, line.Stress, line.JointEfficiency);   
        }

        var rt = Math.Max(dt ?? 0, (double)(structural ?? 0));

        return (od, structural, dt, rt);
    }

    private double? CalculateDesignThickness(decimal? design_pressure, decimal? od, decimal? stress, decimal? joint)
    {
        var dp = design_pressure ?? 1;
        var sts = stress ?? 1;
        var jnt = joint ?? 1;

        var designThickness = (double)((dp * od) ?? 0)/((double)(2*sts*jnt) + (2 * (double)dp * 0.4));
        return designThickness;
    }
}
