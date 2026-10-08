using System.Reflection;
using LeaveManagement.Application.LeaveRequests.Submit;
using LeaveManagement.Domain.LeaveRequests;
using Xunit;

namespace LeaveManagement.Tests.Architecture;

/// <summary>قاعده وابستگی را با آزمون نگه می‌دارد؛ بدون بسته اضافه.</summary>
public sealed class LayerDependencyTests
{
    private static string[] ReferencesOf(Assembly assembly)
        => assembly.GetReferencedAssemblies().Select(a => a.Name!).ToArray();

    [Fact]
    public void Domain_DependsOnNothing()
    {
        var refs = ReferencesOf(typeof(LeaveRequest).Assembly);

        Assert.DoesNotContain(refs, n => n.StartsWith("LeaveManagement."));
        Assert.DoesNotContain(refs, n => n.StartsWith("Microsoft."));
    }

    [Fact]
    public void Application_DoesNotDependOn_Infrastructure_Or_EfCore()
    {
        var refs = ReferencesOf(typeof(SubmitLeaveRequestHandler).Assembly);

        Assert.DoesNotContain("LeaveManagement.Infrastructure", refs);
        Assert.DoesNotContain(refs, n => n.StartsWith("Microsoft.EntityFrameworkCore"));
        Assert.DoesNotContain(refs, n => n.StartsWith("Microsoft.AspNetCore"));
    }
}
