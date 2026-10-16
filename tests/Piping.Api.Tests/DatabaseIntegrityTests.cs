using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Piping.Api.Api.Controllers;
using Piping.Api.Application.Contracts;
using Piping.Api.Application.Services;
using Piping.Api.Domain.Entities;
using Piping.Api.Infrastructure.Persistence;

namespace Piping.Api.Tests;

public sealed class DatabaseIntegrityTests
{
    [Fact]
    public async Task ParentDelete_IsRestrictedWhileChildRecordsExist()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PipingDbContext>().UseSqlite(connection).Options;
        await using var db = new PipingDbContext(options);
        await db.Database.EnsureCreatedAsync();
        var line = new PipingLine { LineNumber = "LINE-1", PipeSizeInch = 6m };
        line.Cmls.Add(new Cml { CmlNumber = 1, Description = "Pipe" });
        db.PipingLines.Add(line);
        await db.SaveChangesAsync();
        var repository = new PipingRepository(db);
        var controller = new PipingLinesController(
            repository,
            new PipeCalculationService((IPipeCalculationRepository)repository));
        var result = await controller.Delete(line.Id, CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(result);
        Assert.True(await db.PipingLines.AnyAsync(x => x.Id == line.Id));
    }
}
