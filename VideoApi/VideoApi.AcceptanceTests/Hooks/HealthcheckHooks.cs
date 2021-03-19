using System;
using System.Net;
using FluentAssertions;
using Polly;
using RestSharp;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public class HealthcheckHooks
    {
        private const int Retries = 4;

        [BeforeScenario(Order = (int) HooksSequence.HealthCheckHooks)]
        public void CheckApiHealth(TestContext context)
        {
            var retryOnForbiddenFirewallExceptions = Policy
                .HandleResult<IRestResponse>(r => r.StatusCode == HttpStatusCode.Forbidden)
                .WaitAndRetry(Retries, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan) =>
                    {
                        NUnit.Framework.TestContext.WriteLine($"Encountered error '{exception.Result.StatusCode}' after {timeSpan.Seconds} seconds. Retrying...");
                    });
            
            var response = retryOnForbiddenFirewallExceptions.Execute(() => context.Client().Execute(context.Get(ApiUriFactory.HealthCheckEndpoints.CheckServiceHealth)));
            response.StatusCode.Should().Be(HttpStatusCode.OK, $"Healthcheck failed with '{response.StatusCode}' and error message '{response.ErrorMessage}'");
        }
    }
}
