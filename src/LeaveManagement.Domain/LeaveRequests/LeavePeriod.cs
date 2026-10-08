using LeaveManagement.Domain.Common;

namespace LeaveManagement.Domain.LeaveRequests;

/// <summary>بازه مرخصی؛ Value Object تغییرناپذیر و همیشه معتبر.</summary>
public sealed record LeavePeriod
{
    public const int MaxDays = 30;

    public DateOnly Start { get; }
    public DateOnly End { get; }
    public int Days => End.DayNumber - Start.DayNumber + 1;

    private LeavePeriod(DateOnly start, DateOnly end)
    {
        Start = start;
        End = end;
    }

    public static LeavePeriod Create(DateOnly start, DateOnly end)
    {
        if (end < start)
            throw new DomainException("تاریخ پایان نباید قبل از تاریخ شروع باشد.");

        var days = end.DayNumber - start.DayNumber + 1;
        if (days > MaxDays)
            throw new DomainException($"مدت مرخصی حداکثر {MaxDays} روز است.");

        return new LeavePeriod(start, end);
    }

    public bool Overlaps(LeavePeriod other) => Start <= other.End && other.Start <= End;

    /// <summary>تعداد روزهایی از این بازه که داخل بازه داده‌شده قرار می‌گیرد.</summary>
    public int DaysWithin(DateOnly from, DateOnly to)
    {
        var start = Start > from ? Start : from;
        var end = End < to ? End : to;
        return end < start ? 0 : end.DayNumber - start.DayNumber + 1;
    }
}
