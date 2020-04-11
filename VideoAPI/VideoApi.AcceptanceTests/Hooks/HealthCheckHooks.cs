using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Api.Healthchecks;
using System.Net;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public static class HealthCheckHooks
    {
        [BeforeScenario(Order = (int)HooksSequence.HealthCheckHooks)]
        public static void CheckApiHealth(TestContext context)
        {
            CheckVideoApiHealth(context.Config.VhServices.VideoApiUrl, context.Tokens.VideoApiBearerToken);
        }
        private static void CheckVideoApiHealth(string apiUrl, string bearerToken)
        {
            HealthcheckManager.CheckHealthOfVideoApi(apiUrl, bearerToken, (WebProxy)Zap.WebProxy);
        }
    }
}
