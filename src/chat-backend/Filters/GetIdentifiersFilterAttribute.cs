using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

public class GetIdentifiersFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var chatId = context.HttpContext.User.FindFirst("chatId").Value;

                context.HttpContext.Items["ChatId"] = Int32.Parse(chatId);
            }
            catch(Exception e)
            {

            }
            context.HttpContext.Items["UserId"] = Int32.Parse(userId);

        }
    }
}