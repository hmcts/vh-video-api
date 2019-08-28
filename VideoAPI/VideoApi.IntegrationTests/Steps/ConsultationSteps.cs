using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.DAL;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class ConsultationSteps : StepsBase
    {
        private readonly ConferenceTestContext _conferenceTestContext;
        private readonly ConsultationEndpoints _endpoints = new ApiUriFactory().ConsultationEndpoints;

        public ConsultationSteps(ApiTestContext apiTestContext, ConferenceTestContext conferenceTestContext) : base(
            apiTestContext)
        {
            _conferenceTestContext = conferenceTestContext;
        }

        [Given(@"I have a (.*) raise consultation request")]
        [Given(@"I have an (.*) raise consultation request")]
        public void GivenIHaveARaiseConsultationRequest(Scenario scenario)
        {
            var request = SetupConsultationRequest(false);
            switch (scenario)
            {
                case Scenario.Valid: break;
                case Scenario.Invalid:
                {
                    request.ConferenceId = Guid.Empty;
                    request.RequestedFor = Guid.Empty;
                    request.RequestedBy = Guid.Empty;
                    break;
                }

                case Scenario.Nonexistent:
                {
                    request.ConferenceId = Guid.NewGuid();
                }
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            SerialiseConsultationRequest(request);
        }

        [Given(@"I have a raise consultation request with an invalid (.*)")]
        public void GivenIHaveARaiseConsultationRequestWithAnInvalidParticipant(string participant)
        {
            var request = SetupConsultationRequest(false);
            switch (participant)
            {
                case "requestedBy":
                    request.RequestedBy = Guid.NewGuid();
                    break;

                case "requestedFor":
                    request.RequestedFor = Guid.NewGuid();
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(participant), participant, null);
            }

            SerialiseConsultationRequest(request);
        }

        [Given(@"I have a (.*) respond consultation request")]
        [Given(@"I have an (.*) respond consultation request")]
        public void GivenIHaveARespondConsultationRequest(Scenario scenario)
        {
            var request = SetupConsultationRequest(true);
            switch (scenario)
            {
                case Scenario.Valid: break;
                case Scenario.Invalid:
                {
                    request.ConferenceId = Guid.Empty;
                    request.RequestedFor = Guid.Empty;
                    request.RequestedBy = Guid.Empty;
                    request.Answer = ConsultationAnswer.None;
                    break;
                }

                case Scenario.Nonexistent:
                {
                    request.ConferenceId = Guid.NewGuid();
                }
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            SerialiseConsultationRequest(request);
        }

        [Given(@"I have an (.*) leave consultation request")]
        public async Task GivenIHaveALeaveConsultationRequest(Scenario scenario)
        {
            var request = await SetupLeaveConsultationRequest(true).ConfigureAwait(false);
            switch (scenario)
            {
                case Scenario.Valid:
                    break;
                case Scenario.Invalid:
                    request.ConferenceId = Guid.Empty;
                    request.ParticipantId = Guid.Empty;
                    break;
                case Scenario.Nonexistent:
                    request.ConferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            SerialiseLeaveConsultationRequest(request);
        }
        
        [Given(@"I have a leave consultation request for a nonexistent participant")]
        public async Task GivenIHaveALeaveConsultationRequestForANonexistentParticipant()
        {
            var request = await SetupLeaveConsultationRequest(true).ConfigureAwait(false);
            request.ParticipantId = Guid.NewGuid();
            SerialiseLeaveConsultationRequest(request);
        }
        
        [Given(@"I have a leave consultation request for a participant not in a consultation")]
        public async Task GivenIHaveALeaveConsultationRequestForAParticipantNotInAConsultation()
        {
            var request = await SetupLeaveConsultationRequest(false).ConfigureAwait(false);
            SerialiseLeaveConsultationRequest(request);
        }

        [Given(@"I have a respond consultation request with an invalid (.*)")]
        public void GivenIHaveARespondConsultationRequestWithAnInvalidParticipant(string participant)
        {
            var request = SetupConsultationRequest(true);
            switch (participant)
            {
                case "requestedBy":
                    request.RequestedBy = Guid.NewGuid();
                    break;

                case "requestedFor":
                    request.RequestedFor = Guid.NewGuid();
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(participant), participant, null);
            }

            SerialiseConsultationRequest(request);
        }

        [Given("no consultation rooms are available")]
        public async Task GivenNoConsultationRoomsAreAvailable()
        {
            using (var db = new VideoApiDbContext(ApiTestContext.VideoBookingsDbContextOptions))
            {
                var conference = await db.Conferences
                    .Include("Participants")
                    .SingleAsync(x => x.Id == _conferenceTestContext.SeededConference.Id);

                conference.Participants[0].UpdateCurrentRoom(RoomType.ConsultationRoom1);
                conference.Participants[1].UpdateCurrentRoom(RoomType.ConsultationRoom2);

                await db.SaveChangesAsync();
            }
        }
        
        [Given(@"I have a (.*) respond to admin consultation request")]
        [Given(@"I have an (.*) respond to admin consultation request")]
        public void GivenIHaveARespondToAdminConsultationRequest(Scenario scenario)
        {
            var request = SetupRespondToAdminConsultationRequest();
            switch (scenario)
            {
                case Scenario.Valid: break;
                case Scenario.Invalid:
                {
                    request.ConferenceId = Guid.Empty;
                    request.Answer = ConsultationAnswer.None;
                    request.ParticipantId = Guid.Empty;
                    request.ConsultationRoom = RoomType.HearingRoom;
                    break;
                }

                case Scenario.Nonexistent:
                {
                    request.ConferenceId = Guid.NewGuid();
                }
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            SerialiseRespondToAdminConsultationRequest(request);
        }

        [Given(@"I have a respond to admin consultation request with a non-existent participant")]
        public void GivenIHaveARespondToAdminConsultationRequestWithNonExistentParticipant()
        {
            var request = SetupRespondToAdminConsultationRequest();
            request.ParticipantId = Guid.NewGuid();

            SerialiseRespondToAdminConsultationRequest(request);
        }

        private void SerialiseConsultationRequest(ConsultationRequest request)
        {
            ApiTestContext.Uri = _endpoints.HandleConsultationRequest;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        private void SerialiseRespondToAdminConsultationRequest(AdminConsultationRequest request)
        {
            ApiTestContext.Uri = _endpoints.RespondToAdminConsultationRequest;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        private void SerialiseLeaveConsultationRequest(LeaveConsultationRequest request)
        {
            ApiTestContext.Uri = _endpoints.LeaveConsultationRequest;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private ConsultationRequest SetupConsultationRequest(bool withAnswer)
        {
            var request = new ConsultationRequest();
            var seededConference = _conferenceTestContext.SeededConference;

            if (seededConference == null) return request;

            var participants = seededConference.GetParticipants().Where(x =>
                x.UserRole == UserRole.Individual || x.UserRole == UserRole.Representative).ToList();

            request.ConferenceId = seededConference.Id;
            request.RequestedBy = participants[0].Id;
            request.RequestedFor = participants[1].Id;

            if (withAnswer)
            {
                request.Answer = ConsultationAnswer.Accepted;
            }

            return request;
        }

        private async Task<LeaveConsultationRequest> SetupLeaveConsultationRequest(bool inConsultationRoom)
        {
            var request = new LeaveConsultationRequest();

            var seededConference = _conferenceTestContext.SeededConference;

            if (seededConference == null)
            {
                return request;
            }

            request.ConferenceId = seededConference.Id;
            request.ParticipantId = seededConference.Participants[0].Id;

            if (!inConsultationRoom)
            {
                return request;
            }

            var participantId = seededConference.Participants[0].Id;
            using (var db = new VideoApiDbContext(ApiTestContext.VideoBookingsDbContextOptions))
            {
                var conference = await db.Conferences
                    .Include("Participants")
                    .SingleAsync(x => x.Id == _conferenceTestContext.SeededConference.Id);

                conference.Participants.Single(x => x.Id == participantId)
                    .UpdateCurrentRoom(RoomType.ConsultationRoom1);

                await db.SaveChangesAsync();

                request.ParticipantId = participantId;
            }

            return request;
        }

        private AdminConsultationRequest SetupRespondToAdminConsultationRequest()
        {
            var request = new AdminConsultationRequest();

            var seededConference = _conferenceTestContext.SeededConference;

            if (seededConference == null)
            {
                return request;
            }

            var participants = seededConference.GetParticipants().Where(x =>
                x.UserRole == UserRole.Individual || x.UserRole == UserRole.Representative).ToList();

            request.ConferenceId = seededConference.Id;
            request.ParticipantId = participants[0].Id;
            request.Answer = ConsultationAnswer.Accepted;
            request.ConsultationRoom = RoomType.ConsultationRoom1;

            return request;
        }

    }
}