using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace WepApiAzureAdSwagger
{
    public class Startup
    {
        private readonly AzureAdSettings _azureAdSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _azureAdSettings = new AzureAdSettings();
            Configuration.Bind("AzureAd", _azureAdSettings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_azureAdSettings);

            services.AddControllers();
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);
            services.AddAuthorizationCore();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WepApiAzureAdSwagger", Version = "v1" });

                c.AddSecurityDefinition("aad-jwt", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(_azureAdSettings.AuthorizationUrl),
                            Scopes = new Dictionary<string, string>
                            {
                                { $"{_azureAdSettings.ApplicationIdUri}/{_azureAdSettings.Scope}", string.Empty } 
                            }
                        }
                    }
                });
                c.OperationFilter<OperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WepApiAzureAdSwagger v1");
                c.OAuthClientId(_azureAdSettings.ClientId);
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
