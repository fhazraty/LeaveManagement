using LeaveManagement.Application.Common;
using LeaveManagement.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace LeaveManagement.Api.Errors;

/// <summary>استثناهای شناخته‌شده را به پاسخ استاندارد ProblemDetails تبدیل می‌کند.</summary>
internal sealed class ApiExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var status = exception switch
        {
            DomainException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ConcurrencyConflictException => StatusCodes.Status409Conflict,
            _ => 0
        };

        if (status == 0)
            return false; // خطای ناشناخته: پاسخ عمومی ۵۰۰ بدون افشای جزئیات

        context.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = { Status = status, Detail = exception.Message }
        });
    }
}
