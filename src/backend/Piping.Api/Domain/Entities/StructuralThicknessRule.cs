using System.ComponentModel.DataAnnotations;

namespace Piping.Api.Domain.Entities;

public sealed class StructuralThicknessRule
{
    public int Id { get; set; }
    public decimal MinPipeSizeInch { get; set; }
    public decimal? MaxPipeSizeInch { get; set; }
    public decimal StructuralThicknessMm { get; set; }
    [MaxLength(100)] public string RuleLabel { get; set; } = "";
}
