using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Common;

namespace LeaveManagement.Application.LeaveRequests.GetById;

public sealed record GetLeaveRequestQuery(Guid Id);

public sealed class GetLeaveRequestHandler(ILeaveRequestRepository repository)
{
    public async Task<LeaveRequestDto> HandleAsync(GetLeaveRequestQuery query, CancellationToken ct)
    {
        var r = await repository.GetByIdAsync(query.Id, ct)
            ?? throw new NotFoundException("درخواست مرخصی یافت نشد.");

        return new LeaveRequestDto(r.Id, r.EmployeeId, r.Period.Start, r.Period.End,
            r.Period.Days, r.Status.ToString(), r.RejectionReason);
    }
}
