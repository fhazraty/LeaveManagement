namespace LeaveManagement.Application.Common;

/// <summary>مستقل از EF Core؛ زیرساخت استثنای خودش را به این ترجمه می‌کند.</summary>
public sealed class ConcurrencyConflictException(string message, Exception? inner = null)
    : Exception(message, inner);
