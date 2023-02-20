using Microsoft.Extensions.Configuration;

namespace Testing.Common.Configuration
{
    public static class ConfigRootBuilder
    {
        public const string UserSecretId = "9AECE566-336D-4D16-88FA-7A76C27321CD";
        public static IConfigurationRoot Build(string userSecretId = UserSecretId)
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Production.json", true) // CI write variables in the pipeline to this file
                .AddUserSecrets(userSecretId)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
