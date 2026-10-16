using Microsoft.AspNetCore.Mvc;
using Piping.Api.Application.Contracts;
using Piping.Api.Domain.Entities;

namespace Piping.Api.Api.Controllers;

[ApiController]
[Route("api/cmls/{cmlId:int}/test-points")]
public sealed class TestPointsController(ITestPointRepository testPoints) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int cmlId, int id, CancellationToken cancellationToken)
    {
        var entity = await testPoints.GetWithReadingsAsync(cmlId, id, cancellationToken);
        if (entity is null) return NotFound();
        return Ok(new
        {
            entity.Id,
            entity.CmlId,
            entity.TpNumber,
            entity.TpDescription,
            entity.Note,
            Readings = entity.Readings.OrderByDescending(x => x.InspectionDate)
                .Select(reading => new { reading.Id, reading.InspectionDate, reading.ActualThicknessMm })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(int cmlId, TestPointWrite request, CancellationToken cancellationToken)
    {
        if (!await testPoints.CmlExistsAsync(cmlId, cancellationToken))
            return NotFound(new { message = "ไม่พบ CML" });
        if (await testPoints.NumberExistsAsync(cmlId, request.TpNumber, null, cancellationToken))
            return Conflict(new { message = "TP Number นี้มีอยู่แล้วใน CML นี้" });

        var entity = new TestPoint { CmlId = cmlId, TpNumber = request.TpNumber, TpDescription = request.TpDescription, Note = request.Note?.Trim() };
        await testPoints.AddAsync(entity, cancellationToken);
        await testPoints.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { cmlId, id = entity.Id }, new { entity.Id, entity.TpNumber });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int cmlId, int id, TestPointWrite request, CancellationToken cancellationToken)
    {
        var entity = await testPoints.GetForUpdateAsync(cmlId, id, cancellationToken);
        if (entity is null) return NotFound();
        if (await testPoints.NumberExistsAsync(cmlId, request.TpNumber, id, cancellationToken))
            return Conflict(new { message = "TP Number นี้มีอยู่แล้วใน CML นี้" });

        entity.TpNumber = request.TpNumber;
        entity.TpDescription = request.TpDescription;
        entity.Note = request.Note?.Trim();
        await testPoints.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int cmlId, int id, CancellationToken cancellationToken)
    {
        var entity = await testPoints.GetForDeleteAsync(cmlId, id, cancellationToken);
        if (entity is null) return NotFound();
        if (entity.Readings.Count > 0) return Conflict(new { message = "ลบไม่ได้: Test Point นี้ยังมี Thickness Reading อยู่" });

        testPoints.Remove(entity);
        await testPoints.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
