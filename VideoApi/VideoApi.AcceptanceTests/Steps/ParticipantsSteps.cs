using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Assertions;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Contract.Enums;
using static Testing.Common.Helper.ApiUriFactory.ConferenceEndpoints;
using static Testing.Common.Helper.ApiUriFactory.ParticipantsEndpoints;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private readonly CommonSteps _commonSteps;
        private const string ParticipantUsernameKey = "ParticipantUsername";
        private const string RequestBodyKey = "RequestBodyKey";
        private const decimal LossPercentage = 0.25m;

        public ParticipantsSteps(TestContext injectedContext, ScenarioContext scenarioContext, CommonSteps commonSteps)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
            _commonSteps = commonSteps;
        }

        [Given(@"I have an add participant to a valid conference request")]
        public void GivenIHaveAnAddParticipantToAValidConferenceRequest()
        {
            var request = new AddParticipantsToConferenceRequest
            {
                Participants = new List<ParticipantRequest> {new ParticipantRequestBuilder(UserRole.Individual).Build()}
            };
            _scenarioContext.Add(ParticipantUsernameKey, request.Participants.First().Username);
            _context.Request = _context.Put(AddParticipantsToConference(_context.Test.ConferenceResponse.Id), request);
        }
        
        [Given(@"I have a request to add two linked participants")]
        public void GivenIHaveARequestToAddTwoLinkedParticipants()
        {
            var participantA = new ParticipantRequestBuilder(UserRole.Individual).Build();
            var participantB = new ParticipantRequestBuilder(UserRole.Individual).Build();
            
            participantA.LinkedParticipants.Add(new LinkedParticipantRequest()
            {
                Type = LinkedParticipantType.Interpreter,
                LinkedRefId = participantB.ParticipantRefId,
                ParticipantRefId = participantA.ParticipantRefId
            });
            
            participantB.LinkedParticipants.Add(new LinkedParticipantRequest()
            {
                Type = LinkedParticipantType.Interpreter,
                LinkedRefId = participantA.ParticipantRefId,
                ParticipantRefId = participantB.ParticipantRefId
            });
            
            var request = new AddParticipantsToConferenceRequest
            {
                Participants = new List<ParticipantRequest>
                {
                    participantA,
                    participantB
                },
            };
            
            _scenarioContext.Add(ParticipantUsernameKey, request.Participants.First().Username);
            _scenarioContext.Add(RequestBodyKey, request);
            _context.Request = _context.Put(AddParticipantsToConference(_context.Test.ConferenceResponse.Id), request);
        }
        
        [Then(@"the linked participants are added")]
        public void ThenTheLinkedParticipantsAreAdded()
        {
            _context.Request = _context.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue();

            var conference = RequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            
            var confParticipants = conference.Participants;
            var linkCount = confParticipants.Sum(x => x.LinkedParticipants.Count);
            linkCount.Should().Be(2);
            
            var request = _scenarioContext.Get<AddParticipantsToConferenceRequest>(RequestBodyKey);
            var participantA = request.Participants[0];
            var participantB = request.Participants[1];
            
            // verify correct links have been added
            var participantAFromContext = confParticipants.Single(x => x.RefId == participantA.ParticipantRefId);
            var participantBFromContext = confParticipants.Single(x => x.RefId == participantB.ParticipantRefId);
            
            participantAFromContext.LinkedParticipants.Should().Contain(x => x.LinkedId == participantBFromContext.Id);
            participantBFromContext.LinkedParticipants.Should().Contain(x => x.LinkedId == participantAFromContext.Id);
        }

        [Given(@"I have an remove participant from a valid conference request")]
        public void GivenIHaveAnRemoveParticipantFromAValidConferenceRequest()
        {
            _scenarioContext.Add(ParticipantUsernameKey, _context.Test.ConferenceResponse.Participants.Last().DisplayName);
            _context.Request = _context.Delete(RemoveParticipantFromConference(_context.Test.ConferenceResponse.Id, _context.Test.ConferenceResponse.Participants.Last().Id));
        }

        [Given(@"I have an update participant details request")]
        public void GivenIHaveAnUpdateParticipantRequest()
        {
            var participant = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Individual);
            var request = new UpdateParticipantRequest()
            {
                Fullname = $"Updated {participant.Name}",
                FirstName = $"Updated {participant.FirstName}",
                LastName = $"Updated {participant.LastName}",
                DisplayName = $"Updated {participant.DisplayName}",
                Representee = $"Updated {participant.Representee}"
            };
            _scenarioContext.Add(ParticipantUsernameKey, participant.Username);
            _context.Request = _context.Patch(UpdateParticipantFromConference(_context.Test.ConferenceResponse.Id, participant.Id), request);
        }

        [Given(@"I have a participant with heartbeat data")]
        public void GivenIHaveSomeHeartbeatData()
        {
            SetHeartbeatDataRequest();
            _commonSteps.WhenISendTheRequestToTheEndpoint();
            _commonSteps.ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode.NoContent, true);
        }

        [Given(@"I have a valid get heartbeat data request")]
        public void GetHeartbeatDataRequest()
        {
            var participantId = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Individual).Id;
            _context.Request = _context.Get(GetHeartbeats(_context.Test.ConferenceResponse.Id, participantId));
        }

        [Given(@"I have a valid set heartbeat data request")]
        public void SetHeartbeatDataRequest()
        {
            var participantId = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Individual).Id;
            var request = new AddHeartbeatRequest()
            {
                OutgoingAudioPercentageLost = LossPercentage,
                OutgoingAudioPercentageLostRecent = LossPercentage,
                IncomingAudioPercentageLost = LossPercentage,
                IncomingAudioPercentageLostRecent = LossPercentage, 
                OutgoingVideoPercentageLost = LossPercentage, 
                OutgoingVideoPercentageLostRecent = LossPercentage, 
                IncomingVideoPercentageLost = LossPercentage,
                IncomingVideoPercentageLostRecent = LossPercentage,
                BrowserName = "Chrome",
                BrowserVersion = "80.0",
                OperatingSystem = "Mac OS X",
                OperatingSystemVersion = "10.15.7"
            };
            _context.Test.HeartbeatData = request;
            _context.Request = _context.Post(SetHeartbeats(_context.Test.ConferenceResponse.Id, participantId), request);
        }

        [Then(@"the heartbeat data is retrieved")]
        public void ThenTheHeartbeatDataIsRetrieved()
        {
            var heartbeatData = RequestHelper.Deserialise<List<ParticipantHeartbeatResponse>>(_context.Response.Content);
            heartbeatData.First().BrowserName.Should().Be(_context.Test.HeartbeatData.BrowserName);
            heartbeatData.First().BrowserVersion.Should().Be(_context.Test.HeartbeatData.BrowserVersion);
            heartbeatData.First().OperatingSystem.Should().Be(_context.Test.HeartbeatData.OperatingSystem);
            heartbeatData.First().OperatingSystemVersion.Should().Be(_context.Test.HeartbeatData.OperatingSystemVersion);
            heartbeatData.First().RecentPacketLoss.Should().Be(LossPercentage);
            heartbeatData.First().Timestamp.Minute.Should().BeOneOf(DateTime.Now.Minute, DateTime.Now.AddMinutes(-1).Minute);
        }

        [Then(@"the participant is (.*)")]
        public void ThenTheParticipantIsAdded(string state)
        {
            _context.Request = _context.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue();
            var conference = RequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            var exists = conference.Participants.Any(participant =>
                participant.Username.ToLower().Equals(_scenarioContext.Get<string>(ParticipantUsernameKey).ToLower()));
            if (state.Equals("added"))
            {
                exists.Should().BeTrue();
            }
            else if (state.Equals("removed"))
            {
                exists.Should().BeFalse();
            }
            else if (state.Equals("updated"))
            {
                var participant = conference.Participants.First(x =>
                    x.Username.Equals(_scenarioContext.Get<string>(ParticipantUsernameKey)));

                participant.Name.Should().Contain("Updated");
                participant.FirstName.Should().Contain("Updated");
                participant.LastName.Should().Contain("Updated");
                participant.DisplayName.Should().Contain("Updated");
                participant.Representee.Should().Contain("Updated");
            }
        }

        [Given(@"I have a valid get judge names data request")]
        public void GetJudgeNamesDataRequest()
        {
            _context.Request = _context.Get(GetDistinctJudgeNames());
        }

        [Then(@"the judge names should be retrieved")]
        public void ThenTheJudgeNamesShouldBeRetrieved()
        {
            var judgesList = RequestHelper.Deserialise<JudgeNameListResponse>(_context.Response.Content).FirstNames;
            var conferences = _context.Test.ConferenceDetailsResponses;

            var judges = conferences.SelectMany(c => c.Participants).Where(p => p.UserRole == UserRole.Judge).ToList();
            judges.Should().NotBeNull();
            judges.Count.Should().BeGreaterOrEqualTo(4);
            judgesList.Count.Should().BeGreaterOrEqualTo(2);
            judgesList.Should().NotBeNull();

            foreach (var judge in judges)
            {
                judgesList.Count(x => x.Contains(judge.FirstName)).Should().Be(1);
            }
        }

        [Given(@"I have a get participants for a participants request with a conference id")]
        public void GivenIHaveAGetParticipantsForConferenceRequest()
        {
            _context.Request = _context.Get(GetParticipantsByConferenceId(_context.Test.ConferenceResponse.Id));
        }

        [Then(@"the participants should be retrieved")]
        public void ThenTheParticipantsShouldBeRetrieved()
        {
            var participants = RequestHelper.Deserialise<List<ParticipantSummaryResponse>>(_context.Response.Content);
            participants.Should().NotBeNull();
            AssertParticipantSummaryResponse.ForParticipant(participants[1]);
        }
    }
}
