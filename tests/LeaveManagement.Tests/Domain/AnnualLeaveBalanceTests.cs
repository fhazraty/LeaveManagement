using System.Globalization;
using LeaveManagement.Domain.Common;
using LeaveManagement.Domain.LeaveRequests;
using Xunit;

namespace LeaveManagement.Tests.Domain;

public sealed class AnnualLeaveBalanceTests
{
    private static readonly PersianCalendar Persian = new();
    private static readonly LeavePolicy Policy = new(annualAllowanceDays: 30);

    private static DateOnly Shamsi(int year, int month, int day)
        => DateOnly.FromDateTime(Persian.ToDateTime(year, month, day, 0, 0, 0, 0));

    private static LeavePeriod Period(DateOnly start, int days)
        => LeavePeriod.Create(start, start.AddDays(days - 1));

    [Fact]
    public void LeaveYear_StartsAtFarvardin1_AndEndsAtLastDayOfEsfand()
    {
        var year = LeaveYear.Of(Shamsi(1405, 7, 16));

        Assert.Equal(1405, year.Year);
        Assert.Equal(Shamsi(1405, 1, 1), year.Start);
        Assert.Equal(Shamsi(1406, 1, 1).AddDays(-1), year.End);
    }

    [Fact]
    public void Period_AcrossNowruz_CoversTwoYears_AndSplitsDays()
    {
        var period = Period(Shamsi(1405, 12, 27), 7);
        var years = LeaveYear.Covering(period);

        Assert.Equal(2, years.Count);
        Assert.Equal(1405, years[0].Year);
        Assert.Equal(1406, years[1].Year);
        Assert.Equal(7, period.DaysWithin(years[0].Start, years[0].End) +
                        period.DaysWithin(years[1].Start, years[1].End));
    }

    [Fact]
    public void Balance_CountsOnlyDaysInsideTheYear()
    {
        var year = LeaveYear.Of(Shamsi(1405, 6, 1));
        var crossing = Period(Shamsi(1405, 12, 27), 7);

        var balance = AnnualLeaveBalance.Calculate(year, Policy,
            [Period(Shamsi(1405, 2, 1), 10), crossing]);

        Assert.Equal(10 + crossing.DaysWithin(year.Start, year.End), balance.UsedDays);
        Assert.Equal(30 - balance.UsedDays, balance.RemainingDays);
    }

    [Fact]
    public void EnsureCovers_ExactlyRemaining_IsAllowed()
    {
        var year = LeaveYear.Of(Shamsi(1405, 6, 1));
        var balance = AnnualLeaveBalance.Calculate(year, Policy, [Period(Shamsi(1405, 2, 1), 20)]);

        balance.EnsureCovers(Period(Shamsi(1405, 8, 1), 10));
    }

    [Fact]
    public void EnsureCovers_MoreThanRemaining_IsRejected()
    {
        var year = LeaveYear.Of(Shamsi(1405, 6, 1));
        var balance = AnnualLeaveBalance.Calculate(year, Policy, [Period(Shamsi(1405, 2, 1), 20)]);

        Assert.Throws<DomainException>(() => balance.EnsureCovers(Period(Shamsi(1405, 8, 1), 11)));
    }

    [Fact]
    public void Policy_WithZeroDays_IsInvalid()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new LeavePolicy(0));
}
