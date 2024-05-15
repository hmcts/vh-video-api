using System.Linq;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using static Testing.Common.Helper.ApiUriFactory.VirtualRoomEndpoints;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public class VirtualRoomSteps
    {
        private readonly TestContext _context;
        
        public VirtualRoomSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }
        
        [Given(@"I have a get interpreter room request")]
        public void GivenIHaveAGetInterpreterRoomRequest()
        {
            var conference = _context.Test.ConferenceResponse;
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);
            
            _context.Request = _context.Get(GetInterpreterRoomForParticipant(conference.Id, participant.Id));
        }
        
        [Given(@"I have a get witness room request")]
        public void GivenIHaveAGetWitnessRoomRequest()
        {
            var conference = _context.Test.ConferenceResponse;
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);
            
            _context.Request = _context.Get(GetWitnessRoomForParticipant(conference.Id, participant.Id));
        }
        
        [Given(@"I have a get judicial room request")]
        public void GivenIHaveAGetJudicialRoomRequest()
        {
            var conference = _context.Test.ConferenceResponse;
            var participant = conference.Participants.First(x => x.UserRole == UserRole.JudicialOfficeHolder);
            
            _context.Request = _context.Get(GetJudicialRoomForParticipant(conference.Id, participant.Id));
        }
        
        [Then(@"the response should have connection details for the room")]
        public void ThenTheResponseShouldHaveConnectionDetailsForTheRoom()
        {
            var interpreterRoom = ApiRequestHelper.Deserialise<SharedParticipantRoomResponse>(_context.Response.Content);
            interpreterRoom.Should().NotBeNull();
            interpreterRoom.PexipNode.Should().NotBeNullOrWhiteSpace();
            interpreterRoom.ParticipantJoinUri.Should().NotBeNullOrWhiteSpace();
        }
    }
}
