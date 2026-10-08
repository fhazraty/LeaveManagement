namespace LeaveManagement.Application.LeaveRequests.Approve;

public sealed record ApproveLeaveRequestCommand(Guid LeaveRequestId, Guid ManagerId);
