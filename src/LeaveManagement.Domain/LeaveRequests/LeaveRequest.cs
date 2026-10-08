using LeaveManagement.Domain.Common;

namespace LeaveManagement.Domain.LeaveRequests;

/// <summary>درخواست مرخصی؛ Aggregate Root که گردش کار خودش را تضمین می‌کند.</summary>
public sealed class LeaveRequest : Entity
{
    public Guid EmployeeId { get; private set; }
    public LeavePeriod Period { get; private set; } = null!;
    public string Reason { get; private set; } = string.Empty;
    public LeaveStatus Status { get; private set; }
    public Guid? DecidedBy { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    private LeaveRequest() { } // EF Core

    public static LeaveRequest Submit(Guid employeeId, LeavePeriod period, string reason, DateOnly today)
    {
        if (employeeId == Guid.Empty)
            throw new DomainException("کارمند مشخص نیست.");
        if (period.Start < today)
            throw new DomainException("شروع مرخصی نمی‌تواند در گذشته باشد.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("ذکر علت مرخصی الزامی است.");

        return new LeaveRequest
        {
            EmployeeId = employeeId,
            Period = period,
            Reason = reason.Trim(),
            Status = LeaveStatus.Pending
        };
    }

    public void Approve(Guid managerId, DateTimeOffset now)
    {
        EnsureCanBeDecidedBy(managerId);
        Status = LeaveStatus.Approved;
        DecidedBy = managerId;
        DecidedAt = now;
    }

    public void Reject(Guid managerId, string reason, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("ذکر دلیل رد الزامی است.");

        EnsureCanBeDecidedBy(managerId);
        Status = LeaveStatus.Rejected;
        DecidedBy = managerId;
        DecidedAt = now;
        RejectionReason = reason.Trim();
    }

    private void EnsureCanBeDecidedBy(Guid managerId)
    {
        if (Status != LeaveStatus.Pending)
            throw new DomainException("فقط درخواست در انتظار بررسی قابل تصمیم‌گیری است.");
        if (managerId == EmployeeId)
            throw new DomainException("هیچ‌کس نمی‌تواند درباره مرخصی خودش تصمیم بگیرد.");
    }
}
