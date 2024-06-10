using System.Security.Claims;
using AuthService.Models;
using AuthService.Models.Requsets;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "RefreshTokenScheme")]
public class TokensController : ControllerBase
{
    private readonly ChatDbContext dbContext;
    private readonly JwtService jwtService;
    public TokensController(ChatDbContext dbContext, JwtService jwtService)
    {
        this.dbContext = dbContext;
        this.jwtService = jwtService;
    }

    [HttpGet("chat-token/{id:int}")]
    public async Task<IActionResult> GetChatToken(int id)
    {
        int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var user = await dbContext.Users.FindAsync(userId);

        var userChat = user.UsersInChats.SingleOrDefault(x => x.ChatId == id);

        if (userChat == null)
        {
            return Unauthorized();
        }

        var role = userChat
            .UserRoleNavigation;

        string chatToken = jwtService.GetChatToken(userChat.UserId, userChat.ChatId, role);

        HttpContext.Response.Headers.Authorization = chatToken;

        return Ok(new { ChatId = id, UserId = user.UserId, Role = role.RoleName });
    }

    [HttpGet("refresh-access-token")]
    public async Task<IActionResult> RefreshAccessToken()
    {
        int id = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var user = await dbContext.Users.FindAsync(id);
        string token = "Bearer " + jwtService.GetAccessToken(user.UserId, user.Login);

        HttpContext.Response.Headers.Authorization = token;

        return NoContent();
    }

    [HttpGet("refresh-chat-token")]
    public async Task<IActionResult> RefreshChatToken(string chatToken)
    {
        
        var claimsPrincipal = (ClaimsPrincipal)HttpContext.Items["ClaimsPrincipal"]!;

        int id = Int32.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var user = await dbContext.Users.FindAsync(id);

        int chatId = Int32.Parse(claimsPrincipal.FindFirst("chatId")?.Value!);

        var userChatInfo = user?.UsersInChats.Single(item => item.ChatId == chatId);

        string newToken = jwtService.GetChatToken(id, userChatInfo.ChatId, userChatInfo.UserRoleNavigation);

        return Ok(newToken);
    }
}