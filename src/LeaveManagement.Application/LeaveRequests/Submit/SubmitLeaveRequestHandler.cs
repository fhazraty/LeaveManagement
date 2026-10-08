using LeaveManagement.Application.Abstractions;
using LeaveManagement.Domain.Common;
using LeaveManagement.Domain.LeaveRequests;

namespace LeaveManagement.Application.LeaveRequests.Submit;

public sealed class SubmitLeaveRequestHandler(
    ILeaveRequestRepository repository,
    IUnitOfWork unitOfWork,
    TimeProvider clock,
    LeavePolicy policy)
{
    public async Task<Guid> HandleAsync(SubmitLeaveRequestCommand command, CancellationToken ct)
    {
        var period = LeavePeriod.Create(command.Start, command.End);

        if (await repository.HasOverlapAsync(command.EmployeeId, period, ct))
            throw new DomainException("این بازه با مرخصی دیگری از همین کارمند هم‌پوشانی دارد.");

        // مانده مرخصی برای هر سال شمسی که بازه با آن اشتراک دارد، جداگانه بررسی می‌شود.
        foreach (var year in LeaveYear.Covering(period))
        {
            var taken = await repository.GetActivePeriodsAsync(command.EmployeeId, year.Start, year.End, ct);
            AnnualLeaveBalance.Calculate(year, policy, taken).EnsureCovers(period);
        }

        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);
        var request = LeaveRequest.Submit(command.EmployeeId, period, command.Reason, today);

        repository.Add(request);
        await unitOfWork.SaveChangesAsync(ct);
        return request.Id;
    }
}
