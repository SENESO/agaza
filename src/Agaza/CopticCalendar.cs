using System;

namespace Agaza;

/// <summary>
/// Coptic Easter by the Julian computus (Meeus), shifted 13 days into the
/// Gregorian calendar. The 13-day shift is valid from 1900 through 2099.
/// </summary>
internal static class CopticCalendar
{
    internal const int MinYear = 1900;
    internal const int MaxYear = 2099;

    public static DateOnly Easter(int year)
    {
        var a = year % 4;
        var b = year % 7;
        var c = year % 19;
        var d = (19 * c + 15) % 30;
        var e = (2 * a + 4 * b - d + 34) % 7;
        var month = (d + e + 114) / 31;
        var day = ((d + e + 114) % 31) + 1;
        return new DateOnly(year, month, day).AddDays(13);
    }
}
