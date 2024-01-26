using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace CityInfo.API.Services;

/// <summary>
/// Configures the Swagger generation options.  The IConfigureOptions approach is used to avoid having to 
/// create an in-between service provider simply to get an IApiVersionDescriptionProvider 
/// (which is required to created versioned OpenAPI specifications)
/// </summary> 
public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider) 
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
 
    public void Configure(SwaggerGenOptions swaggerGenOptions)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently

        foreach (var description in
            _apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            swaggerGenOptions.SwaggerDoc(
                $"{description.GroupName}",
                new()
                {
                    Title = "City Info API",
                    Version = description.ApiVersion.ToString(),
                    Description = "Through this API you can access cities and their points of interest."
                });
        }

        var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

        swaggerGenOptions.IncludeXmlComments(xmlCommentsFullPath);

        swaggerGenOptions.AddSecurityDefinition("CityInfoApiBearerAuth", new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            Description = "Input a valid token to access this API"
        });

        swaggerGenOptions.AddSecurityRequirement(new()
        {
            {
                new ()
                {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "CityInfoApiBearerAuth" }
                },
                new List<string>()
            }
        });
    } 
} 