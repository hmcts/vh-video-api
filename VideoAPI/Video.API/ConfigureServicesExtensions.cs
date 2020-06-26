using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Video.API.Swagger;
using VideoApi.Common;
using VideoApi.Common.Configuration;
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

            services.AddMemoryCache();
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
            }
            else
            {
                services
                    .AddHttpClient<IKinlyApiClient, KinlyApiClient>()
                    .AddTypedClient(httpClient => BuildKinlyClient(servicesConfiguration.KinlyApiUrl, httpClient))
                    .AddHttpMessageHandler<KinlyApiTokenDelegatingHandler>();

                services.AddHttpClient<IWowzaHttpClient, WowzaHttpClient>(x =>
                {
                    x.BaseAddress = new Uri(wowzaConfiguration.RestApiEndpoint);
                    x.DefaultRequestHeaders.Add("Accept", "application/json");
                    x.DefaultRequestHeaders.Add("ContentType", "application/json");
                }).ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler
                {
                    Credentials = new CredentialCache
                    {
                        {
                            new Uri(wowzaConfiguration.RestApiEndpoint),
                            "Digest",
                            new NetworkCredential(wowzaConfiguration.Username, wowzaConfiguration.Password)
                        }
                    }
                });

                services
                    .AddHttpClient<IKinlySelfTestHttpClient, KinlySelfTestHttpClient>()
                    .AddTypedClient(httpClient => BuildKinlyClient(servicesConfiguration.KinlySelfTestApiUrl, httpClient))
                    .AddHttpMessageHandler<KinlySelfTestApiDelegatingHandler>();

                services.AddScoped<IVideoPlatformService, KinlyPlatformService>();
                services.AddScoped<IAudioPlatformService, AudioPlatformService>();
            }

            services.AddScoped<ICustomJwtTokenHandler, CustomJwtTokenHandler>();
            services.AddScoped<ICustomJwtTokenProvider, CustomJwtTokenProvider>();

            if (environment.IsDevelopment())
            {
                services.AddSingleton
                (
                    x =>
                    {
                        var credentials = new StorageSharedKeyCredential(wowzaConfiguration.StorageAccountName, wowzaConfiguration.StorageAccountKey);
                        return new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint), credentials);
                    }
                );
            }
            else
            {
                services.AddSingleton
                (
                    x =>
                    {
                        var defaultAzureCredential = new DefaultAzureCredential();
                        var managedIdentityCredential = new ManagedIdentityCredential(wowzaConfiguration.ManagedIdentityClientId);
                        var chainedTokenCredential = new ChainedTokenCredential(managedIdentityCredential, defaultAzureCredential);
                        return new BlobServiceClient(new Uri(wowzaConfiguration.StorageEndpoint), chainedTokenCredential);
                    }
                );
            }

            services.AddScoped<IStorageService>(x => new AzureStorageService(x.GetService<BlobServiceClient>(), wowzaConfiguration, environment.IsProduction()));

            return services;
        }

        private static IKinlyApiClient BuildKinlyClient(string url, HttpClient httpClient)
        {
            var client = new KinlyApiClient(url, httpClient);
            var contractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()};

            client.JsonSerializerSettings.ContractResolver = contractResolver;
            client.JsonSerializerSettings.Formatting = Formatting.Indented;

            return client;
        }

        /// <summary>
        /// Temporary work-around until typed-client bug is restored
        /// https://github.com/dotnet/aspnetcore/issues/13346#issuecomment-535544207
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="factory"></param>
        /// <typeparam name="TClient"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static IHttpClientBuilder AddTypedClient<TClient>(this IHttpClientBuilder builder,
            Func<HttpClient, TClient> factory)
            where TClient : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            builder.Services.AddTransient(s =>
            {
                var httpClientFactory = s.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(builder.Name);

                return factory(httpClient);
            });

            return builder;
        }

        private static void RegisterEventHandlers(IServiceCollection serviceCollection)
        {
            var eventHandlers = GetAllTypesOf<IEventHandler>();

            foreach (var eventHandler in eventHandlers)
            {
                if (eventHandler.IsInterface || eventHandler.IsAbstract) continue;
                var serviceType = eventHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, eventHandler);
            }
        }

        private static IEnumerable<Type> GetAllTypesOf<T>()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            return runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => typeof(T).IsAssignableFrom(t));
        }

        private static void RegisterCommandHandlers(IServiceCollection serviceCollection)
        {
            var commandHandlers = typeof(ICommand).Assembly.GetTypes().Where(t =>
                t.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(ICommandHandler<>)));

            foreach (var queryHandler in commandHandlers)
            {
                var serviceType = queryHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, queryHandler);
            }
        }

        private static void RegisterQueryHandlers(IServiceCollection serviceCollection)
        {
            var queryHandlers = typeof(IQuery).Assembly.GetTypes().Where(t =>
                t.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

            foreach (var queryHandler in queryHandlers)
            {
                var serviceType = queryHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, queryHandler);
            }
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
