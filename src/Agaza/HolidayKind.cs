namespace Agaza;

/// <summary>How a holiday's date is determined.</summary>
public enum HolidayKind
{
    /// <summary>Fixed Gregorian date (national days).</summary>
    National,

    /// <summary>Fixed in the Coptic calendar or computed from Coptic Easter.</summary>
    Coptic,

    /// <summary>Computed from the tabular Hijri calendar.</summary>
    Islamic,
}
