using Microsoft.EntityFrameworkCore;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Middleware;

public class TenantResolutionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, ApplicationDbContext db)
    {
        var subdomain = context.Request.Headers["X-Subdomain"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(subdomain))
        {
            var tenant = await db.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.ConfiguredDomain == subdomain);
            if (tenant != null)
            {
                tenantContext.SetTenant(tenant);
                if (context.User.Identity?.IsAuthenticated is true)
                {
                    var user = await db.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.TenantId == tenant.Id && u.Email == context.User.Identity.Name);
                    if (user != null)
                    {
                        tenantContext.SetUser(user);
                    }
                }
            }
        }
        await _next(context);
    }
}
