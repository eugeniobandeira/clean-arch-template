using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace CleanArch.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "CleanArch API",
                    Version = "v1",
                    Description = "API gerada com Clean Architecture + Vertical Slice Template",
                    Contact = new OpenApiContact
                    {
                        Name = "eugeniobandeira",
                        Url = new Uri("https://github.com/eugeniobandeira")
                    }
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Insira o token JWT no campo abaixo."
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "CleanArch API";
            options.Theme = ScalarTheme.DeepSpace;
        });

        return app;
    }
}
