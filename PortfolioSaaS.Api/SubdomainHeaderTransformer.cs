using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace PortfolioSaaS.Api;

public class SubdomainHeaderTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // Configurar security schemes para Scalar UI
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        // Bearer token
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Introduce tu token en formato: \"Bearer {tu_token}\""
        };

        // X-Subdomain para multitenancia
        document.Components.SecuritySchemes["Subdomain"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-Subdomain",
            In = ParameterLocation.Header,
            Description = "Tenant subdomain for multitenancy (e.g., 'tenant1')"
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            },
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Subdomain"
                    }
                },
                Array.Empty<string>()
            }
        };
        document.SecurityRequirements.Add(securityRequirement);

        return Task.CompletedTask;
    }
}