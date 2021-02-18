using System.Linq;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using static Testing.Common.Helper.ApiUriFactory.ConsultationEndpoints;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ConsultationSteps
    {
        private readonly TestContext _context;

        public ConsultationSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"I have a valid request private consultation request")]
        public void GivenIHaveAValidRequestPrivateConsultationRequest()
        {
            var individual = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Individual).Id;
            var representative = _context.Test.ConferenceResponse.Participants
                .First(x => x.UserRole == UserRole.Representative).Id;

            var request = new ConsultationRequestResponse()
            {
                ConferenceId = _context.Test.ConferenceResponse.Id,
                RequestedBy = individual,
                RequestedFor = representative,
                Answer = ConsultationAnswer.Accepted
            };
            _context.Request = _context.Post(HandleConsultationRequest, request);
        }
    }
}
