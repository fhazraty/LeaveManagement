namespace LeaveManagement.Application.LeaveRequests.Submit;

public sealed record SubmitLeaveRequestCommand(
    Guid EmployeeId,
    DateOnly Start,
    DateOnly End,
    string Reason);
