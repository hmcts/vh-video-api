using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CommonSteps : BaseSteps
    {
        private readonly TestContext _context;

        public CommonSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [When(@"I send the request to the endpoint")]
        public void WhenISendTheRequestToTheEndpoint()
        {
            _context.Response = _context.Client().Execute(_context.Request);
            if (_context.Response.Content != null)
                _context.Json = _context.Response.Content;
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode httpStatusCode, bool isSuccess)
        {
            _context.Response.StatusCode.Should().Be(httpStatusCode);
            _context.Response.IsSuccessful.Should().Be(isSuccess);
        }

        private List<int> _numbers;
        private int _total;

        [Given(@"I have entered (.*) into the calculator")]
        public void IEnterIntoCalc(int number)
        {
            _numbers.Add(number);
        }

        [When(@"I press add")]
        public void WhenIAdd()
        {
            foreach (var number in _numbers)
            {
                _total += number;
            }
        }

        [Then(@"the result should be (.*) on the screen")]
        public void ThenTotal(int expectedTotal)
        {
            _total.Should().Be(expectedTotal);
        }
    }
}