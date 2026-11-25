using Agaza;

var year = args.Length > 0 && int.TryParse(args[0], out var parsed) ? parsed : DateTime.Today.Year;

Console.WriteLine($"Egyptian public holidays, {year}  (أجازات مصر الرسمية)");
Console.WriteLine();
Console.WriteLine($"{"Date",-12}{"Weekday",-11}{"Holiday",-38}{"بالعربي",-30}Note");
Console.WriteLine(new string('-', 104));

foreach (var h in EgyptianHolidays.ForYear(year))
{
    var note = h.IsEstimate ? "~ moon sighting" : "";
    Console.WriteLine($"{h.Date:yyyy-MM-dd}  {h.Date.DayOfWeek,-11}{h.NameEn,-38}{h.NameAr,-30}{note}");
}
