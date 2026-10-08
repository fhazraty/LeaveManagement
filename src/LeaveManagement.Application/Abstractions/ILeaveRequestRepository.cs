using LeaveManagement.Domain.LeaveRequests;

namespace LeaveManagement.Application.Abstractions;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> HasOverlapAsync(Guid employeeId, LeavePeriod period, CancellationToken ct);

    /// <summary>بازه درخواست‌های در انتظار یا تأییدشده کارمند که با بازه داده‌شده اشتراک دارند.</summary>
    Task<IReadOnlyList<LeavePeriod>> GetActivePeriodsAsync(
        Guid employeeId, DateOnly from, DateOnly to, CancellationToken ct);

    void Add(LeaveRequest request);
}
