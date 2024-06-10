using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CompareId : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? reqId = (string?)context.RouteData.Values["id"];
        string? tokId = context.HttpContext.User
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;
        
        if (!(Int32.TryParse(reqId, out int parsedReqId) && Int32.TryParse(tokId, out int parsedTokId)))
        {
            context.Result = new BadRequestResult();
        }
        else
        {
            if (parsedReqId != parsedTokId)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}