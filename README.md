# LeaveManagement — نمونه آموزشی Clean Architecture در C#

نمونه کوچک و کامل همراه ارائه «معماری تمیز در C#». سناریو: کارمند درخواست مرخصی ثبت می‌کند و مدیر آن را تأیید یا رد می‌کند.

## قواعد کسب‌وکار

1. هر درخواست حداکثر ۳۰ روز و پایان بعد از شروع (`LeavePeriod`)
2. شروع در گذشته نباشد و علت الزامی است (`LeaveRequest.Submit`)
3. بدون هم‌پوشانی با درخواست فعال دیگرِ همان کارمند (`SubmitLeaveRequestHandler`)
4. فقط درخواست در انتظار قابل تصمیم است و هیچ‌کس درباره مرخصی خودش تصمیم نمی‌گیرد (`LeaveRequest.Approve/Reject`)
5. رد فقط با ذکر دلیل (`LeaveRequest.Reject`)
6. مجموع مرخصی هر سال شمسی (فروردین تا اسفند) از سقف سالانه بیشتر نشود؛ درخواست‌های در انتظار و تأییدشده حساب می‌شوند و مرخصی‌ای که از نوروز می‌گذرد بین دو سال تقسیم می‌شود (`AnnualLeaveBalance`، `LeaveYear`)

## ساختار

| پروژه | نقش | ارجاع پروژه | بسته NuGet |
|---|---|---|---|
| `LeaveManagement.Domain` | موجودیت‌ها، Value Object و قواعد | — | — |
| `LeaveManagement.Application` | Use Caseها و رابط‌های مورد نیاز | Domain | DependencyInjection.Abstractions |
| `LeaveManagement.Infrastructure` | EF Core، Repository، UnitOfWork | Application | EntityFrameworkCore.SqlServer |
| `LeaveManagement.Api` | Minimal API و Composition Root | Application، Infrastructure | EntityFrameworkCore.Design (فقط برای Migration) |
| `LeaveManagement.Tests` | آزمون Domain، Use Case و معماری | Domain، Application | xunit |

## اجرا

```powershell
dotnet build
dotnet test

# ساخت پایگاه داده (یک بار)
dotnet tool install --global dotnet-ef
dotnet ef migrations add Initial -p src/LeaveManagement.Infrastructure -s src/LeaveManagement.Api
dotnet ef database update -p src/LeaveManagement.Infrastructure -s src/LeaveManagement.Api

dotnet run --project src/LeaveManagement.Api
```

رشته اتصال در `src/LeaveManagement.Api/appsettings.json` با نام `Leave` است و سقف مرخصی سالانه با `LeavePolicy:AnnualAllowanceDays` (پیش‌فرض ۳۰) تنظیم می‌شود.

## نمونه درخواست‌ها

```http
POST /api/leave-requests
{ "employeeId": "11111111-1111-1111-1111-111111111111", "start": "2026-10-20", "end": "2026-10-22", "reason": "trip" }

POST /api/leave-requests/{id}/approve
{ "managerId": "22222222-2222-2222-2222-222222222222" }

POST /api/leave-requests/{id}/reject
{ "managerId": "22222222-2222-2222-2222-222222222222", "reason": "..." }

GET /api/leave-requests/{id}

GET /api/employees/{employeeId}/leave-balance
→ { "year": 1405, "allowanceDays": 30, "usedDays": 10, "remainingDays": 20 }
```

خطاها با قالب ProblemDetails برمی‌گردند: نقض قاعده (از جمله کمبود مانده) ۴۰۰، یافت‌نشدن ۴۰۴، تعارض هم‌زمانی ۴۰۹.

> برای سادگی، شناسه کارمند و مدیر از بدنه درخواست خوانده می‌شود. در سامانه واقعی باید از کاربر احراز هویت‌شده خوانده شود.

## تمرین

قابلیت «لغو درخواست» را اضافه کنید: وضعیت `Cancelled`، متد `Cancel` روی `LeaveRequest`، Use Case و Endpoint جدید، و آزمون‌ها. درخواست لغوشده نباید در هم‌پوشانی و مانده سالانه حساب شود. آزمون‌های معماری باید سبز بمانند.
