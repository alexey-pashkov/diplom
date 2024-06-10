using System.Security.Claims;
using AutoMapper;
using ChatApp.Models;
using ChatApp.Models.Requests;
using ChatApp.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{

    [Route("/api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly ChatDbContext dbContext;
        private readonly IMapper mapper;
        public ChatsController(ChatDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewChat([FromBody] NewChatRequest request)
        {
            Chat newChat = new Chat
            {
                Name = request.ChatName,
                CreationDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await dbContext.Chats.AddAsync(newChat);
            await dbContext.SaveChangesAsync();

            int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await dbContext.Users.FindAsync(userId)!;

            await dbContext.UsersInChats.AddAsync(new UsersInChats
            {
                User = user,
                UserId = user.UserId,
                Chat = newChat,
                ChatId = newChat.ChatId,
                UserRole = (int)UserRolesEnum.Admin
            });

            await dbContext.SaveChangesAsync();

            return Ok(mapper.Map<ChatInfoDto>(newChat));
        }

        [Authorize(Roles = "admin", Policy = "ChatsTokenPolicy")]
        [HttpDelete]
        public async Task<IActionResult> DeleteChat([FromBody] DeleteChatRequest toDelete)
        {
            Chat? chat = await dbContext.Chats.FindAsync(toDelete.ChatId);

            if (chat == null)
            {
                return NotFound();
            }

            dbContext.Chats.Remove(chat);

            await dbContext.SaveChangesAsync();

            return Ok();
        }


        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetChatInfo(int id)
        {
            var chat = await dbContext.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }
            

            return Ok(mapper.Map<ChatInfoDto>(chat));
        }
    }

}