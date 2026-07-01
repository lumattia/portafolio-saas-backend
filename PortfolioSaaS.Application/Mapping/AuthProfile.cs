using AutoMapper;
using PortfolioSaaS.Application.DTOs.Auth;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, LoginResponse>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role))
            .ForMember(d => d.Token, opt => opt.Ignore());
    }
}
