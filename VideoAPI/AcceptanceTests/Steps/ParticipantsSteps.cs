using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;

        public ParticipantsSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }


    }
}
