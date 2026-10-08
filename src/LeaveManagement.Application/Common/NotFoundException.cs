namespace LeaveManagement.Application.Common;

public sealed class NotFoundException(string message) : Exception(message);
