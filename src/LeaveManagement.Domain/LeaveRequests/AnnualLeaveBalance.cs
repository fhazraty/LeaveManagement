using LeaveManagement.Domain.Common;

namespace LeaveManagement.Domain.LeaveRequests;

/// <summary>مانده مرخصی یک کارمند در یک سال شمسی.</summary>
public sealed class AnnualLeaveBalance
{
    public LeaveYear Year { get; }
    public int AllowanceDays { get; }
    public int UsedDays { get; }
    public int RemainingDays => AllowanceDays - UsedDays;

    private AnnualLeaveBalance(LeaveYear year, int allowanceDays, int usedDays)
    {
        Year = year;
        AllowanceDays = allowanceDays;
        UsedDays = usedDays;
    }

    /// <param name="takenPeriods">بازه درخواست‌های فعال (در انتظار یا تأییدشده) همان کارمند.</param>
    public static AnnualLeaveBalance Calculate(
        LeaveYear year, LeavePolicy policy, IEnumerable<LeavePeriod> takenPeriods)
    {
        var used = takenPeriods.Sum(p => p.DaysWithin(year.Start, year.End));
        return new AnnualLeaveBalance(year, policy.AnnualAllowanceDays, used);
    }

    public void EnsureCovers(LeavePeriod requested)
    {
        var requestedDays = requested.DaysWithin(Year.Start, Year.End);
        if (requestedDays > RemainingDays)
            throw new DomainException(
                $"مانده مرخصی سال {Year.Year} کافی نیست: درخواست {requestedDays} روز، مانده {RemainingDays} روز.");
    }
}
