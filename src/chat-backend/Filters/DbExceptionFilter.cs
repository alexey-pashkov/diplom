using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class DbExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateException || context.Exception is DbUpdateConcurrencyException)
        {
            context.Result = new StatusCodeResult(500);

            context.ExceptionHandled = true;
        }

    }
}