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
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;
using static Testing.Common.Helper.ApiUriFactory.ParticipantsEndpoints;
using TestContext = VideoApi.IntegrationTests.Contexts.TestContext;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantSteps : BaseSteps
    {
        private readonly TestContext _context;

        public ParticipantSteps(TestContext c)
        {
            _context = c;
        }

        [Given(@"I have an add participant to a (.*) conference request")]
        [Given(@"I have an add participant to an (.*) conference request")]
        public void GivenIHaveAnAddParticipantToConferenceRequest(Scenario scenario)
        {
            Guid conferenceId;
            AddParticipantsToConferenceRequest request;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    conferenceId = _context.Test.Conference.Id;
                    request = new AddParticipantsToConferenceRequest
                    {
                        Participants = new List<ParticipantRequest>
                            {new ParticipantRequestBuilder(UserRole.Individual).Build()}
                    };
                    break;
                }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    request = new AddParticipantsToConferenceRequest
                    {
                        Participants = new List<ParticipantRequest>
                            {new ParticipantRequestBuilder(UserRole.Individual).Build()}
                    };
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    request = new AddParticipantsToConferenceRequest
                    {
                        Participants = new List<ParticipantRequest>
                            {new ParticipantRequestBuilder(UserRole.Individual).Build()}
                    };
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

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
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    request = new UpdateParticipantRequest
                    {
                        Fullname = "Automation_Mr Test_Fullname",
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = UpdateParticipantFromConference(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Patch;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an add participant to a conference request with a (.*) body")]
        [Given(@"I have an add participant to a conference request with an (.*) body")]
        public void GivenIHaveAnAddParticipantToConferenceRequestWith(Scenario scenario)
        {
            var request = scenario switch
            {
                Scenario.Invalid => new AddParticipantsToConferenceRequest
                {
                    Participants = new List<ParticipantRequest>()
                },
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

            _context.Uri = AddParticipantsToConference(_context.Test.Conference.Id);
            _context.HttpMethod = HttpMethod.Put;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
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
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(_context.Test.Conference.Id, _context.Test.Conference.GetParticipants()[0].Id,
                    1,2,3,4,5,6,7,8, DateTime.UtcNow.AddMinutes(5), "chrome", "1"),
                new Heartbeat(_context.Test.Conference.Id, _context.Test.Conference.GetParticipants()[0].Id,
                    8,7,6,5,4,3,2,1, DateTime.UtcNow.AddMinutes(2), "chrome", "1"),
                new Heartbeat(_context.Test.Conference.Id, _context.Test.Conference.GetParticipants()[0].Id,
                    5456,4495,5642,9795,5653,8723,4242,3343, DateTime.UtcNow.AddMinutes(1), "chrome", "1")
            };

            await _context.TestDataManager.SeedHeartbeats(heartbeats);
        }

        [Given(@"I have a participant with heartbeat data")]
        public void GivenIHaveAParticipantWithHeartbeatData()
        {
        }

        [Then(@"(.*) heartbeats should be retrieved")]
        [Then(@"(.*) heartbeat should be retrieved")]
        public async Task ThenTheHeartbeatsShouldBeRetrieved(int count)
        {
            var conferenceId = _context.Test.Conference.Id;
            var participantId = _context.Test.Conference.GetParticipants()[0].Id;

            _context.Uri = GetHeartbeats(conferenceId, participantId);
            _context.HttpMethod = HttpMethod.Get;
            _context.ResponseMessage = await SendGetRequestAsync(_context);
            _context.ResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            _context.ResponseMessage.IsSuccessStatusCode.Should().Be(true);

            var json = await _context.ResponseMessage.Content.ReadAsStringAsync();
            var result = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantHeartbeatResponse>>(json);
            result.Should().NotBeNullOrEmpty().And.NotContainNulls();
            result.Should().HaveCount(count);
            result.Should().BeInAscendingOrder(x => x.Timestamp);
            result.Should().ContainItemsAssignableTo<ParticipantHeartbeatResponse>();
        }


        [Given(@"I have a valid get heartbeats request")]
        public void GivenIHaveAValidGetHeartbeatsRequest()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have a get heartbeats request with a nonexistent conference id")]
        public void GivenIHaveAGetHeartbeatsRequestWithANonexistentConferenceId()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have a valid set heartbeats request")]
        public void GivenIHaveAValidSetHeartbeatsRequest()
        {
            var participantId = _context.Test.Conference.GetParticipants()[0].Id;

            _context.Uri = SetHeartbeats(_context.Test.Conference.Id, participantId);
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(new AddHeartbeatRequest { BrowserName = "firefox" });
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an invalid set heartbeats request")]
        public void GivenIHaveAnInvalidSetHeartbeatsRequest()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have a set heartbeats request with a nonexistent conference id")]
        public void GivenIHaveASetHeartbeatsRequestWithANonexistentConferenceId()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"(.*) heartbeats should be retrieved")]
        public void ThenHeartbeatsShouldBeRetrievedScenarioGetHeartbeatsNotFoundWithNoHeartbeats(int p0)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
