using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class HealthCheckSteps : StepsBase
    {
        private readonly HealthCheckEndpoints _endpoints = new ApiUriFactory().HealthCheckEndpoints;

        public HealthCheckSteps(ApiTestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I make a call to the healthcheck endpoint")]
        public void GivenIMakeACallToTheHealthCheckEndpoint()
        {
            ApiTestContext.Uri = _endpoints.CheckServiceHealth();
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }
    }
}