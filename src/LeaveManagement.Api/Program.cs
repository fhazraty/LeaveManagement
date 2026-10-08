using LeaveManagement.Api.Endpoints;
using LeaveManagement.Api.Errors;
using LeaveManagement.Application;
using LeaveManagement.Domain.LeaveRequests;
using LeaveManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Composition Root: تنها جایی که همه لایه‌ها به هم وصل می‌شوند
var annualAllowanceDays = builder.Configuration.GetValue("LeavePolicy:AnnualAllowanceDays", 30);
builder.Services.AddApplication(new LeavePolicy(annualAllowanceDays));
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("Leave")
    ?? throw new InvalidOperationException("Connection string 'Leave' is missing."));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapLeaveRequestEndpoints();

app.Run();
