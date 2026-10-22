using LeaveManagement.Application.LeaveBalances.GetBalance;
using LeaveManagement.Application.LeaveRequests.Approve;
using LeaveManagement.Application.LeaveRequests.Cancel;
using LeaveManagement.Application.LeaveRequests.GetById;
using LeaveManagement.Application.LeaveRequests.Reject;
using LeaveManagement.Application.LeaveRequests.Submit;
using LeaveManagement.Domain.LeaveRequests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LeaveManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, LeavePolicy policy)
    {
        services.AddSingleton(policy);
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<SubmitLeaveRequestHandler>();
        services.AddScoped<ApproveLeaveRequestHandler>();
        services.AddScoped<RejectLeaveRequestHandler>();
        services.AddScoped<CancelLeaveRequestHandler>();
        services.AddScoped<GetLeaveRequestHandler>();
        services.AddScoped<GetLeaveBalanceHandler>();
        return services;
    }
}
