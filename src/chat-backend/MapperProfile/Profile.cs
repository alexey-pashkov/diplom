using AutoMapper;
using ChatApp.Models;
using ChatApp.Models.DTOs;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<Chat, ChatInfoDto>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.CreationDate));

        CreateMap<User, UserInfoDto>();
    }
}