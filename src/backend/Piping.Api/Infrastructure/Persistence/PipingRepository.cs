using Microsoft.EntityFrameworkCore;
using Piping.Api.Application.Contracts;
using Piping.Api.Domain.Entities;

namespace Piping.Api.Infrastructure.Persistence;

public sealed class PipingRepository(PipingDbContext db) :
    IPipingLineRepository,
    ICmlRepository,
    ITestPointRepository,
    IThicknessReadingRepository,
    IPipeCalculationRepository
{
    public async Task<IReadOnlyList<PipingLine>> ListAsync(CancellationToken cancellationToken) =>
        await db.PipingLines.AsNoTracking().OrderBy(x => x.LineNumber).ToListAsync(cancellationToken);

    public Task<PipingLine?> GetWithMeasurementsAsync(int id, CancellationToken cancellationToken) =>
        db.PipingLines.AsNoTracking().AsSplitQuery()
            .Include(x => x.Cmls).ThenInclude(x => x.TestPoints).ThenInclude(x => x.Readings)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<PipingLine?> GetWithCmlsAsync(int id, CancellationToken cancellationToken) =>
        db.PipingLines.AsNoTracking().AsSplitQuery().Include(x => x.Cmls)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<PipingLine?> GetForUpdateAsync(int id, CancellationToken cancellationToken) =>
        db.PipingLines.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<PipingLine?> GetForDeleteAsync(int id, CancellationToken cancellationToken) =>
        db.PipingLines.Include(x => x.Cmls).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> LineNumberExistsAsync(string lineNumber, int? excludingId, CancellationToken cancellationToken) =>
        db.PipingLines.AnyAsync(
            x => x.LineNumber == lineNumber && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);

    public Task AddAsync(PipingLine entity, CancellationToken cancellationToken) =>
        db.PipingLines.AddAsync(entity, cancellationToken).AsTask();

    public void Remove(PipingLine entity) => db.PipingLines.Remove(entity);

    public Task<Cml?> GetWithMeasurementsAsync(int lineId, int id, CancellationToken cancellationToken) =>
        db.Cmls.AsNoTracking().AsSplitQuery().Include(x => x.TestPoints).ThenInclude(x => x.Readings)
            .SingleOrDefaultAsync(x => x.Id == id && x.PipingLineId == lineId, cancellationToken);

    public Task<Cml?> GetForUpdateAsync(int lineId, int id, CancellationToken cancellationToken) =>
        db.Cmls.SingleOrDefaultAsync(x => x.Id == id && x.PipingLineId == lineId, cancellationToken);

    public Task<Cml?> GetForDeleteAsync(int lineId, int id, CancellationToken cancellationToken) =>
        db.Cmls.Include(x => x.TestPoints)
            .SingleOrDefaultAsync(x => x.Id == id && x.PipingLineId == lineId, cancellationToken);

    public Task<bool> PipingLineExistsAsync(int lineId, CancellationToken cancellationToken) =>
        db.PipingLines.AnyAsync(x => x.Id == lineId, cancellationToken);

    public Task<bool> NumberExistsAsync(int lineId, int cmlNumber, int? excludingId, CancellationToken cancellationToken) =>
        db.Cmls.AnyAsync(
            x => x.PipingLineId == lineId && x.CmlNumber == cmlNumber && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);

    public Task AddAsync(Cml entity, CancellationToken cancellationToken) =>
        db.Cmls.AddAsync(entity, cancellationToken).AsTask();

    public void Remove(Cml entity) => db.Cmls.Remove(entity);

    public Task<TestPoint?> GetWithReadingsAsync(int cmlId, int id, CancellationToken cancellationToken) =>
        db.TestPoints.AsNoTracking().Include(x => x.Readings)
            .SingleOrDefaultAsync(x => x.Id == id && x.CmlId == cmlId, cancellationToken);

    Task<TestPoint?> ITestPointRepository.GetForUpdateAsync(int cmlId, int id, CancellationToken cancellationToken) =>
        db.TestPoints.SingleOrDefaultAsync(x => x.Id == id && x.CmlId == cmlId, cancellationToken);

    Task<TestPoint?> ITestPointRepository.GetForDeleteAsync(int cmlId, int id, CancellationToken cancellationToken) =>
        db.TestPoints.Include(x => x.Readings)
            .SingleOrDefaultAsync(x => x.Id == id && x.CmlId == cmlId, cancellationToken);

    public Task<bool> CmlExistsAsync(int cmlId, CancellationToken cancellationToken) =>
        db.Cmls.AnyAsync(x => x.Id == cmlId, cancellationToken);

    Task<bool> ITestPointRepository.NumberExistsAsync(int cmlId, int tpNumber, int? excludingId, CancellationToken cancellationToken) =>
        db.TestPoints.AnyAsync(
            x => x.CmlId == cmlId && x.TpNumber == tpNumber && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);

    public Task AddAsync(TestPoint entity, CancellationToken cancellationToken) =>
        db.TestPoints.AddAsync(entity, cancellationToken).AsTask();

    public void Remove(TestPoint entity) => db.TestPoints.Remove(entity);

    public Task<ThicknessReading?> GetAsync(int testPointId, int id, CancellationToken cancellationToken) =>
        db.ThicknessReadings.SingleOrDefaultAsync(
            x => x.Id == id && x.TestPointId == testPointId,
            cancellationToken);

    public Task<bool> TestPointExistsAsync(int testPointId, CancellationToken cancellationToken) =>
        db.TestPoints.AnyAsync(x => x.Id == testPointId, cancellationToken);

    public Task AddAsync(ThicknessReading entity, CancellationToken cancellationToken) =>
        db.ThicknessReadings.AddAsync(entity, cancellationToken).AsTask();

    public void Remove(ThicknessReading entity) => db.ThicknessReadings.Remove(entity);

    public async Task<IReadOnlyList<PipeOdLookup>> GetPipeOdLookupsAsync(CancellationToken cancellationToken) =>
        await db.PipeOdLookups.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StructuralThicknessRule>> GetStructuralThicknessRulesAsync(CancellationToken cancellationToken) =>
        await db.StructuralThicknessRules.AsNoTracking().ToListAsync(cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
