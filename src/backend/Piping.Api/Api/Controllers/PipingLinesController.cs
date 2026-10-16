using Microsoft.AspNetCore.Mvc;
using Piping.Api.Application.Contracts;
using Piping.Api.Domain.Entities;

namespace Piping.Api.Api.Controllers;

[ApiController]
[Route("api/piping-lines")]
public sealed class PipingLinesController(IPipingLineRepository lines, IPipeCalculationService calculations) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var rows = (await lines.ListAsync(cancellationToken)).Select(x => new
        {
            x.Id,
            x.LineNumber,
            x.Location,
            x.FromDescription,
            x.ToDescription,
            x.PipeSizeInch,
            x.Service,
            x.Material
        });
        return Ok(rows);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var line = await lines.GetWithMeasurementsAsync(id, cancellationToken);
        if (line is null) return NotFound();
        var (od, structural, dt, rt) = await calculations.CalculateAsync(id, line.PipeSizeInch, cancellationToken);

        var cmls = line.Cmls.OrderBy(x => x.CmlNumber).Select(cml => new
        {
            cml.Id,
            cml.CmlNumber,
            cml.Description,
            ActualOutsideDiameterMm = od,
            DesignThicknessMm = dt?.ToString("F3"),
            DesignThicknessStatus = "pending-confirmation",
            StructuralThicknessMm = structural,
            RequiredThicknessMm = dt?.ToString("F3"),
            RequiredThicknessStatus = "pending-confirmation",
            TestPoints = cml.TestPoints.OrderBy(x => x.TpNumber).Select(tp => new
            {
                tp.Id,
                tp.TpNumber,
                tp.TpDescription,
                tp.Note,
                Readings = tp.Readings.OrderByDescending(r => r.InspectionDate)
                    .Select(r => new { r.Id, r.InspectionDate, r.ActualThicknessMm })
            })
        });
        return Ok(new
        {
            line.Id,
            line.LineNumber,
            line.Location,
            line.FromDescription,
            line.ToDescription,
            line.DrawingNumber,
            line.Service,
            line.Material,
            line.InserviceDate,
            line.PipeSizeInch,
            line.OriginalThicknessMm,
            line.Stress,
            line.JointEfficiency,
            line.CorrosionAllowanceMm,
            line.DesignLife,
            line.DesignPressure,
            line.OperatingPressure,
            line.DesignTemperature,
            line.OperatingTemperature,
            Cmls = cmls
        });
    }

    [HttpGet("{id:int}/calculations")]
    public async Task<IActionResult> Calculations(int id, CancellationToken cancellationToken)
    {
        var line = await lines.GetWithCmlsAsync(id, cancellationToken);
        if (line is null) return NotFound();
        var (od, structural, dt, rt) = await calculations.CalculateAsync(id, line.PipeSizeInch, cancellationToken);
        return Ok(new
        {
            line.Id,
            line.PipeSizeInch,
            ActualOutsideDiameter = new CalculationValue(od, "mm", od is null ? "unavailable" : "calculated", od is null ? "Pipe Size ไม่มีในตาราง lookup" : null),
            DesignThickness = new CalculationDoubleValue(dt, "mm", "pending-confirmation", "รอยืนยันสูตรและวิธีปัดเศษ"),
            StructuralThickness = new CalculationValue(structural, "mm", structural is null ? "unavailable" : "calculated", structural is null ? "Pipe Size อยู่นอกช่วงที่ระบุ" : null),
            RequiredThickness = new CalculationDoubleValue(rt, "mm", "pending-confirmation", "รอยืนยันสูตรและวิธีปัดเศษ"),
            CmlNumbers = line.Cmls.Select(x => x.CmlNumber)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(PipingLineWrite request, CancellationToken cancellationToken)
    {
        if (await lines.LineNumberExistsAsync(request.LineNumber, null, cancellationToken))
            return Conflict(new { message = "Line Number นี้มีอยู่แล้ว" });
        var entity = new PipingLine(); Apply(entity, request);
        await lines.AddAsync(entity, cancellationToken);
        await lines.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, new { entity.Id, entity.LineNumber });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PipingLineWrite request, CancellationToken cancellationToken)
    {
        var entity = await lines.GetForUpdateAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        if (await lines.LineNumberExistsAsync(request.LineNumber, id, cancellationToken))
            return Conflict(new { message = "Line Number นี้มีอยู่แล้ว" });
        Apply(entity, request); await lines.SaveChangesAsync(cancellationToken); return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await lines.GetForDeleteAsync(id, cancellationToken);
        if (entity is null) return NotFound();
        if (entity.Cmls.Count > 0) return Conflict(new { message = "ลบไม่ได้: Line Number นี้ยังมี CML อยู่" });
        lines.Remove(entity); await lines.SaveChangesAsync(cancellationToken); return NoContent();
    }

    private static void Apply(PipingLine entity, PipingLineWrite request)
    {
        entity.LineNumber = request.LineNumber.Trim();
        entity.Location = request.Location?.Trim(); entity.FromDescription = request.FromDescription?.Trim();
        entity.ToDescription = request.ToDescription?.Trim(); entity.DrawingNumber = request.DrawingNumber?.Trim();
        entity.Service = request.Service?.Trim(); entity.Material = request.Material?.Trim();
        entity.InserviceDate = request.InserviceDate; entity.PipeSizeInch = request.PipeSizeInch;
        entity.OriginalThicknessMm = request.OriginalThicknessMm; entity.Stress = request.Stress;
        entity.JointEfficiency = request.JointEfficiency; entity.CorrosionAllowanceMm = request.CorrosionAllowanceMm;
        entity.DesignLife = request.DesignLife; entity.DesignPressure = request.DesignPressure;
        entity.OperatingPressure = request.OperatingPressure; entity.DesignTemperature = request.DesignTemperature;
        entity.OperatingTemperature = request.OperatingTemperature;
    }
}
