using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;
using static Testing.Common.Helper.ApiUriFactory.ParticipantsEndpoints;
using TestContext = VideoApi.IntegrationTests.Contexts.TestContext;
using Testing.Common.Assertions;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly CommonSteps _commonSteps;

        public ParticipantSteps(TestContext c, CommonSteps commonSteps)
        {
            _context = c;
            _commonSteps = commonSteps;
        }

        [Given(@"I have an add participant to a (.*) conference request")]
        [Given(@"I have an add participant to an (.*) conference request")]
        public void GivenIHaveAnAddParticipantToConferenceRequest(Scenario scenario)
        {
            var request = new AddParticipantsToConferenceRequest()
            {
                Participants = new List<ParticipantRequest>
                    {new ParticipantRequestBuilder(UserRole.Individual).Build()}
            };

            var conferenceId = scenario switch
            {
                Scenario.Valid => _context.Test.Conference.Id,
                Scenario.Nonexistent => Guid.NewGuid(),
                Scenario.Invalid => Guid.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

            _context.Uri = AddParticipantsToConference(conferenceId);
            _context.HttpMethod = HttpMethod.Put;
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an update participant to a (.*) conference request")]
        public void GivenIHaveAnUpdateParticipantToConferenceRequest(Scenario scenario)
        {
            Guid conferenceId;
            UpdateParticipantRequest request;
            var participantId = Guid.Empty; 
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    conferenceId = _context.Test.Conference.Id; 
                    participantId = _context.Test.Conference.Participants.First().Id;
                    request = new UpdateParticipantRequest
                    {
                        Fullname = "Automation_Mr Test_Fullname",
                        FirstName = "Automation_Mr Test_Firstname",
                        LastName = "Automation_Mr Test_Lastname",
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;
                }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    participantId = Guid.NewGuid();
                    request = new UpdateParticipantRequest
                    {
                        Fullname = "Automation_Mr Test_Fullname",
                        FirstName = "Automation_Mr Test_Firstname",
                        LastName = "Automation_Mr Test_Lastname",
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    request = new UpdateParticipantRequest
                    {
                        Fullname = "Automation_Mr Test_Fullname",
                        FirstName = "Automation_Mr Test_Firstname",
                        LastName = "Automation_Mr Test_Lastname",
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = UpdateParticipantFromConference(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Patch;
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an add participant to a conference request with a (.*) body")]
        [Given(@"I have an add participant to a conference request with an invalid body")]
        public void GivenIHaveAnAddParticipantToConferenceRequestWithInvalidBody()
        {
            var request = new AddParticipantsToConferenceRequest
            {
                Participants = new List<ParticipantRequest>()
            };
            _context.Uri = AddParticipantsToConference(_context.Test.Conference.Id);
            _context.HttpMethod = HttpMethod.Put;
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an remove participant from a (.*) conference request")]
        [Given(@"I have an remove participant from an (.*) conference request")]
        public void GivenIHaveAnRemoveParticipantFromConferenceRequest(Scenario scenario)
        {
            Guid conferenceId;
            Guid participantId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    conferenceId = _context.Test.Conference.Id;
                    participantId = _context.Test.Conference.GetParticipants().First().Id;
                    break;
                }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    participantId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    participantId = Guid.Empty;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = RemoveParticipantFromConference(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a remove participant from a conference request for a (.*) participant")]
        [Given(@"I have a remove participant from a conference request for an (.*) participant")]
        public void GivenIHaveAnRemoveParticipantFromConferenceRequestWithParticipant(Scenario scenario)
        {
            var participantId = scenario switch
            {
                Scenario.Nonexistent => Guid.NewGuid(),
                Scenario.Invalid => Guid.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

            _context.Uri = RemoveParticipantFromConference(_context.Test.Conference.Id, participantId);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given("I have a (.*) get self test score request")]
        public void GivenIHaveAGetSelfTestScoreRequest(Scenario scenario)
        {
            var participantId = scenario switch
            {
                Scenario.Nonexistent => _context.Test.Conference.Id,
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

            _context.Uri = GetTestCallResultForParticipant(_context.Test.Conference.Id, participantId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a participant with heartbeat data")]
        public async Task GivenIHaveHeartbeats()
        {
            var participantId = _context.Test.Conference.GetParticipants()[0].Id;
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(_context.Test.Conference.Id, participantId,
                    1,2,3,4,5,6,7,8, DateTime.UtcNow.AddMinutes(5), "chrome", "1"),
                new Heartbeat(_context.Test.Conference.Id, participantId,
                    8,7,6,5,4,3,2,1, DateTime.UtcNow.AddMinutes(2), "chrome", "1"),
                new Heartbeat(_context.Test.Conference.Id, participantId,
                    5456,4495,5642,9795,5653,8723,4242,3343, DateTime.UtcNow.AddMinutes(1), "chrome", "1")
            };

            await _context.TestDataManager.SeedHeartbeats(heartbeats);
        }

        [Given(@"I have a valid get heartbeats request")]
        public void GetHeartbeatsRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            var participantId = _context.Test.Conference.GetParticipants()[0].Id;
            _context.Uri = GetHeartbeats(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get heartbeats request with a nonexistent conference id")]
        public void GivenIHaveAGetHeartbeatsRequestWithANonexistentConferenceId()
        {
            var conferenceId = Guid.NewGuid();
            var participantId = _context.Test.Conference.GetParticipants()[0].Id;
            _context.Uri = GetHeartbeats(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get heartbeats request with a nonexistent participant id")]
        public void GivenIHaveAGetHeartbeatsRequestWithANonexistentParticipantId()
        {
            var conferenceId = _context.Test.Conference.Id;
            var participantId = Guid.NewGuid();
            _context.Uri = GetHeartbeats(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid set heartbeats request")]
        public void GivenIHaveAValidSetHeartbeatsRequest()
        {
            var participantId = _context.Test.Conference.GetParticipants()[0].Id;
            _context.Uri = SetHeartbeats(_context.Test.Conference.Id, participantId);
            _context.HttpMethod = HttpMethod.Post;
            _context.HttpContent = RequestBody.Set(new AddHeartbeatRequest { BrowserName = "firefox" });
        }

        [Given(@"I have a set heartbeats request with a nonexistent participant id")]
        public void GivenIHaveASetHeartbeatsRequestWithANonexistentParticipantId()
        {
            var participantId = Guid.NewGuid();
            _context.Uri = SetHeartbeats(_context.Test.Conference.Id, participantId);
            _context.HttpMethod = HttpMethod.Post;
            _context.HttpContent = RequestBody.Set(new AddHeartbeatRequest { BrowserName = "firefox" });
        }

        [Given(@"I have a set heartbeats request with a nonexistent conference id")]
        public void GivenIHaveASetHeartbeatsRequestWithANonexistentConferenceId()
        {
            var participantId = _context.Test.Conference.GetParticipants()[0].Id;
            var conferenceId = Guid.NewGuid();
            _context.Uri = SetHeartbeats(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Post;
            _context.HttpContent = RequestBody.Set(new AddHeartbeatRequest { BrowserName = "firefox" });
        }

        [Given(@"I have a get participants for a participants request with a (.*) conference id")]
        [Given(@"I have a get participants for a participants request with an (.*) conference id")]
        public void GivenIHaveAGetParticipantsForConferenceRequest(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForRequest(scenario);
            _context.Uri = GetParticipantsByConferenceId(conferenceId);
            _context.HttpMethod = HttpMethod.Get;

        }

        private Guid GetConferenceIdForRequest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        conferenceId = _context.Test.Conference.Id;
                        break;
                    }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            return conferenceId;
        }

        [Then(@"the participants should be retrieved")]
        public async Task ThenTheParticipantsShouldBeRetrieved()
        {
            var result = await Response.GetResponses<List<ParticipantSummaryResponse>>(_context.Response.Content);
            result.Should().NotBeNull();
            AssertParticipantSummaryResponse.ForParticipant(result[1]);
        }


        [Then(@"(.*) heartbeats should be retrieved")]
        [Then(@"(.*) heartbeat should be retrieved")]
        public async Task ThenTheHeartbeatsShouldBeRetrieved(int count)
        {
            var result = await Response.GetResponses<List<ParticipantHeartbeatResponse>>(_context.Response.Content);
            result.Should().HaveCount(count);
            if (count > 0)
            {
                result.Should().BeInAscendingOrder(x => x.Timestamp);
                result.Should().ContainItemsAssignableTo<ParticipantHeartbeatResponse>();
            }
        }

        [Then(@"the heartbeats should be saved")]
        public async Task ThenTheHeartbeatsShouldBeSaved()
        {
            GetHeartbeatsRequest();
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _commonSteps.ThenTheResponseShouldHaveStatus(HttpStatusCode.OK, true);
            await ThenTheHeartbeatsShouldBeRetrieved(1);
        }
    }
}
