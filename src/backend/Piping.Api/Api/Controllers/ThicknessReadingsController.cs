using Microsoft.AspNetCore.Mvc;
using Piping.Api.Application.Contracts;
using Piping.Api.Domain.Entities;

namespace Piping.Api.Api.Controllers;

[ApiController]
[Route("api/test-points/{testPointId:int}/readings")]
public sealed class ThicknessReadingsController(IThicknessReadingRepository readings) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int testPointId, int id, CancellationToken cancellationToken)
    {
        var entity = await readings.GetAsync(testPointId, id, cancellationToken);
        return entity is null ? NotFound() : Ok(new
        {
            entity.Id,
            entity.TestPointId,
            entity.InspectionDate,
            entity.ActualThicknessMm
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(int testPointId, ThicknessReadingWrite request, CancellationToken cancellationToken)
    {
        if (!await readings.TestPointExistsAsync(testPointId, cancellationToken))
            return NotFound(new { message = "ไม่พบ Test Point" });

        var entity = new ThicknessReading
        {
            TestPointId = testPointId,
            InspectionDate = request.InspectionDate!.Value,
            ActualThicknessMm = request.ActualThicknessMm
        };
        await readings.AddAsync(entity, cancellationToken);
        await readings.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { testPointId, id = entity.Id }, new { entity.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int testPointId, int id, ThicknessReadingWrite request, CancellationToken cancellationToken)
    {
        var entity = await readings.GetAsync(testPointId, id, cancellationToken);
        if (entity is null) return NotFound();
        entity.InspectionDate = request.InspectionDate!.Value;
        entity.ActualThicknessMm = request.ActualThicknessMm;
        await readings.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int testPointId, int id, CancellationToken cancellationToken)
    {
        var entity = await readings.GetAsync(testPointId, id, cancellationToken);
        if (entity is null) return NotFound();
        readings.Remove(entity);
        await readings.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
