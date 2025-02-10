using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.Generation.Processors.Security;
using VideoApi.Common;
using VideoApi.Common.Configuration;
using VideoApi.Common.Helpers;
using VideoApi.Common.Security;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Factories;
using VideoApi.Services.Handlers;
using VideoApi.Services.Handlers.Vodafone;
using VideoApi.Swagger;
using ZymLabs.NSwag.FluentValidation;

namespace VideoApi
{
    [ExcludeFromCodeCoverage]
    public static class ConfigureServicesExtensions
    {
        /// <summary>
        /// The AddSwagger function configures Swagger documentation and security settings for a C#
        /// application.
        /// </summary>
        /// <param name="services">The `IServiceCollection` interface in ASP.NET Core is used
        /// to register application services. In the provided code snippet, the `AddSwagger` method
        /// extends the functionality of `IServiceCollection` by adding Swagger documentation and
        /// security definitions to the service collection.</param>
        /// <returns>
        /// The `AddSwagger` method is returning the `IServiceCollection` after configuring Swagger
        /// documentation and security settings for the Video API.
        /// </returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddScoped(provider =>
            {
                var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
                var loggerFactory = provider.GetService<ILoggerFactory>();
                
                return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
            });
            services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT",
                        new OpenApiSecurityScheme
                        {
                            Type = OpenApiSecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            In = OpenApiSecurityApiKeyLocation.Header,
                            Description = "Type into the textbox: Bearer {your JWT token}.",
                            Scheme = "bearer"
                        }));
                document.Title = "Video API";
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                document.OperationProcessors.Add(new AuthResponseOperationProcessor());
                var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider
                    .GetService<FluentValidationSchemaProcessor>();
                // Add the fluent validations schema processor
                document.SchemaSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
            });
            return services;
        }
        
        /// <summary>
        /// The AddCustomTypes function configures and registers various services and dependencies based
        /// on the provided environment and configuration settings.
        /// </summary>
        /// <param name="services">IServiceCollection is a built-in interface in ASP.NET Core
        /// that defines a contract for a collection of service descriptors. It is used to register
        /// application services with the built-in dependency injection container. The AddCustomTypes
        /// method you provided is an extension method for IServiceCollection that registers various
        /// custom types and services within the</param>
        /// <param name="environment">The `IWebHostEnvironment` parameter in the
        /// `AddCustomTypes` method is used to provide information about the web hosting environment in
        /// which the application is running. It provides access to the application's content root path,
        /// environment name, and other environment-related information. This parameter is typically
        /// used to determine the</param>
        /// <param name="useStub">The `useStub` parameter in the `AddCustomTypes` method is a boolean
        /// flag that determines whether to register stub implementations of certain services or the
        /// actual implementations. If `useStub` is `true`, stub implementations will be registered for
        /// services like `IAudioPlatformService`, `IConsultation</param>
        /// <returns>
        /// The method `AddCustomTypes` is returning an `IServiceCollection` after configuring various
        /// services and dependencies within the method.
        /// </returns>
        public static IServiceCollection AddCustomTypes(this IServiceCollection services,
            IWebHostEnvironment environment, bool useStub)
        {
            var container = services.BuildServiceProvider();
            var vodafoneConfiguration = container.GetService<IOptions<VodafoneConfiguration>>().Value;
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
            
            services.AddTransient<VodafoneApiTokenDelegatingHandler>();
            
            services.AddSingleton<IPollyRetryService, PollyRetryService>();
            
            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);
            RegisterEventHandlers(services);
            
            if (useStub)
            {
                var vodafoneConfigOptions = container.GetService<IOptions<VodafoneConfiguration>>();
                services.AddScoped<IAudioPlatformService, AudioPlatformServiceStub>();
                services.AddScoped<IConsultationService, ConsultationServiceStub>();
                services.AddScoped<ISupplierPlatformServiceFactory>(_ =>
                    new TestSupplierPlatformServiceFactory(vodafoneConfigOptions.Value));
            }
            else
            {
                services.AddTransient<SupplierLoggingDelegatingHandler>();

                services
                    .AddHttpClient<IVodafoneApiClient, SupplierApiClient>()
                    .AddTypedClient<IVodafoneApiClient>(httpClient =>
                        BuildSupplierClient(vodafoneConfiguration.ApiUrl, httpClient))
                    .AddHttpMessageHandler<VodafoneApiTokenDelegatingHandler>()
                    .AddHttpMessageHandler<SupplierLoggingDelegatingHandler>();
                
                services.AddScoped<IAudioPlatformService, AudioPlatformService>();
                services.AddScoped<IConsultationService, ConsultationService>();
                services.AddScoped<ISupplierPlatformServiceFactory, SupplierPlatformServiceFactory>();
            }
            
            services.AddScoped<IVodafoneJwtProvider, VodafoneJwtProvider>();
            services.AddScoped<IVodafoneJwtTokenHandler, VodafoneJwtHandler>();
            
            services.AddScoped<IQuickLinksJwtTokenProvider, QuickLinksJwtTokenProvider>();
            
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IInstantMessageService, InstantMessageService>();
            
            var blobClientExtension = new BlobClientExtension();
            
            if (environment.IsDevelopment())
            {
                var vhBlobServiceClient = new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint),
                    new StorageSharedKeyCredential(wowzaConfiguration.StorageAccountName,
                        wowzaConfiguration.StorageAccountKey));
                
                var cvpBlobServiceClient = new BlobServiceClient(new Uri(cvpConfiguration.StorageEndpoint),
                    new StorageSharedKeyCredential(cvpConfiguration.StorageAccountName,
                        cvpConfiguration.StorageAccountKey));
                
                
                services.AddSingleton<IAzureStorageService>(x =>
                    new VhAzureStorageService(vhBlobServiceClient, wowzaConfiguration, false, blobClientExtension));
                services.AddSingleton<IAzureStorageService>(x =>
                    new CvpAzureStorageService(cvpBlobServiceClient, cvpConfiguration, false, blobClientExtension));
            }
            else
            {
                var vhBlobServiceClient = new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint),
                    new ChainedTokenCredential(
                        new ManagedIdentityCredential(wowzaConfiguration.ManagedIdentityClientId),
                        new DefaultAzureCredential()));
                
                var cvpBlobServiceClient = new BlobServiceClient(new Uri(cvpConfiguration.StorageEndpoint),
                    new ChainedTokenCredential(new ManagedIdentityCredential(cvpConfiguration.ManagedIdentityClientId),
                        new DefaultAzureCredential()));
                
                services.AddSingleton<IAzureStorageService>(x =>
                    new VhAzureStorageService(vhBlobServiceClient, wowzaConfiguration, true, blobClientExtension));
                services.AddSingleton<IAzureStorageService>(x =>
                    new CvpAzureStorageService(cvpBlobServiceClient, cvpConfiguration, true, blobClientExtension));
            }
            
            services.AddSingleton<IAzureStorageServiceFactory, AzureStorageServiceFactory>();
            
            return services;
        }
        
        private static SupplierApiClient BuildSupplierClient(string url, HttpClient httpClient)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                throw new InvalidOperationException($"Invalid Supplier API URL provided: {url}");
            }
            Console.WriteLine("Using Supplier API URL: " + url);
            httpClient.BaseAddress = new Uri(url.TrimEnd('/'), UriKind.RelativeOrAbsolute);
            var client = new SupplierApiClient(httpClient)
            {
                BaseUrlAddress = url
            };
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
            serviceCollection.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                });

            return serviceCollection;
        }
    }
}
