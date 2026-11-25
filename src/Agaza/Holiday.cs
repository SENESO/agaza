using System;

namespace Agaza;

/// <summary>An Egyptian public holiday on a concrete Gregorian date.</summary>
/// <param name="Date">Gregorian date of the holiday.</param>
/// <param name="NameEn">English name.</param>
/// <param name="NameAr">Arabic name.</param>
/// <param name="Kind">How the date is determined.</param>
/// <param name="IsEstimate">
/// True for Hijri-based holidays: the date comes from the tabular calendar,
/// while official observance follows moon sighting and can shift by a day or two.
/// </param>
public sealed record Holiday(
    DateOnly Date,
    string NameEn,
    string NameAr,
    HolidayKind Kind,
    bool IsEstimate);
