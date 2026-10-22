using LeaveManagement.Application.Abstractions;
using LeaveManagement.Domain.LeaveRequests;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Persistence.Repositories;

internal sealed class LeaveRequestRepository(LeaveDbContext db) : ILeaveRequestRepository
{
    public Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken ct)
        => db.LeaveRequests.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> HasOverlapAsync(Guid employeeId, LeavePeriod period, CancellationToken ct)
        => db.LeaveRequests.AnyAsync(x =>
            x.EmployeeId == employeeId &&
            x.Status != LeaveStatus.Rejected &&
            x.Status != LeaveStatus.Canceled &&
            x.Period.Start <= period.End &&
            period.Start <= x.Period.End, ct);

    public async Task<IReadOnlyList<LeavePeriod>> GetActivePeriodsAsync(
        Guid employeeId, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var rows = await db.LeaveRequests
            .Where(x => x.EmployeeId == employeeId &&
                        x.Status != LeaveStatus.Rejected &&
                        x.Period.Start <= to &&
                        from <= x.Period.End)
            .Select(x => new { x.Period.Start, x.Period.End })
            .ToListAsync(ct);

        return rows.Select(r => LeavePeriod.Create(r.Start, r.End)).ToList();
    }

    public void Add(LeaveRequest request) => db.LeaveRequests.Add(request);
}
