using System.Security.Cryptography;
using System.Text;
using AuthService.Models;
using AuthService.Models.DTOs;
using AuthService.Models.Requsets;
using AuthService.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandomString4Net;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ChatDbContext dbContext;
    private readonly JwtService jwtService;
    private readonly IMapper mapper;
    public AuthController(ChatDbContext dbContext, JwtService jwtService, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.jwtService = jwtService;
        this.mapper = mapper;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(AccountRequest regData)
    {
        if (dbContext.Users.SingleOrDefault(u => u.Login == regData.Login) != null)
        {
            return Conflict("User already exist");
        }
        string salt = RandomString.GetString(Types.ALPHANUMERIC_MIXEDCASE, 10);

        byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(regData.Password + salt));

        User newUser = new User
        {
            Login = regData.Login,
            PasswordHash = passwordHash,
            PasswordSalt = Encoding.UTF8.GetBytes(salt)
        };

        await dbContext.Users.AddAsync(newUser);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }

        string refreshToken = jwtService.GetRefreshToken(newUser.UserId);
        string accessToken = jwtService.GetAccessToken(newUser.UserId, newUser.Login);

        HttpContext.Response.Headers.Authorization = "Bearer " + accessToken;

        return Ok("Bearer " + refreshToken);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody]AccountRequest account)
    {
        User? user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Login == account.Login);

        if (user == null)
        {
            return NotFound();
        }

        string salt = Encoding.UTF8.GetString(user.PasswordSalt);

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(account.Password + salt));

        if (!hash.SequenceEqual(user.PasswordHash))
        {
            return Unauthorized();
        }
        
        string refreshToken = jwtService.GetRefreshToken(user.UserId);
        string accessToken = jwtService.GetAccessToken(user.UserId, user.Login);

        HttpContext.Response.Headers.Authorization = "Bearer " + accessToken;

        return Ok("Bearer " + refreshToken);
    }
}