using AutoMapper;
using PortfolioSaaS.Application.DTOs.SectionTemplates;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class SectionTemplateProfile : Profile
{
    public SectionTemplateProfile()
    {
        CreateMap<SectionTemplate, SectionTemplateDto>();
    }
}
