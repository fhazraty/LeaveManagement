namespace LeaveManagement.Application.LeaveRequests.GetById;

public sealed record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly Start,
    DateOnly End,
    int Days,
    string Status,
    string? RejectionReason);
