using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Persistence;

internal sealed class UnitOfWork(LeaveDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // ترجمه استثنای EF به استثنای مستقل لایه Application
            throw new ConcurrencyConflictException(
                "درخواست هم‌زمان توسط کاربر دیگری تغییر کرده است.", ex);
        }
    }
}
