using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;
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
            var individual = _context.Test.ConferenceResponse.Participants.Find(x => x.UserRole.Equals(UserRole.Individual)).Id;
            var representative = _context.Test.ConferenceResponse.Participants
                .Find(x => x.UserRole.Equals(UserRole.Representative)).Id;

            var request = new ConsultationRequest()
            {
                ConferenceId = _context.Test.ConferenceResponse.Id,
                RequestedBy = individual,
                RequestedFor = representative,
                Answer = ConsultationAnswer.Accepted
            };
            _context.Request = _context.Post(HandleConsultationRequest, request);
        }
        
        [Given(@"I have a valid consultation request as a Judge")]
        public void GivenIHaveAValidConsultationRequestAsAJudge()
        {
            var judge = _context.Test.ConferenceResponse.Participants
                .Find(x => x.UserRole.Equals(UserRole.Individual)).Id;
            
            var request = new StartConsultationRequest()
            {
                ConferenceId = _context.Test.ConferenceResponse.Id,
                RequestedBy = judge,
                RoomType = RoomType.ConsultationRoom1
            };
            _context.Request = _context.Post(HandleConsultationRequest, request);
        }
    }
}
