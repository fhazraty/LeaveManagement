using LeaveManagement.Application.Abstractions;
using LeaveManagement.Domain.LeaveRequests;

namespace LeaveManagement.Tests.Fakes;

internal sealed class FakeLeaveRequestRepository : ILeaveRequestRepository
{
    public List<LeaveRequest> Items { get; } = [];

    public Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

    public Task<bool> HasOverlapAsync(Guid employeeId, LeavePeriod period, CancellationToken ct)
        => Task.FromResult(Items.Any(x =>
            x.EmployeeId == employeeId &&
            x.Status != LeaveStatus.Rejected &&
            x.Period.Overlaps(period)));

    public Task<IReadOnlyList<LeavePeriod>> GetActivePeriodsAsync(
        Guid employeeId, DateOnly from, DateOnly to, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<LeavePeriod>>(Items
            .Where(x => x.EmployeeId == employeeId &&
                        x.Status != LeaveStatus.Rejected &&
                        x.Period.Start <= to && from <= x.Period.End)
            .Select(x => x.Period)
            .ToList());

    public void Add(LeaveRequest request) => Items.Add(request);
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        SaveCount++;
        return Task.CompletedTask;
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
