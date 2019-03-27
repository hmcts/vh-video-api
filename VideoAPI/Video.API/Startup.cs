﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Video.API.Extensions;
using VideoApi.Common.Configuration;
using VideoApi.DAL;
using VideoApi.Events.Hub;

namespace Video.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger();
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
                }));
            
            services.AddJsonOptions();
            RegisterSettings(services);
            bool.TryParse(Configuration["UseStub"], out var useStub);
            services.AddCustomTypes(useStub);
            RegisterAuth(services);
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddDbContextPool<VideoApiDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("VhVideoApi")));
        }

        private void RegisterSettings(IServiceCollection services)
        {
            services.Configure<AzureAdConfiguration>(options => Configuration.Bind("AzureAd", options));
            services.Configure<ServiceBusSettings>(options => Configuration.Bind("ServiceBusQueue", options));
            services.Configure<ServicesConfiguration>(options => Configuration.Bind("Services", options));
        }

        private void RegisterAuth(IServiceCollection serviceCollection)
        {
            var securitySettings = Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();

            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"{securitySettings.Authority}{securitySettings.TenantId}";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidAudience = securitySettings.VhVideoApiResourceId
                };
            }).AddJwtBearer("EventHubUser", options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (string.IsNullOrEmpty(accessToken)) return Task.CompletedTask;

                        var path = context.HttpContext.Request.Path;
                        if (path.StartsWithSegments("/eventhub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
                options.Authority = $"{securitySettings.Authority}{securitySettings.TenantId}";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidAudience = securitySettings.VhVideoWebClientId
                };
            });

            serviceCollection.AddAuthorization(AddPolicies);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.RunLatestMigrations();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                const string url = "/swagger/v1/swagger.json";
                c.SwaggerEndpoint(url, "Video API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseCors("CorsPolicy");

            app.UseMiddleware<LogResponseBodyMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseMvc();

            app.UseSignalR(routes =>
            {
                const string path = "/eventhub";
                routes.MapHub<EventHub>(path,
                    options =>
                    {
                        options.Transports = HttpTransportType.ServerSentEvents | HttpTransportType.LongPolling |
                                             HttpTransportType.WebSockets;
                    });
            });
        }

        private static void AddPolicies(AuthorizationOptions options)
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("EventHubUser", new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("EventHubUser")
                .Build());
        }
    }
}