namespace LeaveManagement.Application.LeaveRequests.Reject;

public sealed record RejectLeaveRequestCommand(Guid LeaveRequestId, Guid ManagerId, string Reason);
