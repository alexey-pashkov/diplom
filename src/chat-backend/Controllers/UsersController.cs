using AutoMapper;
using ChatApp.Models;
using ChatApp.Models.DTOs;
using ChatApp.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ChatDbContext dbContext;
        private readonly IMapper mapper;
        public UsersController(ChatDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = dbContext.Users;
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new {user.UserId, user.Login});
            }
        }

        [HttpPatch("{id:int}")]
        [GetIdentifiersFilter]
        public async Task<IActionResult> UpdateUserLogin(int id, [FromBody] UpdateLoginRequest update)
        {
            int userId = (int)HttpContext.Items["UserId"];

            if (userId != id)
            {
                return Unauthorized();
            }

            User user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Login = update.Login; 

            await dbContext.SaveChangesAsync();

            return Ok(mapper.Map<UserInfoDto>(user));
        }
    }
}
