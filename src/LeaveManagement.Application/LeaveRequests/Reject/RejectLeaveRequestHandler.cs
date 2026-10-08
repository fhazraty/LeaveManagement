using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Common;

namespace LeaveManagement.Application.LeaveRequests.Reject;

public sealed class RejectLeaveRequestHandler(
    ILeaveRequestRepository repository,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task HandleAsync(RejectLeaveRequestCommand command, CancellationToken ct)
    {
        var request = await repository.GetByIdAsync(command.LeaveRequestId, ct)
            ?? throw new NotFoundException("درخواست مرخصی یافت نشد.");

        request.Reject(command.ManagerId, command.Reason, clock.GetUtcNow());

        await unitOfWork.SaveChangesAsync(ct);
    }
}
