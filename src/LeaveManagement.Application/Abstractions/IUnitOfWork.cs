namespace LeaveManagement.Application.Abstractions;

public interface IUnitOfWork
{
    /// <exception cref="Common.ConcurrencyConflictException">تغییر هم‌زمان رکورد.</exception>
    Task SaveChangesAsync(CancellationToken ct);
}
