using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.DAL;
using VideoApi.Extensions;
using VideoApi.Health;
using VideoApi.Middleware.Logging;
using VideoApi.Middleware.Validation;
using VideoApi.Telemetry;
using VideoApi.Services;

namespace VideoApi
{
    [ExcludeFromCodeCoverage]
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
            
            var envName = Configuration["Services:VideoApiUrl"];
            services.AddSingleton<IFeatureToggles>(new FeatureToggles(Configuration["LaunchDarkly:SdkKey"], envName));

            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsTelemetryProcessor<SuccessfulDependencyProcessor>();

            services.AddJsonOptions();
            RegisterSettings(services);
            bool useStub = !bool.TryParse(Configuration["UseStub"], out useStub) || useStub;

            services.AddCustomTypes(Environment, useStub);
            RegisterAuth(services);


            services.AddMvc(opt => opt.Filters.Add(typeof(LoggingMiddleware)));
            services.AddTransient<IRequestModelValidatorService, RequestModelValidatorService>();

            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(LoggingMiddleware));
                opt.Filters.Add(typeof(RequestModelValidatorFilter));
                opt.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), 500));
            });
            services.AddValidatorsFromAssemblyContaining<IRequestModelValidatorService>();

            services.AddVhHealthChecks();
            services.AddDbContextPool<VideoApiDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("VideoApi"),
                    builder => builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null));
            });
        }

        private void RegisterSettings(IServiceCollection services)
        {
            SettingsConfiguration = Configuration.Get<SettingsConfiguration>();
            services.Configure<AzureAdConfiguration>(options => Configuration.Bind("AzureAd", options));
            services.Configure<ServicesConfiguration>(options => Configuration.Bind("Services", options));
            services.Configure<WowzaConfiguration>(options => Configuration.Bind("WowzaConfiguration", options));
            services.Configure<KinlyConfiguration>(options => Configuration.Bind("KinlyConfiguration", options));
            services.Configure<VodafoneConfiguration>(options => Configuration.Bind("VodafoneConfiguration", options));
            services.Configure<CvpConfiguration>(options => Configuration.Bind("CvpConfiguration", options));
            services.Configure<QuickLinksConfiguration>(options => Configuration.Bind("QuickLinks", options));
            
            services.AddSingleton(Configuration.GetSection("KinlyConfiguration").Get<KinlyConfiguration>());
            services.AddSingleton(Configuration.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>());
            services.AddSingleton(Configuration.GetSection("WowzaConfiguration").Get<WowzaConfiguration>());
            
            services.AddSingleton<IBlobClientExtension, BlobClientExtension>();
            services.AddHostedService<LongRunningService>();
            services.AddSingleton<IBackgroundWorkerQueue, BackgroundWorkerQueue>();
        }

        private void RegisterAuth(IServiceCollection serviceCollection)
        {
            var securitySettings = Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();
            var serviceSettings = Configuration.GetSection("Services").Get<ServicesConfiguration>();

            serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = $"{securitySettings.Authority}{securitySettings.TenantId}";
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseOpenApi();
            app.UseSwaggerUi(c => { c.DocumentTitle = "Video API V1"; });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (!SettingsConfiguration.DisableHttpsRedirection)
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();
            app.UseCors("CorsPolicy");

            app.UseMiddleware<RequestBodyLoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute(); 
                
                endpoints.MapHealthChecks("/health/liveness", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("self"),
                    ResponseWriter = HealthCheckResponseWriter
                });

                endpoints.MapHealthChecks("/health/startup", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("startup"),
                    ResponseWriter = HealthCheckResponseWriter
                });
                
                endpoints.MapHealthChecks("/health/readiness", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("readiness"),
                    ResponseWriter = HealthCheckResponseWriter
                });
            });
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
        
        private async Task HealthCheckResponseWriter(HttpContext context, HealthReport report)
        {
            var result = JsonConvert.SerializeObject(new
            {
                status = report.Status.ToString(),
                details = report.Entries.Select(e => new
                {
                    key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                    error = e.Value.Exception?.Message
                })
            });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    }
}
