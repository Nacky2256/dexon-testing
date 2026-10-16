using Microsoft.AspNetCore.Mvc;
using Piping.Api.Application.Contracts;
using Piping.Api.Domain.Entities;

namespace Piping.Api.Api.Controllers;

[ApiController]
[Route("api/piping-lines/{lineId:int}/cmls")]
public sealed class CmlsController(ICmlRepository cmls) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int lineId, int id, CancellationToken cancellationToken)
    {
        var cml = await cmls.GetWithMeasurementsAsync(lineId, id, cancellationToken);
        if (cml is null) return NotFound();
        return Ok(new
        {
            cml.Id,
            cml.PipingLineId,
            cml.CmlNumber,
            cml.Description,
            TestPoints = cml.TestPoints.OrderBy(x => x.TpNumber).Select(point => new
            {
                point.Id,
                point.CmlId,
                point.TpNumber,
                point.TpDescription,
                point.Note,
                Readings = point.Readings.OrderByDescending(x => x.InspectionDate)
                    .Select(reading => new { reading.Id, reading.InspectionDate, reading.ActualThicknessMm })
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(int lineId, CmlWrite request, CancellationToken cancellationToken)
    {
        if (!await cmls.PipingLineExistsAsync(lineId, cancellationToken))
            return NotFound(new { message = "ไม่พบ Piping Line" });
        if (await cmls.NumberExistsAsync(lineId, request.CmlNumber, null, cancellationToken))
            return Conflict(new { message = "CML Number นี้มีอยู่แล้วใน Line Number นี้" });

        var entity = new Cml { PipingLineId = lineId, CmlNumber = request.CmlNumber, Description = request.Description.Trim() };
        await cmls.AddAsync(entity, cancellationToken);
        await cmls.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { lineId, id = entity.Id }, new { entity.Id, entity.CmlNumber });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int lineId, int id, CmlWrite request, CancellationToken cancellationToken)
    {
        var entity = await cmls.GetForUpdateAsync(lineId, id, cancellationToken);
        if (entity is null) return NotFound();
        if (await cmls.NumberExistsAsync(lineId, request.CmlNumber, id, cancellationToken))
            return Conflict(new { message = "CML Number นี้มีอยู่แล้วใน Line Number นี้" });

        entity.CmlNumber = request.CmlNumber;
        entity.Description = request.Description.Trim();
        await cmls.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int lineId, int id, CancellationToken cancellationToken)
    {
        var entity = await cmls.GetForDeleteAsync(lineId, id, cancellationToken);
        if (entity is null) return NotFound();
        if (entity.TestPoints.Count > 0) return Conflict(new { message = "ลบไม่ได้: CML นี้ยังมี Test Point อยู่" });

        cmls.Remove(entity);
        await cmls.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
