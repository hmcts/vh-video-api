using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Video.API.Swagger;
using Video.API.Validations;
using VideoApi.Common;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.ServiceBus;
using VideoApi.Services;
using VideoApi.Services.Kinly;

namespace Video.API
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services
                .AddMvc()
                .AddFluentValidation(fv =>
                    fv.RegisterValidatorsFromAssemblyContaining<BookNewConferenceRequestValidation>());
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            var contractsXmlFile = $"{typeof(BookNewConferenceRequest).Assembly.GetName().Name}.xml";
            var contractsXmlPath = Path.Combine(AppContext.BaseDirectory, contractsXmlFile);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Video API", Version = "v1"});
                c.AddFluentValidationRules();
                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(contractsXmlPath);
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()},
                });
                c.OperationFilter<AuthResponsesOperationFilter>();
                c.SchemaFilter<EnumSchemaFilter>();
            });

            return services;
        }

        public static IServiceCollection AddCustomTypes(this IServiceCollection services, bool useStub)
        {
            services.AddMemoryCache();
            
            services.AddSingleton<ITelemetryInitializer, BadRequestTelemetry>();
            
            services.AddScoped<ITokenProvider, AzureTokenProvider>();

            services.AddScoped<IQueryHandlerFactory, QueryHandlerFactory>();
            services.AddScoped<IQueryHandler, QueryHandler>();
            
            services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddScoped<ICommandHandler, CommandHandler>();
            
            services.AddScoped<IEventHandlerFactory, EventHandlerFactory>();
            services.AddScoped<IServiceBusQueueClient, ServiceBusQueueClient>();
            
            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);
            RegisterEventHandlers(services);

            if (useStub)
            {
                services.AddScoped<IVideoPlatformService, KinlyPlatformServiceStub>();
            }
            else
            {
                services.AddScoped<IVideoPlatformService, KinlyPlatformService>();
                var container = services.BuildServiceProvider();
                var servicesConfiguration = container.GetService<IOptions<ServicesConfiguration>>().Value;
                services.AddHttpClient<IKinlyApiClient, KinlyApiClient>().AddTypedClient(httpClient =>
                    BuildKinlyClient(httpClient, servicesConfiguration));
            }
            
            services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.ContractResolver =
                        new DefaultContractResolver();
                }).AddHubOptions<EventHub>(options => { options.EnableDetailedErrors = true; });
            
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            
            return services;
        }

        private static IKinlyApiClient BuildKinlyClient(HttpClient httpClient,
            ServicesConfiguration servicesConfiguration)
        {
            return new KinlyApiClient(httpClient) {BaseUrl = servicesConfiguration.KinlyApiUrl};
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
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = contractResolver;
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                })
                .AddJsonOptions(options =>
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));

            return serviceCollection;
        }
    }
}