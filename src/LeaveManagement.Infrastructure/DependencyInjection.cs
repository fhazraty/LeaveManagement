using LeaveManagement.Application.Abstractions;
using LeaveManagement.Infrastructure.Persistence;
using LeaveManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<LeaveDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
