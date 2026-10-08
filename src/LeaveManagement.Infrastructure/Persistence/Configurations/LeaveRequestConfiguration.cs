using LeaveManagement.Domain.LeaveRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

internal sealed class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.ComplexProperty(x => x.Period, period =>
        {
            period.Property(p => p.Start).HasColumnName("StartDate");
            period.Property(p => p.End).HasColumnName("EndDate");
        });

        builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();
        builder.Property(x => x.RejectionReason).HasMaxLength(500);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        // توکن هم‌زمانی فقط در زیرساخت است؛ Domain از آن خبر ندارد.
        builder.Property<byte[]>("RowVersion").IsRowVersion();

        builder.HasIndex(x => new { x.EmployeeId, x.Status });
    }
}
