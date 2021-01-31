using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WepApiAzureAdSwagger
{
    public class OperationFilter : IOperationFilter
    {
        private readonly AzureAdSettings _settings;

        public OperationFilter(AzureAdSettings settings)
        {
            _settings = settings;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "aad-jwt"
                        },
                        UnresolvedReference = true
                    }, new[] { $"{_settings.ApplicationIdUri}/{_settings.Scope}"}
                }
            });
        }
    }
}
