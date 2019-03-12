using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ConferenceSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ConferenceEndpoints _endpoints = new ApiUriFactory().ConferenceEndpoints;

        public ConferenceSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }


    }
}
