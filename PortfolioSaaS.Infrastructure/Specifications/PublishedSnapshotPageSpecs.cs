using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class PublishedSnapshotPageSpecs
{
    public class ByOriginalPageId : Specification<PublishedSnapshotPage>
    {
        public ByOriginalPageId(Guid originalPageId)
        {
            Query.Where(p => p.OriginalPageId == originalPageId)
                 .Include(x => x.Sections).ThenInclude(x => x.SectionTemplate);
        }
    }

    public class BySlug : Specification<PublishedSnapshotPage>
    {
        public BySlug(string slug)
        {
            Query.Where(p => p.Slug == slug)
                 .Include(x => x.Sections).ThenInclude(x => x.SectionTemplate);
        }
    }
}
