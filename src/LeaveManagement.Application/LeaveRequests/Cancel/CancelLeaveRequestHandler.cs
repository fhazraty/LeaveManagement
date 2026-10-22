using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Common;

namespace LeaveManagement.Application.LeaveRequests.Cancel;

public sealed class CancelLeaveRequestHandler(
    ILeaveRequestRepository repository,
    IUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task HandleAsync(CancelLeaveRequestCommand command, CancellationToken ct)
    {
        var request = await repository.GetByIdAsync(command.LeaveRequestId, ct)
            ?? throw new NotFoundException("درخواست مرخصی یافت نشد.");

        request.Cancel(command.EmployeeId, clock.GetUtcNow());

        await unitOfWork.SaveChangesAsync(ct);
    }
}
