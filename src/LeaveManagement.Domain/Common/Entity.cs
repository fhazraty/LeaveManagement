namespace LeaveManagement.Domain.Common;

/// <summary>پایه همه موجودیت‌ها: هویت یکتا.</summary>
public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
}
