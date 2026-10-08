using LeaveManagement.Application.LeaveRequests.Approve;
using LeaveManagement.Application.LeaveRequests.Submit;
using LeaveManagement.Application.Common;
using LeaveManagement.Domain.Common;
using LeaveManagement.Domain.LeaveRequests;
using LeaveManagement.Tests.Fakes;
using Xunit;

namespace LeaveManagement.Tests.Application;

public sealed class SubmitLeaveRequestHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 9, 0, 0, TimeSpan.Zero);
    private static readonly Guid Employee = Guid.NewGuid();

    private readonly FakeLeaveRequestRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    private SubmitLeaveRequestHandler CreateHandler(int annualAllowanceDays = 30)
        => new(_repository, _unitOfWork, new FixedTimeProvider(Now), new LeavePolicy(annualAllowanceDays));

    private static SubmitLeaveRequestCommand Command(int fromDay, int toDay) => new(
        Employee, new DateOnly(2026, 10, fromDay), new DateOnly(2026, 10, toDay), "سفر");

    [Fact]
    public async Task Handle_ValidCommand_SavesPendingRequest()
    {
        var id = await CreateHandler().HandleAsync(Command(5, 7), CancellationToken.None);

        var saved = Assert.Single(_repository.Items);
        Assert.Equal(id, saved.Id);
        Assert.Equal(LeaveStatus.Pending, saved.Status);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Handle_OverlappingPeriod_IsRejected_AndNothingIsSaved()
    {
        await CreateHandler().HandleAsync(Command(5, 7), CancellationToken.None);

        await Assert.ThrowsAsync<DomainException>(
            () => CreateHandler().HandleAsync(Command(7, 9), CancellationToken.None));

        Assert.Single(_repository.Items);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Handle_ExactlyRemainingBalance_IsAccepted()
    {
        await CreateHandler(annualAllowanceDays: 10).HandleAsync(Command(5, 11), CancellationToken.None); // 7 days

        await CreateHandler(annualAllowanceDays: 10).HandleAsync(Command(20, 22), CancellationToken.None); // 3 days

        Assert.Equal(2, _repository.Items.Count);
    }

    [Fact]
    public async Task Handle_MoreThanRemainingBalance_IsRejected_AndNothingIsSaved()
    {
        await CreateHandler(annualAllowanceDays: 10).HandleAsync(Command(5, 11), CancellationToken.None); // 7 days

        var error = await Assert.ThrowsAsync<DomainException>(
            () => CreateHandler(annualAllowanceDays: 10).HandleAsync(Command(20, 23), CancellationToken.None)); // 4 days

        Assert.Contains("مانده", error.Message);
        Assert.Single(_repository.Items);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Handle_RejectedRequests_DoNotConsumeBalance()
    {
        await CreateHandler(annualAllowanceDays: 10).HandleAsync(Command(5, 14), CancellationToken.None); // 10 days
        _repository.Items[0].Reject(Guid.NewGuid(), "no", Now);

        await CreateHandler(annualAllowanceDays: 10).HandleAsync(Command(20, 29), CancellationToken.None); // 10 days

        Assert.Equal(2, _repository.Items.Count);
    }

    [Fact]
    public async Task Approve_UnknownRequest_ThrowsNotFound()
    {
        var handler = new ApproveLeaveRequestHandler(_repository, _unitOfWork, new FixedTimeProvider(Now));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(
            new ApproveLeaveRequestCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }
}
