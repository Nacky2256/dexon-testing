using System.ComponentModel.DataAnnotations;
using Piping.Api.Application.Contracts;

namespace Piping.Api.Tests;

public sealed class ValidationTests
{
    [Fact]
    public void PipingLineWrite_RejectsDecimalsBeyondSixPlaces()
    {
        var request = new PipingLineWrite { LineNumber = "TEST", PipeSizeInch = 0.3570001m };
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        Assert.False(valid);
        Assert.Contains(results, result => result.ErrorMessage!.Contains("6 ตำแหน่ง", StringComparison.Ordinal));
    }

    [Fact]
    public void PipingLineWrite_AcceptsSamplePipeSizePrecision()
    {
        var request = new PipingLineWrite { LineNumber = "TEST", PipeSizeInch = 0.357m };
        var results = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true));
    }
}
