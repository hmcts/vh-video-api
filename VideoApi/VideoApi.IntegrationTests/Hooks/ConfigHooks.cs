using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using Azure.Storage.Blobs;
using FluentAssertions;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using TechTalk.SpecFlow;
using Testing.Common.Configuration;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Kinly;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using ConfigurationManager = AcceptanceTests.Common.Configuration.ConfigurationManager;

namespace VideoApi.IntegrationTests.Hooks
{
    [Binding]
    public class ConfigHooks
    {
        private const string KinlyApiSecretConfigKeyName = "KinlyConfiguration:ApiSecret";
        private const string KinlyCallbackSecretConfigKeyName = "KinlyConfiguration:CallbackSecret";

        private static IConfigurationRoot _configRoot;

        public ConfigHooks(TestContext context)
        {
            context.Config = new Config();
            context.Tokens = new VideoApiTokens();
        }

        /// <summary>
        /// This will insert a random callback secret per test run
        /// </summary>
        private static void AddRandomAccountKey()
        {
            var secret = Convert.ToBase64String(new HMACSHA256().Key);
            Environment.SetEnvironmentVariable(KinlyApiSecretConfigKeyName, secret);
            Environment.SetEnvironmentVariable(KinlyCallbackSecretConfigKeyName, secret);
        }

        [BeforeScenario(Order = (int)HooksSequence.ConfigHooks)]
        public void RegisterSecrets(TestContext context)
        {
            AddRandomAccountKey();
            _configRoot = ConfigRootBuilder.Build();
            RegisterDefaultData(context);
            RegisterHearingServices(context);
            RegisterKinlySettings(context);
            RegisterWowzaSettings(context);
            RegisterCvpSettings(context);
            RegisterDatabaseSettings(context);
            RegisterServer(context);
            RegisterApiSettings(context);
        }

        private static void RegisterDefaultData(TestContext context)
        {
            context.Test = new Test()
            {
                CaseName = "Video Api Integration Test",
                ClosedConferences = new List<Conference>(),
                ClosedConferencesWithMessages = new List<Conference>(),
                Conferences = new List<Conference>(),
                TodaysConferences = new List<Conference>()
            };
            context.Test.CaseName.Should().NotBeNullOrWhiteSpace();
        }

        private void RegisterHearingServices(TestContext context)
        {
            context.Config.Services = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.Services);
        }

        private void RegisterKinlySettings(TestContext context)
        {
            context.Config.KinlyConfiguration = Options.Create(_configRoot.GetSection("KinlyConfiguration").Get<KinlyConfiguration>()).Value;
            context.Config.KinlyConfiguration.CallbackUri = context.Config.Services.CallbackUri;
            context.Config.KinlyConfiguration.CallbackUri.Should().NotBeEmpty();
            context.Config.KinlyConfiguration.KinlyApiUrl.Should().NotBeEmpty();
        }

        private void RegisterWowzaSettings(TestContext context)
        {
            context.Config.Wowza = Options.Create(_configRoot.GetSection("WowzaConfiguration").Get<WowzaConfiguration>()).Value;
            context.Config.Wowza.StorageAccountKey.Should().NotBeNullOrEmpty();
            context.Config.Wowza.StorageAccountName.Should().NotBeNullOrEmpty();
            context.Config.Wowza.StorageContainerName.Should().NotBeNullOrEmpty();
        }

        private void RegisterCvpSettings(TestContext context)
        {
            context.Config.Cvp = Options.Create(_configRoot.GetSection("CvpConfiguration").Get<CvpConfiguration>()).Value;
            context.Config.Cvp.StorageAccountKey.Should().NotBeNullOrEmpty();
            context.Config.Cvp.StorageAccountName.Should().NotBeNullOrEmpty();
            context.Config.Cvp.StorageContainerName.Should().NotBeNullOrEmpty();
        }

        private void RegisterDatabaseSettings(TestContext context)
        {
            context.Config.DbConnection = Options.Create(_configRoot.GetSection("ConnectionStrings").Get<ConnectionStringsConfig>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(context.Config.DbConnection);
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<VideoApiDbContext>();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseSqlServer(context.Config.DbConnection.VideoApi);
            context.VideoBookingsDbContextOptions = dbContextOptionsBuilder.Options;
            context.TestDataManager = new TestDataManager(context.Config.KinlyConfiguration, context.VideoBookingsDbContextOptions);
        }

        private static void RegisterServer(TestContext context)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                    .UseKestrel(c => c.AddServerHeader = false)
                    .UseEnvironment("Development")
                    .UseStartup<Startup>()
                    .ConfigureTestServices(services =>
                    {
                        services.AddAuthentication(options =>
                        {
                            options.DefaultScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                            options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                        }).AddFakeJwtBearer();

                        // Remove application IEmailProvider service
                        var azStorageServices = services.Where(d => d.ServiceType == typeof(IAzureStorageService)).ToList();
                        foreach (var azStorageService in azStorageServices)
                        {
                            services.Remove(azStorageService);
                        }
                        var blobConnectionString = _configRoot.GetValue<string>("Azure:StorageConnectionString");
                        var connectionString =
                            "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
                        var serviceClient = new BlobServiceClient(connectionString);
                        
                        NUnit.Framework.TestContext.WriteLine($"Blob connectionstring is {blobConnectionString}");
                        var blobClientExtension = new BlobClientExtension();
                        
                        services.AddSingleton<IAzureStorageService>(x => new VhAzureStorageService(serviceClient, context.Config.Wowza, false, blobClientExtension));
                        services.AddSingleton<IAzureStorageService>(x => new CvpAzureStorageService(serviceClient, context.Config.Cvp, false, blobClientExtension));
                    });
            context.Server = new TestServer(webHostBuilder);
        }

        private static void RegisterApiSettings(TestContext context)
        {
            context.Response = new HttpResponseMessage();
        }
    }
}

