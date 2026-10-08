namespace LeaveManagement.Domain.LeaveRequests;

/// <summary>سیاست مرخصی سازمان؛ سقف مرخصی هر کارمند در یک سال شمسی.</summary>
public sealed record LeavePolicy
{
    public int AnnualAllowanceDays { get; }

    public LeavePolicy(int annualAllowanceDays)
    {
        if (annualAllowanceDays is < 1 or > 366)
            throw new ArgumentOutOfRangeException(nameof(annualAllowanceDays),
                "سقف مرخصی سالانه باید بین ۱ تا ۳۶۶ روز باشد.");

        AnnualAllowanceDays = annualAllowanceDays;
    }
}
