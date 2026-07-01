using AutoMapper;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Application.DTOs.SectionTemplates;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class SectionTemplateService(BaseRepository<SectionTemplate> sectionTemplateRepository, IMapper mapper)
{
    private readonly BaseRepository<SectionTemplate> _sectionTemplateRepository = sectionTemplateRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<PagedList<SectionTemplateDto>> GetAllAsync(SectionTemplateFilterRequest request, PagedParameters parameters)
    {
        var spec = SectionTemplateSpecs.Page(request, parameters);
        var templates = await _sectionTemplateRepository.PageBySpecAsync(spec);

        return _mapper.Map<PagedList<SectionTemplateDto>>(templates);
    }

    public async Task<SectionTemplateDto?> GetByIdAsync(Guid id)
    {
        var template = await _sectionTemplateRepository.GetByIdAsync(id);
        if (template == null) return null;

        return new SectionTemplateDto
        {
            Id = template.Id,
            ComponentSelector = template.ComponentSelector,
            Name = template.Name,
            CategoryTags = template.CategoryTags,
            PreviewImageUrl = template.PreviewImageUrl,
            DefaultContentJson = template.DefaultContentJson,
        };
    }
}
