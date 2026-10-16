using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Piping.Api.Api;

public sealed class DatabaseConstraintExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException { InnerException: SqliteException { SqliteErrorCode: 19 } })
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "ข้อมูลขัดแย้งกับรายการที่มีอยู่",
                Detail = "คำขอนี้ไม่ผ่านข้อจำกัดของฐานข้อมูล กรุณาตรวจสอบข้อมูลและลองอีกครั้ง."
            }
        });
    }
}
