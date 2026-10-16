namespace Piping.Api.Domain.Entities;

public sealed class ThicknessReading
{
    public int Id { get; set; }
    public int TestPointId { get; set; }
    public TestPoint? TestPoint { get; set; }
    public DateOnly InspectionDate { get; set; }
    public decimal ActualThicknessMm { get; set; }
}
