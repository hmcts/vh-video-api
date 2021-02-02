using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Video.API.Factories;
using Video.API.Services;
using Video.API.Swagger;
using VideoApi.Common;
using VideoApi.Common.Configuration;
using VideoApi.Common.Helpers;
using VideoApi.Common.Security;
using VideoApi.Common.Security.Kinly;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Handlers;
using VideoApi.Services.Kinly;

namespace Video.API
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            var contractsXmlFile = $"{typeof(BookNewConferenceRequest).Assembly.GetName().Name}.xml";
            var contractsXmlPath = Path.Combine(AppContext.BaseDirectory, contractsXmlFile);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Video API", Version = "v1"});
                c.AddFluentValidationRules();
                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(contractsXmlPath);
                c.EnableAnnotations();

                c.AddSecurityDefinition("Bearer", //Name the security scheme
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                        Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
                c.OperationFilter<AuthResponsesOperationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }

        public static IServiceCollection AddCustomTypes(this IServiceCollection services, IWebHostEnvironment environment, bool useStub)
        {
            var container = services.BuildServiceProvider();
            var servicesConfiguration = container.GetService<IOptions<ServicesConfiguration>>().Value;
            var wowzaConfiguration = container.GetService<IOptions<WowzaConfiguration>>().Value;
            var cvpConfiguration = container.GetService<IOptions<CvpConfiguration>>().Value;

            services.AddMemoryCache();
            services.AddScoped<ILoggingDataExtractor, LoggingDataExtractor>();
            services.AddScoped<IRoomReservationService, RoomReservationService>();

            services.AddScoped<ITokenProvider, AzureTokenProvider>();
            services.AddTransient<UserApiTokenHandler>();
            services.AddSingleton<ITelemetryInitializer, BadRequestTelemetry>();

            services.AddScoped<IQueryHandlerFactory, QueryHandlerFactory>();
            services.AddScoped<IQueryHandler, QueryHandler>();

            services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddScoped<ICommandHandler, CommandHandler>();

            services.AddScoped<IEventHandlerFactory, EventHandlerFactory>();
            services.AddTransient<KinlyApiTokenDelegatingHandler>();
            services.AddTransient<KinlySelfTestApiDelegatingHandler>();
            services.AddSingleton<IPollyRetryService, PollyRetryService>();

            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);
            RegisterEventHandlers(services);

            if (useStub)
            {
                services.AddScoped<IVideoPlatformService, KinlyPlatformServiceStub>();
                services.AddScoped<IAudioPlatformService, AudioPlatformServiceStub>();
                services.AddScoped<IConsultationService, ConsultationServiceStub>();
            }
            else
            {
                services
                    .AddHttpClient<IKinlyApiClient, KinlyApiClient>()
                    .AddTypedClient(httpClient => BuildKinlyClient(servicesConfiguration.KinlyApiUrl, httpClient))
                    .AddHttpMessageHandler<KinlyApiTokenDelegatingHandler>();

                foreach (var restApiEndpoint in wowzaConfiguration.RestApiEndpoints)
                {
                    var handler = new HttpClientHandler
                    {
                        Credentials = new CredentialCache
                        {
                            {
                                new Uri(restApiEndpoint), "Digest", new NetworkCredential(wowzaConfiguration.Username, wowzaConfiguration.Password)
                            }
                        }
                    };

                    var client = new WowzaHttpClient(new HttpClient(handler)
                    {
                        BaseAddress = new Uri(restApiEndpoint), DefaultRequestHeaders = {{"Accept", "application/json"}, {"ContentType", "application/json"}}
                    });

                    services.AddSingleton<IWowzaHttpClient>(client);
                }

                services
                    .AddHttpClient<IKinlySelfTestHttpClient, KinlySelfTestHttpClient>()
                    .AddHttpMessageHandler<KinlySelfTestApiDelegatingHandler>();

                services.AddScoped<IVideoPlatformService, KinlyPlatformService>();
                services.AddScoped<IAudioPlatformService, AudioPlatformService>();
                services.AddScoped<IConsultationService, ConsultationService>();
            }

            services.AddScoped<ICustomJwtTokenHandler, CustomJwtTokenHandler>();
            services.AddScoped<ICustomJwtTokenProvider, CustomJwtTokenProvider>();

            if (environment.IsDevelopment())
            {
                var vhBlobServiceClient = new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint),
                    new StorageSharedKeyCredential(wowzaConfiguration.StorageAccountName, wowzaConfiguration.StorageAccountKey));

                var cvpBlobServiceClient = new BlobServiceClient(new Uri(cvpConfiguration.StorageEndpoint),
                    new StorageSharedKeyCredential(cvpConfiguration.StorageAccountName, cvpConfiguration.StorageAccountKey));

                services.AddSingleton<IAzureStorageService>(x => new VhAzureStorageService(vhBlobServiceClient, wowzaConfiguration, false));
                services.AddSingleton<IAzureStorageService>(x => new CvpAzureStorageService(cvpBlobServiceClient, cvpConfiguration, false));

            }
            else
            {
                var vhBlobServiceClient = new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint),
                    new ChainedTokenCredential(new ManagedIdentityCredential(wowzaConfiguration.ManagedIdentityClientId),
                        new DefaultAzureCredential()));

                var cvpBlobServiceClient = new BlobServiceClient(new Uri(cvpConfiguration.StorageEndpoint),
                    new ChainedTokenCredential(new ManagedIdentityCredential(cvpConfiguration.ManagedIdentityClientId),
                        new DefaultAzureCredential()));

                services.AddSingleton<IAzureStorageService>(x => new VhAzureStorageService(vhBlobServiceClient, wowzaConfiguration, true));
                services.AddSingleton<IAzureStorageService>(x => new CvpAzureStorageService(cvpBlobServiceClient, cvpConfiguration, true));
            }

            services.AddSingleton<IAzureStorageServiceFactory, AzureStorageServiceFactory>();
            
            return services;
        }

        private static IKinlyApiClient BuildKinlyClient(string url, HttpClient httpClient)
        {
            var client = new KinlyApiClient(url, httpClient){ ReadResponseAsString = true};
            var contractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()};

            client.JsonSerializerSettings.ContractResolver = contractResolver;
            client.JsonSerializerSettings.Formatting = Formatting.Indented;

            return client;
        }

        private static void RegisterEventHandlers(IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan.FromAssemblyOf<IEventHandler>()
                .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler))
                .Where(_ => !_.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }

        private static void RegisterCommandHandlers(IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan.FromAssemblyOf<ICommand>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>))
                .Where(_ => !_.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            serviceCollection.Decorate(typeof(ICommandHandler<>), typeof(CommandHandlerLoggingDecorator<>));
        }

        private static void RegisterQueryHandlers(IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan.FromAssemblyOf<IQuery>()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>))
                .Where(_ => !_.IsGenericType))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            serviceCollection.Decorate(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));
        }

        public static IServiceCollection AddJsonOptions(this IServiceCollection serviceCollection)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            serviceCollection.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = contractResolver;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            return serviceCollection;
        }
    }
}
