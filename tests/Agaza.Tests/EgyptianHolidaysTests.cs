using System;
using System.Linq;
using Xunit;

namespace Agaza.Tests;

public class EgyptianHolidaysTests
{
    [Fact]
    public void ForYear_returns_holidays_sorted_by_date()
    {
        var dates = EgyptianHolidays.ForYear(2026).Select(h => h.Date).ToArray();

        Assert.Equal(dates.OrderBy(d => d), dates);
    }

    [Theory]
    [InlineData(1, 7, "Coptic Christmas")]
    [InlineData(1, 25, "25 January Revolution and Police Day")]
    [InlineData(4, 25, "Sinai Liberation Day")]
    [InlineData(5, 1, "Labour Day")]
    [InlineData(6, 30, "30 June Revolution Day")]
    [InlineData(7, 23, "23 July Revolution Day")]
    [InlineData(10, 6, "Armed Forces Day")]
    public void Fixed_holidays_fall_on_their_dates(int month, int day, string name)
    {
        var holiday = EgyptianHolidays.ForYear(2026)
            .Single(h => h.Date == new DateOnly(2026, month, day));

        Assert.Equal(name, holiday.NameEn);
        Assert.False(holiday.IsEstimate);
    }

    [Theory]
    [InlineData(2025, 4, 20)]
    [InlineData(2026, 4, 12)]
    [InlineData(2027, 5, 2)]
    public void Coptic_Easter_matches_the_computus(int year, int month, int day)
    {
        var easter = EgyptianHolidays.ForYear(year).Single(h => h.NameEn == "Coptic Easter");

        Assert.Equal(new DateOnly(year, month, day), easter.Date);
    }

    [Theory]
    [InlineData(2025)]
    [InlineData(2026)]
    [InlineData(2027)]
    public void Sham_El_Nessim_is_the_Monday_after_Coptic_Easter(int year)
    {
        var holidays = EgyptianHolidays.ForYear(year);
        var easter = holidays.Single(h => h.NameEn == "Coptic Easter");
        var sham = holidays.Single(h => h.NameEn == "Sham El-Nessim");

        Assert.Equal(easter.Date.AddDays(1), sham.Date);
        Assert.Equal(DayOfWeek.Monday, sham.Date.DayOfWeek);
    }

    [Fact]
    public void Islamic_New_Year_1446_is_2024_07_07()
    {
        var holiday = EgyptianHolidays.ForYear(2024).Single(h => h.NameEn == "Islamic New Year");

        Assert.Equal(new DateOnly(2024, 7, 7), holiday.Date);
        Assert.Equal(HolidayKind.Islamic, holiday.Kind);
        Assert.True(holiday.IsEstimate);
    }

    [Fact]
    public void Eid_Al_Fitr_1446_runs_three_days_from_2025_03_30()
    {
        var days = EgyptianHolidays.ForYear(2025)
            .Where(h => h.NameEn.StartsWith("Eid al-Fitr"))
            .Select(h => h.Date)
            .ToArray();

        Assert.Equal(new[]
        {
            new DateOnly(2025, 3, 30),
            new DateOnly(2025, 3, 31),
            new DateOnly(2025, 4, 1),
        }, days);
    }

    [Fact]
    public void Eid_Al_Adha_1446_follows_Arafat_Day()
    {
        var holidays = EgyptianHolidays.ForYear(2025);

        Assert.Equal(new DateOnly(2025, 6, 5), holidays.Single(h => h.NameEn == "Arafat Day").Date);
        Assert.Equal(new DateOnly(2025, 6, 6), holidays.Single(h => h.NameEn == "Eid al-Adha (day 1)").Date);
        Assert.Equal(new DateOnly(2025, 6, 8), holidays.Single(h => h.NameEn == "Eid al-Adha (day 3)").Date);
    }

    [Fact]
    public void Mawlid_1446_is_2024_09_15()
    {
        var mawlid = EgyptianHolidays.ForYear(2024).Single(h => h.NameEn == "Mawlid al-Nabi");

        Assert.Equal(new DateOnly(2024, 9, 15), mawlid.Date);
    }

    [Fact]
    public void A_Gregorian_year_can_hold_two_Islamic_New_Years()
    {
        var newYears = EgyptianHolidays.ForYear(2008)
            .Where(h => h.NameEn == "Islamic New Year")
            .Select(h => h.Date)
            .ToArray();

        Assert.Equal(new[] { new DateOnly(2008, 1, 9), new DateOnly(2008, 12, 28) }, newYears);
    }

    [Fact]
    public void Holidays_carry_bilingual_names()
    {
        var sham = EgyptianHolidays.ForYear(2026).Single(h => h.NameEn == "Sham El-Nessim");

        Assert.Equal("شم النسيم", sham.NameAr);
    }

    [Fact]
    public void Find_returns_the_holiday_on_a_date()
    {
        var holiday = EgyptianHolidays.Find(new DateOnly(2026, 10, 6));

        Assert.NotNull(holiday);
        Assert.Equal("Armed Forces Day", holiday!.NameEn);
    }

    [Fact]
    public void Find_returns_null_on_a_regular_day()
    {
        Assert.Null(EgyptianHolidays.Find(new DateOnly(2026, 2, 10)));
    }

    [Fact]
    public void IsHoliday_matches_Find()
    {
        Assert.True(EgyptianHolidays.IsHoliday(new DateOnly(2026, 4, 13))); // Sham El-Nessim
        Assert.False(EgyptianHolidays.IsHoliday(new DateOnly(2026, 2, 10)));
    }

    [Fact]
    public void Year_outside_the_supported_range_throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => EgyptianHolidays.ForYear(1899));
        Assert.Throws<ArgumentOutOfRangeException>(() => EgyptianHolidays.ForYear(2100));
    }
}
