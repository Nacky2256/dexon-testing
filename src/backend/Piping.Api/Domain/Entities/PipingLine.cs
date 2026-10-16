using System.ComponentModel.DataAnnotations;

namespace Piping.Api.Domain.Entities;

public sealed class PipingLine
{
    public int Id { get; set; }
    [MaxLength(100)] public string LineNumber { get; set; } = "";
    [MaxLength(200)] public string? Location { get; set; }
    [MaxLength(500)] public string? FromDescription { get; set; }
    [MaxLength(500)] public string? ToDescription { get; set; }
    [MaxLength(200)] public string? DrawingNumber { get; set; }
    [MaxLength(100)] public string? Service { get; set; }
    [MaxLength(200)] public string? Material { get; set; }
    public DateOnly? InserviceDate { get; set; }
    public decimal? PipeSizeInch { get; set; }
    public decimal? OriginalThicknessMm { get; set; }
    public decimal? Stress { get; set; }
    public decimal? JointEfficiency { get; set; }
    public decimal? CorrosionAllowanceMm { get; set; }
    public decimal? DesignLife { get; set; }
    public decimal? DesignPressure { get; set; }
    public decimal? OperatingPressure { get; set; }
    public decimal? DesignTemperature { get; set; }
    public decimal? OperatingTemperature { get; set; }
    public List<Cml> Cmls { get; set; } = [];
}
