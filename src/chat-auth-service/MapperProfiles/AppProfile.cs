using AuthService.Models;
using AuthService.Models.DTOs;
using AutoMapper;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<User, UserInfo>();
    }
}