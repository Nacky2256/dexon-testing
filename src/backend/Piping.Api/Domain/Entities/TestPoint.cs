using System.ComponentModel.DataAnnotations;

namespace Piping.Api.Domain.Entities;

public sealed class TestPoint
{
    public int Id { get; set; }
    public int CmlId { get; set; }
    public Cml? Cml { get; set; }
    public int TpNumber { get; set; }
    public int TpDescription { get; set; }
    [MaxLength(500)] public string? Note { get; set; }
    public List<ThicknessReading> Readings { get; set; } = [];
}
