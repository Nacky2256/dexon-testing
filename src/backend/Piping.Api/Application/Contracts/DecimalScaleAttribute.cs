using System.ComponentModel.DataAnnotations;

namespace Piping.Api.Application.Contracts;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class DecimalScaleAttribute(int maximumScale) : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not decimal number) return false;
        var scale = (decimal.GetBits(number)[3] >> 16) & 0x7F;
        return scale <= maximumScale;
    }

    public override string FormatErrorMessage(string name) => $"{name} รองรับทศนิยมได้ไม่เกิน {maximumScale} ตำแหน่ง";
}
