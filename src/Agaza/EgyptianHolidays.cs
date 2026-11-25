using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Agaza;

/// <summary>
/// Egyptian public holidays. Fixed national days, Coptic Easter and
/// Sham El-Nessim by computus, and Islamic holidays from the tabular Hijri
/// calendar (<see cref="HijriCalendar"/>, civil scheme). Supported range:
/// 1900-2099, the years where the Julian-to-Gregorian 13-day shift holds.
/// </summary>
public static class EgyptianHolidays
{
    private static readonly HijriCalendar Hijri = new();

    private static readonly (int Month, int Day, string En, string Ar)[] Fixed =
    {
        (1, 25, "25 January Revolution and Police Day", "ثورة 25 يناير وعيد الشرطة"),
        (4, 25, "Sinai Liberation Day", "عيد تحرير سيناء"),
        (5, 1, "Labour Day", "عيد العمال"),
        (6, 30, "30 June Revolution Day", "ثورة 30 يونيو"),
        (7, 23, "23 July Revolution Day", "ثورة 23 يوليو"),
        (10, 6, "Armed Forces Day", "عيد القوات المسلحة"),
    };

    private static readonly (int Month, int Day, int Days, string En, string Ar)[] Islamic =
    {
        (1, 1, 1, "Islamic New Year", "رأس السنة الهجرية"),
        (3, 12, 1, "Mawlid al-Nabi", "المولد النبوي الشريف"),
        (10, 1, 3, "Eid al-Fitr", "عيد الفطر المبارك"),
        (12, 9, 1, "Arafat Day", "يوم عرفة"),
        (12, 10, 3, "Eid al-Adha", "عيد الأضحى المبارك"),
    };

    /// <summary>
    /// All public holidays of a Gregorian year, sorted by date. A holiday can
    /// appear twice in one Gregorian year when the shorter Hijri year wraps
    /// around (for example, two Islamic New Years in 2008).
    /// </summary>
    public static IReadOnlyList<Holiday> ForYear(int year)
    {
        if (year < CopticCalendar.MinYear || year > CopticCalendar.MaxYear)
            throw new ArgumentOutOfRangeException(nameof(year), year,
                $"Supported range is {CopticCalendar.MinYear}-{CopticCalendar.MaxYear}.");

        var holidays = new List<Holiday>();

        foreach (var (month, day, en, ar) in Fixed)
            holidays.Add(new Holiday(new DateOnly(year, month, day), en, ar, HolidayKind.National, IsEstimate: false));

        holidays.Add(new Holiday(new DateOnly(year, 1, 7), "Coptic Christmas", "عيد الميلاد المجيد", HolidayKind.Coptic, IsEstimate: false));

        var easter = CopticCalendar.Easter(year);
        holidays.Add(new Holiday(easter, "Coptic Easter", "عيد القيامة المجيد", HolidayKind.Coptic, IsEstimate: false));
        holidays.Add(new Holiday(easter.AddDays(1), "Sham El-Nessim", "شم النسيم", HolidayKind.Coptic, IsEstimate: false));

        // The Hijri year is ~11 days shorter than the Gregorian year, so a
        // Gregorian year overlaps two (sometimes three at the edges) Hijri
        // years; any Hijri-based holiday can land twice in one Gregorian year.
        var hyGuess = (year - 622) * 33 / 32;
        for (var hy = hyGuess - 1; hy <= hyGuess + 1; hy++)
        {
            foreach (var (month, day, days, en, ar) in Islamic)
            {
                for (var offset = 0; offset < days; offset++)
                {
                    var date = ToGregorian(hy, month, day + offset);
                    if (date.Year != year)
                        continue;
                    var nameEn = days > 1 ? $"{en} (day {offset + 1})" : en;
                    holidays.Add(new Holiday(date, nameEn, ar, HolidayKind.Islamic, IsEstimate: true));
                }
            }
        }

        return holidays.OrderBy(h => h.Date).ToArray();
    }

    /// <summary>True when the date is an Egyptian public holiday.</summary>
    public static bool IsHoliday(DateOnly date) => Find(date) is not null;

    /// <summary>The holiday on a date, or null when it is a regular day.</summary>
    public static Holiday? Find(DateOnly date) =>
        ForYear(date.Year).FirstOrDefault(h => h.Date == date);

    private static DateOnly ToGregorian(int hy, int hm, int hd)
    {
        var dt = Hijri.ToDateTime(hy, hm, hd, 0, 0, 0, 0);
        return DateOnly.FromDateTime(dt);
    }
}
