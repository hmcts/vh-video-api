using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using VideoApi.Common;
using VideoApi.Common.Configuration;
using VideoApi.Common.Helpers;
using VideoApi.Common.Security;
using VideoApi.Common.Security.Kinly;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Services.Factories;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Handlers;
using VideoApi.Services.Kinly;
using VideoApi.Swagger;
using ZymLabs.NSwag.FluentValidation;

namespace VideoApi
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSingleton<FluentValidationSchemaProcessor>();
            services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.AddSecurity("JWT", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: Bearer {your JWT token}.",
                        Scheme = "bearer"
                    });
                document.Title = "Video API";
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                document.OperationProcessors.Add(new AuthResponseOperationProcessor());
            });
            return services;
        }

        public static IServiceCollection AddCustomTypes(this IServiceCollection services, IWebHostEnvironment environment, bool useStub)
        {
            var container = services.BuildServiceProvider();
            var kinlyConfiguration = container.GetService<IOptions<KinlyConfiguration>>().Value;
            var wowzaConfiguration = container.GetService<IOptions<WowzaConfiguration>>().Value;
            var cvpConfiguration = container.GetService<IOptions<CvpConfiguration>>().Value;

            services.AddMemoryCache();
            services.AddScoped<ILoggingDataExtractor, LoggingDataExtractor>();

            services.AddScoped<ITokenProvider, AzureTokenProvider>();
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
                services.AddScoped<IVirtualRoomService, VirtualRoomServiceStub>();
            }
            else
            {
                services
                    .AddHttpClient<IKinlyApiClient, KinlyApiClient>()
                    .AddTypedClient(httpClient => BuildKinlyClient(kinlyConfiguration.KinlyApiUrl, httpClient))
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
                services.AddScoped<IVirtualRoomService, VirtualRoomService>();
            }

            services.AddScoped<ICustomJwtTokenHandler, CustomJwtTokenHandler>();
            services.AddScoped<ICustomJwtTokenProvider, CustomJwtTokenProvider>();
            services.AddScoped<IQuickLinksJwtTokenProvider, QuickLinksJwtTokenProvider>();

            var blobClientExtension = new BlobClientExtension();

            if (environment.IsDevelopment())
            {
                var vhBlobServiceClient = new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint),
                    new StorageSharedKeyCredential(wowzaConfiguration.StorageAccountName, wowzaConfiguration.StorageAccountKey));

                var cvpBlobServiceClient = new BlobServiceClient(new Uri(cvpConfiguration.StorageEndpoint),
                    new StorageSharedKeyCredential(cvpConfiguration.StorageAccountName, cvpConfiguration.StorageAccountKey));



                services.AddSingleton<IAzureStorageService>(x => new VhAzureStorageService(vhBlobServiceClient, wowzaConfiguration, false, blobClientExtension));
                services.AddSingleton<IAzureStorageService>(x => new CvpAzureStorageService(cvpBlobServiceClient, cvpConfiguration, false, blobClientExtension));

            }
            else
            {
                var vhBlobServiceClient = new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint),
                    new ChainedTokenCredential(new ManagedIdentityCredential(wowzaConfiguration.ManagedIdentityClientId),
                        new DefaultAzureCredential()));

                var cvpBlobServiceClient = new BlobServiceClient(new Uri(cvpConfiguration.StorageEndpoint),
                    new ChainedTokenCredential(new ManagedIdentityCredential(cvpConfiguration.ManagedIdentityClientId),
                        new DefaultAzureCredential()));

                services.AddSingleton<IAzureStorageService>(x => new VhAzureStorageService(vhBlobServiceClient, wowzaConfiguration, true, blobClientExtension));
                services.AddSingleton<IAzureStorageService>(x => new CvpAzureStorageService(cvpBlobServiceClient, cvpConfiguration, true, blobClientExtension));
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
