using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using Faker;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using static Testing.Common.Helper.ApiUriFactory.EPEndpoints;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class EndpointSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly CommonSteps _commonSteps;
        
        public EndpointSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
        }
        
        [Given(@"I have a get endpoints request for a nonexistent conference")]
        public void GivenIHaveARequestForEndpointsForNonexistentConference()
        {
            SetupGetEndpointRequest(Guid.NewGuid());
        }
        
        [Given(@"I have a conference with no endpoints")]
        public async Task GivenIHaveAConferenceWithNoEndpoints()
        {
            await _commonSteps.GivenIHaveAConference();
        }

        [Given(@"I have a conference with endpoints and endpoint defence advocate is in a consultation")]
        public async Task GivenTheDefenceAdvocateIsInConsultationRoom()
        {
            var conference1 = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Applicant", "rep@hmcts.net")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithEndpoint("Display1", Internet.FreeEmail(), "rep@hmcts.net")
                .WithEndpoint("Display2", Internet.FreeEmail())
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .WithAudioRecordingRequired(false).Build();

            var consultationRoom = new ConsultationRoom(conference1.Id, "name", VirtualCourtRoomType.JudgeJOH, false);
            var defenseAdvocate = conference1.Participants.Single(x => x.Username == "rep@hmcts.net");
            defenseAdvocate.UpdateCurrentConsultationRoom(consultationRoom);

            _context.Test.Conference = await _context.TestDataManager.SeedConference(conference1);
            _context.Test.Conferences.Add(_context.Test.Conference);
        }

        [Given(@"I have a conference with endpoints")]
        public async Task GivenIHaveAConferenceWithEndpoints()
        {
            var conference1 = new ConferenceBuilder()
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Representative, "Applicant", "rep@hmcts.net")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithEndpoint("Display1", Internet.FreeEmail(), "rep@hmcts.net")
                .WithEndpoint("Display2", Internet.FreeEmail())
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .WithAudioRecordingRequired(false).Build();
            _context.Test.Conference = await _context.TestDataManager.SeedConference(conference1);
            _context.Test.Conferences.Add(_context.Test.Conference);
            NUnit.Framework.TestContext.WriteLine($"New seeded conference id: {_context.Test.Conference.Id}");
        }
        
        [Given(@"I have get endpoints for conference request")]
        public void GivenIHaveAGetEndpointsForAConferenceRequest()
        {
            SetupGetEndpointRequest(_context.Test.Conference.Id);
        }
        
        [Given(@"I have an add endpoint to conference request")]
        [Then(@"I have an add endpoint to conference request")]
        public void GivenIHaveAValidAddEndpointRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            var request = new AddEndpointRequest
            {
                Pin = "1234",
                SipAddress = $"{Guid.NewGuid()}@sip.com",
                DisplayName = "Automated Add EP test", 
                DefenceAdvocate = "Defence Sol"
            };
            SetupAddEndpointRequest(conferenceId, request);
        }
        
        [Given(@"I have an add endpoint to a non-existent conference request")]
        public void GivenIHaveAnNonExistentAddEndpointRequest()
        {
            var conferenceId = Guid.NewGuid();
            var request = new AddEndpointRequest
            {
                Pin = "1234",
                SipAddress = "1234add_auto_test@sip.com",
                DisplayName = "Automated Add EP test",
                DefenceAdvocate = "Defence Sol"
            };
            SetupAddEndpointRequest(conferenceId, request);
        }
        
        [Given(@"I have an invalid add endpoint to conference request")]
        public void GivenIHaveAnInvalidAddEndpointRequest()
        {
            var conferenceId = Guid.NewGuid();
            var request = new AddEndpointRequest
            {
                SipAddress = string.Empty,
                DisplayName = string.Empty
            };
            SetupAddEndpointRequest(conferenceId, request);
        }
        
        [Given(@"I have remove endpoint from a conference request")]
        public async Task GivenIHaveRemoveEndpointFromAConferenceRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            string sipAddress;
            await using (var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions))
            {
                var conf = await db.Conferences.Include(x => x.Endpoints).SingleAsync(x => x.Id == conferenceId);
                sipAddress = conf.Endpoints[0].SipAddress;
            }
            
            SetupRemoveEndpointRequest(conferenceId, sipAddress);
        }
        
        [Given(@"I have remove non-existent endpoint from a conference request")]
        public void GivenIHaveRemoveNon_ExistentEndpointFromAConferenceRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            var sipAddress = "sip@sip.com";
            SetupRemoveEndpointRequest(conferenceId, sipAddress);
        }
        
        [Then(@"the endpoint response should be (.*)")]
        public async Task ThenTheEndpointResponseShould(int number)
        {
            await AssertEndpointLength(number);
        }
        
        [Then(@"the endpoint response should not be empty")]
        public  async Task  ThenTheEndpointResponseShouldNotBeEmpty()
        {
            await AssertEndpointLength(_context.Test.Conference.Endpoints.Count);
        }
        
        [Then(@"the endpoint response should be empty")]
        public async Task ThenTheEndpointResponseShouldBeEmpty()
        {
            if (_context.Test.Conference == null)
            {
                await AssertEndpointLength(0);
            }
            else
            {
                await AssertEndpointLength(_context.Test.Conference.Endpoints.Count);
            }
        }

        [Given(@"I have update to a non-existent endpoint for a conference request")]
        public void GivenIHaveUpdateToANon_ExistentEndpointForAConferenceRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            var sipAddress = "sip@sip.com";
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Automated Add EP test"
            };
            SetupUpdateEndpointRequest(conferenceId, sipAddress, request);
        }

        [Given(@"I have update endpoint for a conference request")]
        public void GivenIHaveUpdateEndpointForAConferenceRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            var sipAddress = _context.Test.Conference.Endpoints.First().SipAddress;
            var request = new UpdateEndpointRequest
            {
                DisplayName = "Automated Add EP test",
                DefenceAdvocate = "Sol One"
            };
            SetupUpdateEndpointRequest(conferenceId, sipAddress, request);
        }

        [Then(@"the endpoint status should be (.*)")]
        public async Task ThenTheEndpointsStateShouldBe(EndpointState state)
        {
            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var conf = await db.Conferences.Include(x => x.Endpoints)
                .SingleAsync(x => x.Id == _context.Test.Conference.Id);
            var endpoint = conf.GetEndpoints().First(x => x.Id == _context.Test.ParticipantId);
            endpoint.State.Should().Be(state);
        }
        
        private async Task AssertEndpointLength(int length)
        {
            var result = await ApiClientResponse.GetResponses<IList<EndpointResponse>>(_context.Response.Content);
            result.Should().HaveCount(length);
        }

        private void SetupGetEndpointRequest(Guid conferenceId)
        {
            _context.Uri = GetEndpointsForConference(conferenceId);
            _context.HttpMethod = HttpMethod.Get;
        }
        
        private void SetupAddEndpointRequest(Guid conferenceId, AddEndpointRequest request)
        {
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _context.Uri = AddEndpointsToConference(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
        }
        
        private void SetupRemoveEndpointRequest(Guid conferenceId, string sipAddress)
        {
            _context.Uri = RemoveEndpointsFromConference(conferenceId, sipAddress);
            _context.HttpMethod = HttpMethod.Delete;
        }

        private void SetupUpdateEndpointRequest(Guid conferenceId, string sipAddress, UpdateEndpointRequest request)
        {
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _context.Uri = UpdateEndpoint(conferenceId, sipAddress);
            _context.HttpMethod = HttpMethod.Patch;
        }
    }
}
