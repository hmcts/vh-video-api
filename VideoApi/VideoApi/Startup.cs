using System;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Kinly;
using VideoApi.DAL;
using VideoApi.Extensions;
using VideoApi.Telemetry;
using VideoApi.ValidationMiddleware;
using VideoApi.Validations;
using VideoApi.Services;

namespace VideoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }
        public SettingsConfiguration SettingsConfiguration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson();
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

            services.AddApplicationInsightsTelemetry(options => options.ConnectionString = Configuration["ApplicationInsights:InstrumentationKey"]);
            services.AddApplicationInsightsTelemetryProcessor<SuccessfulDependencyProcessor>();

            services.AddJsonOptions();
            RegisterSettings(services);
            bool useStub = !bool.TryParse(Configuration["UseStub"], out useStub) || useStub;

            services.AddCustomTypes(Environment, useStub);
            RegisterAuth(services);
            services.AddTransient<IRequestModelValidatorService, RequestModelValidatorService>();

            services.AddMvc(opt => opt.Filters.Add(typeof(LoggingMiddleware)));
            services.AddMvc(opt => opt.Filters.Add(typeof(RequestModelValidatorFilter)))
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BookNewConferenceRequestValidation>());
            services.AddTransient<IValidatorFactory, RequestModelValidatorFactory>();
            services.AddDbContextPool<VideoApiDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("VideoApi"));
                });
        }

        private void RegisterSettings(IServiceCollection services)
        {
            SettingsConfiguration = Configuration.Get<SettingsConfiguration>();
            services.Configure<AzureAdConfiguration>(options => Configuration.Bind("AzureAd", options));
            services.Configure<ServicesConfiguration>(options => Configuration.Bind("Services", options));
            services.Configure<WowzaConfiguration>(options => Configuration.Bind("WowzaConfiguration", options));
            services.Configure<KinlyConfiguration>(options => Configuration.Bind("KinlyConfiguration", options));
            services.Configure<CvpConfiguration>(options => Configuration.Bind("CvpConfiguration", options));
            services.Configure<QuickLinksConfiguration>(options => Configuration.Bind("QuickLinks", options));
            services.AddSingleton(Configuration.GetSection("KinlyConfiguration").Get<KinlyConfiguration>());
            services.AddSingleton(Configuration.GetSection("WowzaConfiguration").Get<WowzaConfiguration>());
            services.AddSingleton<IBlobClientExtension, BlobClientExtension>();
            services.AddHostedService<LongRunningService>();
            services.AddSingleton<IBackgroundWorkerQueue, BackgroundWorkerQueue>();
        }

        private void RegisterAuth(IServiceCollection serviceCollection)
        {
            var securitySettings = Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();
            var serviceSettings = Configuration.GetSection("Services").Get<ServicesConfiguration>();

            if (!Environment.IsDevelopment())
            {
                if (String.IsNullOrEmpty(securitySettings.Authority))
                    throw new ArgumentException("authority missing");
            
                if (String.IsNullOrEmpty(securitySettings.TenantId))
                    throw new ArgumentException("TenantId missing");
            }

            serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority            = $"{securitySettings.Authority}{securitySettings.TenantId}";
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateLifetime = true,
                        ValidAudience = serviceSettings.VideoApiResourceId
                    };
                });

            serviceCollection.AddAuthorization(AddPolicies);
            serviceCollection.AddMvc(AddMvcPolicies);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseOpenApi();
            app.UseSwaggerUi3(c =>
            {
                c.DocumentTitle = "Video API V1";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if(!SettingsConfiguration.DisableHttpsRedirection)
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            
            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

            app.UseMiddleware<LogResponseBodyMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
        }

        private static void AddPolicies(AuthorizationOptions options)
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        }

        private static void AddMvcPolicies(MvcOptions options)
        {
            options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().Build()));
        }
    }
}
