using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Domain.Enums;

namespace PortfolioSaaS.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();

        // Seed Section Templates
        SectionTemplate? richTextTemplate = null;
        SectionTemplate? backGroundImageTextTemplate = null;
        SectionTemplate? imageTextTemplate = null;
        if (!await db.SectionTemplates.AnyAsync())
        {
            (richTextTemplate, backGroundImageTextTemplate, imageTextTemplate) = await SeedSectionTemplatesAsync(db);
        }
        if (await db.Tenants.AnyAsync())
            return;

        var admin = new Tenant
        {
            Id = Guid.NewGuid(),
            ConfiguredDomain = "mattialu"
        };

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = admin.Id,
            Email = "lumattia88@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.PlatformAdmin
        };

        var demoTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            ConfiguredDomain = "juan"
        };

        var demoUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = demoTenant.Id,
            Email = "juan@localhost",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tenant123!"),
            Role = UserRole.TenantOwner
        };

        // Now create some demo Page/Section data too!
        var homePageId = Guid.NewGuid();
        var homePage = new Page
        {
            Id = homePageId,
            TenantId = admin.Id,
            Title = "Home",
            Slug = "",
            MetaDescription = "Welcome to my portfolio",
            Disabled = false
        };
        // Don't set HomePageId yet to avoid circular dependency

        if (richTextTemplate != null)
        {
            homePage.Sections.Add(new Section
            {
                Id = Guid.NewGuid(),
                PageId = homePageId,
                SectionTemplateId = richTextTemplate.Id,
                ContentJson = """{"inputs":{"text":"Testing"}}""",
                Order = 0,
                IsEnabled = true,
            });
        }

        if (imageTextTemplate != null)
        {
            homePage.Sections.Add(new Section
            {
                Id = Guid.NewGuid(),
                PageId = homePageId,
                SectionTemplateId = imageTextTemplate.Id,
                ContentJson = """{"inputs":{"image":"https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=600","text":"About Me"}}""",
                Order = 1,
                IsEnabled = true
            });
        }

        var themeConfig = new ThemeConfig
        {
            Id = Guid.NewGuid(),
            TenantId = demoTenant.Id,
            Light = new Color
            {
                PrimaryColor = "#6366f1"
            }
        };
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            TenantId = admin.Id,
            Type = MenuType.Sidebar,
            MenuItems =
        [
            new()
            {
                Id = Guid.NewGuid(),
                Text = "Home",
                Url = "",
                Order = 0
            },
            new()
            {
                Id = Guid.NewGuid(),
                Text = "GitHub",
                Url = "https://github.com",
                Order = 1
            }
        ]
        };

        db.Tenants.AddRange(admin, demoTenant);
        db.Users.AddRange(adminUser, demoUser);
        db.Pages.Add(homePage);
        db.ThemeConfigs.Add(themeConfig);
        db.Menus.Add(menu);
        await db.SaveChangesAsync();

        db.Tenants.Update(demoTenant);
        await db.SaveChangesAsync();
    }

    private static async Task<(SectionTemplate? centeredText, SectionTemplate? hero, SectionTemplate? imageLeft)> SeedSectionTemplatesAsync(ApplicationDbContext db)
    {
        var imageSectionId = Guid.NewGuid();
        var richTextSectionId = Guid.NewGuid();
        var imageTextSectionId = Guid.NewGuid();
        var backgroundImageTextSectionId = Guid.NewGuid();
        var carouselSectionId = Guid.NewGuid();

        var templates = new List<SectionTemplate>
        {
            new()
            {
                Id = imageSectionId,
                ComponentSelector = "image",
                Name = "Image Section",
                CategoryTags = SectionCategory.Image,
                DefaultContentJson = """{"image":""}""",
            },
            new()
            {
                Id = richTextSectionId,
                ComponentSelector = "rich-text",
                Name = "Rich Text Section",
                CategoryTags = SectionCategory.Text ,
                DefaultContentJson = """{"text":""}""",
            },
            new()
            {
                Id = imageTextSectionId,
                ComponentSelector = "image-text",
                Name = "Image + Text Section",
                CategoryTags = SectionCategory.Text | SectionCategory.Image,
            },
            new()
            {
                Id = backgroundImageTextSectionId,
                ComponentSelector = "background-image-text",
                Name = "Background Image + Text Section",
                CategoryTags = SectionCategory.Text | SectionCategory.Image,
            },
            new()
            {
                Id = carouselSectionId,
                ComponentSelector = "carousel",
                Name = "Carousel Section",
                CategoryTags = SectionCategory.Image | SectionCategory.Container,
            }
        };

        db.SectionTemplates.AddRange(templates);
        await db.SaveChangesAsync();

        return (
            templates.First(t => t.Id == richTextSectionId),
            templates.First(t => t.Id == backgroundImageTextSectionId),
            templates.First(t => t.Id == imageTextSectionId)
        );
    }
}
