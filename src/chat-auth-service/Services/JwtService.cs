using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using AuthService.Models.Requsets;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class JwtService
{
    private readonly IConfiguration configuration;

    public JwtService(IConfiguration config)
    {
        configuration = config;
    }

    public string GetAccessToken(int id, string login)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] secret = Encoding.UTF8.GetBytes(configuration["JwtSettings:AppKey"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, login)
            }),
            IssuedAt = DateTime.Now,
            Issuer = configuration["JwtSettings:Issuer"],
            Expires = DateTime.Now.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GetRefreshToken(int id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] secret = Encoding.UTF8.GetBytes(configuration["JwtSettings:AppKey"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            ]),
            Issuer = configuration["JwtSettings:Issuer"],
            IssuedAt = DateTime.Now,
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GetChatToken(int userId, int chatId, UserRole userRole)
    {
        byte[] secret = Encoding.UTF8.GetBytes(configuration["JwtSettings:ChatsKey"]!);

        var claims = new List<Claim>
        {
            new Claim(type: ClaimTypes.NameIdentifier, value: userId.ToString()),
            new Claim(type: "chatId", value: chatId.ToString()),
            new Claim(type: ClaimTypes.Role, value: userRole.RoleName)
        };

        var key = new SymmetricSecurityKey(secret);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}