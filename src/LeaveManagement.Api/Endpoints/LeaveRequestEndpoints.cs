using LeaveManagement.Application.LeaveBalances.GetBalance;
using LeaveManagement.Application.LeaveRequests.Approve;
using LeaveManagement.Application.LeaveRequests.GetById;
using LeaveManagement.Application.LeaveRequests.Reject;
using LeaveManagement.Application.LeaveRequests.Submit;

namespace LeaveManagement.Api.Endpoints;

public static class LeaveRequestEndpoints
{
    public static IEndpointRouteBuilder MapLeaveRequestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/leave-requests");

        group.MapPost("/", async (SubmitLeaveRequestCommand command,
            SubmitLeaveRequestHandler handler, CancellationToken ct) =>
        {
            var id = await handler.HandleAsync(command, ct);
            return Results.Created($"/api/leave-requests/{id}", new { id });
        });

        group.MapGet("/{id:guid}", async (Guid id,
            GetLeaveRequestHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.HandleAsync(new GetLeaveRequestQuery(id), ct)));

        group.MapPost("/{id:guid}/approve", async (Guid id, DecisionBody body,
            ApproveLeaveRequestHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new ApproveLeaveRequestCommand(id, body.ManagerId), ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/reject", async (Guid id, DecisionBody body,
            RejectLeaveRequestHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new RejectLeaveRequestCommand(id, body.ManagerId, body.Reason ?? ""), ct);
            return Results.NoContent();
        });

        app.MapGet("/api/employees/{employeeId:guid}/leave-balance", async (Guid employeeId,
            GetLeaveBalanceHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.HandleAsync(new GetLeaveBalanceQuery(employeeId), ct)));

        return app;
    }

    // برای سادگی نمونه؛ در سامانه واقعی شناسه مدیر از کاربر احراز هویت‌شده خوانده می‌شود.
    public sealed record DecisionBody(Guid ManagerId, string? Reason);
}
