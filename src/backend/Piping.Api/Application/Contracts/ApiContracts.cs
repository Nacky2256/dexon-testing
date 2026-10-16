using System.ComponentModel.DataAnnotations;

namespace Piping.Api.Application.Contracts;

public sealed class PipingLineWrite
{
    [Required, StringLength(100)] public string LineNumber { get; set; } = "";
    [StringLength(200)] public string? Location { get; set; }
    [StringLength(500)] public string? FromDescription { get; set; }
    [StringLength(500)] public string? ToDescription { get; set; }
    [StringLength(200)] public string? DrawingNumber { get; set; }
    [StringLength(100)] public string? Service { get; set; }
    [StringLength(200)] public string? Material { get; set; }
    public DateOnly? InserviceDate { get; set; }
    [Required, Range(typeof(decimal), "0.001", "1000"), DecimalScale(6)] public decimal? PipeSizeInch { get; set; }
    [Range(typeof(decimal), "0.001", "100000"), DecimalScale(6)] public decimal? OriginalThicknessMm { get; set; }
    [DecimalScale(6)] public decimal? Stress { get; set; }
    [Range(typeof(decimal), "0", "1"), DecimalScale(6)] public decimal? JointEfficiency { get; set; }
    [Range(typeof(decimal), "0", "100000"), DecimalScale(6)] public decimal? CorrosionAllowanceMm { get; set; }
    [Range(typeof(decimal), "0", "100000"), DecimalScale(6)] public decimal? DesignLife { get; set; }
    [DecimalScale(6)] public decimal? DesignPressure { get; set; }
    [DecimalScale(6)] public decimal? OperatingPressure { get; set; }
    [DecimalScale(6)] public decimal? DesignTemperature { get; set; }
    [DecimalScale(6)] public decimal? OperatingTemperature { get; set; }
}

public sealed class CmlWrite
{
    [Range(1, int.MaxValue)] public int CmlNumber { get; set; }
    [Required, StringLength(200)] public string Description { get; set; } = "";
}

public sealed class TestPointWrite
{
    [Range(1, int.MaxValue)] public int TpNumber { get; set; }
    [Range(0, 360)] public int TpDescription { get; set; }
    [StringLength(500)] public string? Note { get; set; }
}

public sealed class ThicknessReadingWrite
{
    [Required] public DateOnly? InspectionDate { get; set; }
    [Range(typeof(decimal), "0.001", "100000"), DecimalScale(6)] public decimal ActualThicknessMm { get; set; }
}

public sealed record CalculationValue(decimal? Value, string Unit, string Status, string? Message = null);
public sealed record CalculationDoubleValue(double? Value, string Unit, string Status, string? Message = null);
