using Microsoft.Extensions.Configuration;

namespace Testing.Common.Configuration
{
    public static class ConfigRootBuilder
    {
        private const string UserSecretId = "9AECE566-336D-4D16-88FA-7A76C27321CD";
        public static IConfigurationRoot Build(string userSecretId = UserSecretId, bool useSecrets = true)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true)
                .AddJsonFile("appsettings.Production.json", true); // CI write variables in the pipeline to this file

            if (useSecrets)
            {
                builder = builder.AddUserSecrets(userSecretId);
            }

            return builder.AddEnvironmentVariables()
                .Build();
        }
    }
}
