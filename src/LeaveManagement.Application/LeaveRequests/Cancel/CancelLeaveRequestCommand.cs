namespace LeaveManagement.Application.LeaveRequests.Cancel;

public sealed record CancelLeaveRequestCommand(Guid LeaveRequestId, Guid EmployeeId);
