using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using static Testing.Common.Helper.ApiUriFactory.VirtualRoomEndpoints;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class VirtualRoomSteps : BaseSteps
    {
        private readonly TestContext _context;

        public VirtualRoomSteps(TestContext context)
        {
            _context = context;
        }
        
        [Given(@"I have a get interpreter room request")]
        public void GivenIHaveAGetInterpreterRoomRequest()
        {
            var conference = _context.Test.Conference;
            var participant = conference.Participants.First(x => !x.IsJudge());
            _context.Uri = GetInterpreterRoomForParticipant(conference.Id, participant.Id);
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get interpreter room request for a non-existent conference")]
        public void GivenIHaveAGetInterpreterRoomRequestNonExistentConference()
        {
            _context.Uri = GetInterpreterRoomForParticipant(Guid.NewGuid(), Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get interpreter room request for a non-existent participant")]
        public void GivenIHaveAGetInterpreterRoomRequestNonExistentParticipant()
        {
            var conference = _context.Test.Conference;
            _context.Uri = GetInterpreterRoomForParticipant(conference.Id,Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get witness room request")]
        public void GivenIHaveAGetWitnessRoomRequest()
        {
            var conference = _context.Test.Conference;
            var participant = conference.Participants.First(x => !x.IsJudge());
            _context.Uri = GetWitnessRoomForParticipant(conference.Id, participant.Id);
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get witness room request for a non-existent conference")]
        public void GivenIHaveAGetWitnessRoomRequestForANon_ExistentConference()
        {
            _context.Uri = GetWitnessRoomForParticipant(Guid.NewGuid(), Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get witness room request for a non-existent participant")]
        public void GivenIHaveAGetWitnessRoomRequestForANon_ExistentParticipant()
        {
            var conference = _context.Test.Conference;
            _context.Uri = GetWitnessRoomForParticipant(conference.Id,Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get judicial office holder room request")]
        public void GivenIHaveAGetJudicialOfficeHolderRoomRequest()
        {
            var conference = _context.Test.Conference;
            var participant = conference.Participants.First(x => x.UserRole == UserRole.JudicialOfficeHolder);
            _context.Uri = GetJudicialRoomForParticipant(conference.Id, participant.Id);
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Then(@"the response should have connection details for the room")]
        public async Task ThenTheResponseShouldHaveConnectionDetailsForTheRoom()
        {
            var json = await _context.Response.Content.ReadAsStringAsync();
            var room = RequestHelper.Deserialise<SharedParticipantRoomResponse>(json);
            room.PexipNode.Should().NotBeNullOrWhiteSpace();
            room.ParticipantJoinUri.Should().NotBeNullOrWhiteSpace();
        }
    }
}
