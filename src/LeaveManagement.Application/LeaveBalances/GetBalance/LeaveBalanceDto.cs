namespace LeaveManagement.Application.LeaveBalances.GetBalance;

public sealed record LeaveBalanceDto(
    Guid EmployeeId,
    int Year,
    int AllowanceDays,
    int UsedDays,
    int RemainingDays);
