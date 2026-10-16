using System.ComponentModel.DataAnnotations;

namespace Piping.Api.Domain.Entities;

public sealed class Cml
{
    public int Id { get; set; }
    public int PipingLineId { get; set; }
    public PipingLine? PipingLine { get; set; }
    public int CmlNumber { get; set; }
    [MaxLength(200)] public string Description { get; set; } = "";
    public List<TestPoint> TestPoints { get; set; } = [];
}
