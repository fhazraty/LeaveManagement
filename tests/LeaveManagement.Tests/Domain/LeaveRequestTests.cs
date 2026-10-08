using LeaveManagement.Domain.Common;
using LeaveManagement.Domain.LeaveRequests;
using Xunit;

namespace LeaveManagement.Tests.Domain;

public sealed class LeaveRequestTests
{
    private static readonly DateOnly Today = new(2026, 10, 1);
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 9, 0, 0, TimeSpan.Zero);
    private static readonly Guid Employee = Guid.NewGuid();
    private static readonly Guid Manager = Guid.NewGuid();

    private static LeaveRequest NewRequest() => LeaveRequest.Submit(
        Employee, LeavePeriod.Create(Today.AddDays(1), Today.AddDays(3)), "سفر خانوادگی", Today);

    [Fact]
    public void Submit_CreatesPendingRequest()
    {
        var request = NewRequest();

        Assert.Equal(LeaveStatus.Pending, request.Status);
        Assert.Equal(3, request.Period.Days);
    }

    [Fact]
    public void Submit_StartInThePast_IsRejected()
        => Assert.Throws<DomainException>(() => LeaveRequest.Submit(
            Employee, LeavePeriod.Create(Today.AddDays(-1), Today), "سفر", Today));

    [Fact]
    public void Period_LongerThanMax_IsRejected()
        => Assert.Throws<DomainException>(() =>
            LeavePeriod.Create(Today, Today.AddDays(LeavePeriod.MaxDays)));

    [Fact]
    public void Period_EndBeforeStart_IsRejected()
        => Assert.Throws<DomainException>(() => LeavePeriod.Create(Today.AddDays(2), Today));

    [Fact]
    public void Approve_PendingRequest_RecordsDecision()
    {
        var request = NewRequest();

        request.Approve(Manager, Now);

        Assert.Equal(LeaveStatus.Approved, request.Status);
        Assert.Equal(Manager, request.DecidedBy);
        Assert.Equal(Now, request.DecidedAt);
    }

    [Fact]
    public void Approve_Twice_IsRejected()
    {
        var request = NewRequest();
        request.Approve(Manager, Now);

        Assert.Throws<DomainException>(() => request.Approve(Manager, Now));
    }

    [Fact]
    public void Employee_CannotDecide_OwnRequest()
        => Assert.Throws<DomainException>(() => NewRequest().Approve(Employee, Now));

    [Fact]
    public void Reject_WithoutReason_IsRejected()
        => Assert.Throws<DomainException>(() => NewRequest().Reject(Manager, " ", Now));
}
