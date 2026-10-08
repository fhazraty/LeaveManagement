using System.Globalization;

namespace LeaveManagement.Domain.LeaveRequests;

/// <summary>سال شمسی برای محاسبه مانده مرخصی؛ از اول فروردین تا آخر اسفند.</summary>
public sealed record LeaveYear
{
    private static readonly PersianCalendar Calendar = new();

    public int Year { get; }
    public DateOnly Start { get; }
    public DateOnly End { get; }

    private LeaveYear(int year)
    {
        Year = year;
        Start = FirstDayOf(year);
        End = FirstDayOf(year + 1).AddDays(-1);
    }

    public static LeaveYear Of(DateOnly date)
        => new(Calendar.GetYear(date.ToDateTime(TimeOnly.MinValue)));

    /// <summary>سال‌هایی که بازه مرخصی با آن‌ها اشتراک دارد؛ مثلاً یک مرخصی از اسفند تا فروردین دو سال را پوشش می‌دهد.</summary>
    public static IReadOnlyList<LeaveYear> Covering(LeavePeriod period)
    {
        var first = Of(period.Start).Year;
        var last = Of(period.End).Year;
        return Enumerable.Range(first, last - first + 1).Select(y => new LeaveYear(y)).ToList();
    }

    private static DateOnly FirstDayOf(int year)
        => DateOnly.FromDateTime(Calendar.ToDateTime(year, 1, 1, 0, 0, 0, 0));
}
