using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Common;

namespace LeaveManagement.Application.LeaveRequests.Approve;

public sealed class ApproveLeaveRequestHandler(
    ILeaveRequestRepository repository,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task HandleAsync(ApproveLeaveRequestCommand command, CancellationToken ct)
    {
        var request = await repository.GetByIdAsync(command.LeaveRequestId, ct)
            ?? throw new NotFoundException("درخواست مرخصی یافت نشد.");

        request.Approve(command.ManagerId, clock.GetUtcNow());

        await unitOfWork.SaveChangesAsync(ct);
    }
}
