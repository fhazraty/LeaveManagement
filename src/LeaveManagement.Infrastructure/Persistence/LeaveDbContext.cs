using LeaveManagement.Domain.LeaveRequests;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Persistence;

public sealed class LeaveDbContext(DbContextOptions<LeaveDbContext> options) : DbContext(options)
{
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeaveDbContext).Assembly);
}
