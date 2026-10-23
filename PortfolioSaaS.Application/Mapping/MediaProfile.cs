using AutoMapper;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class MediaProfile : Profile
{
    public MediaProfile()
    {
        CreateMap<Media, MediaDto>();
    }
}
