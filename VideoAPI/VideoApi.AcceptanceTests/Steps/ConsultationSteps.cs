using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ConsultationSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ConsultationEndpoints _endpoints = new ApiUriFactory().ConsultationEndpoints;

        public ConsultationSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"I have a valid request private consultation request")]
        public void GivenIHaveAValidRequestPrivateConsultationRequest()
        {
            var individual = _context.NewConference.Participants.Find(x => x.UserRole.Equals(UserRole.Individual)).Id;
            var representative = _context.NewConference.Participants
                .Find(x => x.UserRole.Equals(UserRole.Representative)).Id;

            var request = new ConsultationRequest()
            {
                ConferenceId = _context.NewConferenceId,
                RequestedBy = individual,
                RequestedFor = representative,
                Answer = ConsultationAnswer.Accepted
            };
            _context.Request = _context.Post(_endpoints.HandleConsultationRequest, request);
        }
    }
}
