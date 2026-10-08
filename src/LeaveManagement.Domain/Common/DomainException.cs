namespace LeaveManagement.Domain.Common;

/// <summary>نقض یک قاعده کسب‌وکار.</summary>
public sealed class DomainException(string message) : Exception(message);
