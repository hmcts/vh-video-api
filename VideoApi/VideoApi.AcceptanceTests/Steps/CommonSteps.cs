using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CommonSteps
    {
        private readonly TestContext _context;

        public CommonSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [When(@"I send the request to the endpoint")]
        [When(@"I resend the request to the endpoint")]
        public void WhenISendTheRequestToTheEndpoint()
        {
            _context.Response = _context.Client().Execute(_context.Request);
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode httpStatusCode, bool isSuccess)
        {
            _context.Response.StatusCode.Should().Be(httpStatusCode, $"Request should be valid but, {_context.Response.Content}");
            _context.Response.IsSuccessful.Should().Be(isSuccess);
        }
    }
}
