using Piping.Api.Domain.Entities;

namespace Piping.Api.Application.Contracts;

public interface IPersistenceUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IPipingLineRepository : IPersistenceUnitOfWork
{
    Task<IReadOnlyList<PipingLine>> ListAsync(CancellationToken cancellationToken);
    Task<PipingLine?> GetWithMeasurementsAsync(int id, CancellationToken cancellationToken);
    Task<PipingLine?> GetWithCmlsAsync(int id, CancellationToken cancellationToken);
    Task<PipingLine?> GetForUpdateAsync(int id, CancellationToken cancellationToken);
    Task<PipingLine?> GetForDeleteAsync(int id, CancellationToken cancellationToken);
    Task<bool> LineNumberExistsAsync(string lineNumber, int? excludingId, CancellationToken cancellationToken);
    Task AddAsync(PipingLine entity, CancellationToken cancellationToken);
    void Remove(PipingLine entity);
}

public interface ICmlRepository : IPersistenceUnitOfWork
{
    Task<Cml?> GetWithMeasurementsAsync(int lineId, int id, CancellationToken cancellationToken);
    Task<Cml?> GetForUpdateAsync(int lineId, int id, CancellationToken cancellationToken);
    Task<Cml?> GetForDeleteAsync(int lineId, int id, CancellationToken cancellationToken);
    Task<bool> PipingLineExistsAsync(int lineId, CancellationToken cancellationToken);
    Task<bool> NumberExistsAsync(int lineId, int cmlNumber, int? excludingId, CancellationToken cancellationToken);
    Task AddAsync(Cml entity, CancellationToken cancellationToken);
    void Remove(Cml entity);
}

public interface ITestPointRepository : IPersistenceUnitOfWork
{
    Task<TestPoint?> GetWithReadingsAsync(int cmlId, int id, CancellationToken cancellationToken);
    Task<TestPoint?> GetForUpdateAsync(int cmlId, int id, CancellationToken cancellationToken);
    Task<TestPoint?> GetForDeleteAsync(int cmlId, int id, CancellationToken cancellationToken);
    Task<bool> CmlExistsAsync(int cmlId, CancellationToken cancellationToken);
    Task<bool> NumberExistsAsync(int cmlId, int tpNumber, int? excludingId, CancellationToken cancellationToken);
    Task AddAsync(TestPoint entity, CancellationToken cancellationToken);
    void Remove(TestPoint entity);
}

public interface IThicknessReadingRepository : IPersistenceUnitOfWork
{
    Task<ThicknessReading?> GetAsync(int testPointId, int id, CancellationToken cancellationToken);
    Task<bool> TestPointExistsAsync(int testPointId, CancellationToken cancellationToken);
    Task AddAsync(ThicknessReading entity, CancellationToken cancellationToken);
    void Remove(ThicknessReading entity);
}

public interface IPipeCalculationRepository
{
    Task<IReadOnlyList<PipeOdLookup>> GetPipeOdLookupsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<StructuralThicknessRule>> GetStructuralThicknessRulesAsync(CancellationToken cancellationToken);
}

public interface IPipeCalculationService
{
    Task<(decimal? Od, decimal? Structural, double? DesignThickness, double? RequireThickness)> CalculateAsync(
        int id,
        decimal? pipeSize,
        CancellationToken cancellationToken = default);
}
