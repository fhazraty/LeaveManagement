using LeaveManagement.Application.Abstractions;
using LeaveManagement.Domain.LeaveRequests;

namespace LeaveManagement.Application.LeaveBalances.GetBalance;

public sealed record GetLeaveBalanceQuery(Guid EmployeeId);

/// <summary>مانده مرخصی کارمند در سال شمسی جاری.</summary>
public sealed class GetLeaveBalanceHandler(
    ILeaveRequestRepository repository,
    TimeProvider clock,
    LeavePolicy policy)
{
    public async Task<LeaveBalanceDto> HandleAsync(GetLeaveBalanceQuery query, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);
        var year = LeaveYear.Of(today);

        var taken = await repository.GetActivePeriodsAsync(query.EmployeeId, year.Start, year.End, ct);
        var balance = AnnualLeaveBalance.Calculate(year, policy, taken);

        return new LeaveBalanceDto(query.EmployeeId, year.Year,
            balance.AllowanceDays, balance.UsedDays, balance.RemainingDays);
    }
}
