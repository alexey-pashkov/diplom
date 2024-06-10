using AutoMapper;
using ChatApp;
using ChatApp.Models;
using ChatApp.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[GetIdentifiersFilter]
public class UserController : ControllerBase
{
    private readonly ChatDbContext dbContext;
    private readonly IMapper mapper;

    public UserController(ChatDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    // [Authorize]
    // [HttpPatch]
    // public async Task<IActionResult> UpdateUser([FromBody] JsonPatchDocument<User> patchUser)
    // {
    //     int id = (int)HttpContext.Items["UserId"];
    //     var user = await dbContext.Users.FindAsync(id);

    //     if (user == null)
    //     {
    //         return BadRequest();
    //     }

    //     patchUser.ApplyTo(user);
    //     try
    //     {
    //         await dbContext.SaveChangesAsync();
    //     }
    //     catch
    //     {
    //         return StatusCode(500);
    //     }

    //     return Ok();
    // }

    [Authorize]
    [HttpGet("chats")]
    public async Task<IActionResult> GetUserChats()
    {
        int id = (int)HttpContext.Items["UserId"];
        var user = await dbContext.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var chats = user.UsersInChats
            .Select(x => x.Chat)
            .Select(mapper.Map<ChatInfoDto>);

        return Ok(chats);
    }
}