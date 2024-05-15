using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using static Testing.Common.Helper.ApiUriFactory;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class ConsultationSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ConferenceBaseSteps _conferenceSteps;
        private readonly CommonSteps _commonSteps;

        public ConsultationSteps(TestContext context, ConferenceBaseSteps conferenceSteps, CommonSteps commonSteps)
        {
            _context = context;
            _conferenceSteps = conferenceSteps;
            _commonSteps = commonSteps;
        }

        [Given(@"I have a booked conference")]
        public async Task GivenIHaveABookedConference()
        {
            _conferenceSteps.GivenIHaveABookANewConferenceRequest(Scenario.Valid);
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _commonSteps.ThenTheResponseShouldHaveStatus(HttpStatusCode.Created, true);
            await _conferenceSteps.ThenAConferenceDetailsShouldBeRetrieved();
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
            var request = await SetupLeaveConsultationRequest(true);
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
            var request = await SetupLeaveConsultationRequest(true);
            request.ParticipantId = Guid.NewGuid();
            SerialiseLeaveConsultationRequest(request);
        }
        
        [Given(@"I have a leave consultation request for a participant not in a consultation")]
        public async Task GivenIHaveALeaveConsultationRequestForAParticipantNotInAConsultation()
        {
            var request = await SetupLeaveConsultationRequest(false);
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
        
        [Given(@"I have a (.*) respond to admin consultation request")]
        [Given(@"I have an (.*) respond to admin consultation request")]
        public void GivenIHaveARespondToConsultationRequestResponse(Scenario scenario)
        {
            var request = SetupRespondToConsultationRequestResponse();
            switch (scenario)
            {
                case Scenario.Valid: break;
                case Scenario.Invalid:
                {
                    request.ConferenceId = Guid.Empty;
                    request.Answer = ConsultationAnswer.None;
                    request.RequestedFor = Guid.Empty;
                    request.RequestedBy = Guid.Empty;
                    request.RoomLabel = RoomType.HearingRoom.ToString();
                    break;
                }

                case Scenario.Nonexistent:
                {
                    request.ConferenceId = Guid.NewGuid();
                }
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            SerialiseRespondToConsultationRequestResponse(request);
        }

        [Given(@"I have a respond to admin consultation request with a non-existent participant")]
        public void GivenIHaveARespondToConsultationRequestResponseWithNonExistentParticipant()
        {
            var request = SetupRespondToConsultationRequestResponse();
            request.RequestedFor = Guid.NewGuid();

            SerialiseRespondToConsultationRequestResponse(request);
        }

        [Given(@"I have a start endpoint consultation with a linked defence advocate")]
        public void GivenIHaveAStartEndpointConsultationWithALinkedDefenceAdvocate()
        {
            var conference = _context.Test.Conference;
            var endpoint = _context.Test.Conference.Endpoints.First(x=> !string.IsNullOrEmpty(x.DefenceAdvocate));
            var defenceAdvocate =
                _context.Test.Conference.Participants.First(x => x.Username.Equals(endpoint.DefenceAdvocate));

            var request = new EndpointConsultationRequest
            {
                ConferenceId = conference.Id,
                EndpointId = endpoint.Id,
                RequestedById = defenceAdvocate.Id,
                RoomLabel = "name"
            };

            SerialiseEndpointConsultationRequest(request);
        }
        
        [Given(@"I have a start endpoint consultation without a linked defence advocate")]
        public void GivenIHaveAStartEndpointConsultationWithoutALinkedDefenceAdvocate()
        {
            var conference = _context.Test.Conference;
            var endpoint = _context.Test.Conference.Endpoints.First(x=> string.IsNullOrEmpty(x.DefenceAdvocate));
            var defenceAdvocate =
                _context.Test.Conference.Participants.First(x => x.UserRole == UserRole.Representative);

            var request = new EndpointConsultationRequest
            {
                ConferenceId = conference.Id,
                EndpointId = endpoint.Id,
                RequestedById = defenceAdvocate.Id
            };

            SerialiseEndpointConsultationRequest(request);
        }
        
        [Given(@"I have a start endpoint consultation for a non-existent conference")]
        public void GivenIHaveAStartEndpointConsultationForNonexistentConference()
        {
            var endpoint = _context.Test.Conference.Endpoints.First(x=> !string.IsNullOrEmpty(x.DefenceAdvocate));
            var defenceAdvocate =
                _context.Test.Conference.Participants.First(x => x.Username.Equals(endpoint.DefenceAdvocate));

            var request = new EndpointConsultationRequest
            {
                ConferenceId = Guid.NewGuid(),
                EndpointId = endpoint.Id,
                RequestedById = defenceAdvocate.Id
            };

            SerialiseEndpointConsultationRequest(request);
        }
        
        [Given(@"I have a start endpoint consultation for a non-existent endpoint")]
        public void GivenIHaveAStartEndpointConsultationForNonexistentEndpoint()
        {
            var conference = _context.Test.Conference;
            var request = new EndpointConsultationRequest
            {
                ConferenceId = conference.Id,
                EndpointId = Guid.NewGuid(),
                RequestedById = Guid.NewGuid()
            };

            SerialiseEndpointConsultationRequest(request);
        }
        
        [Given(@"I have a start endpoint consultation for a non-existent defence advocate")]
        public void GivenIHaveAStartEndpointConsultationForNonexistentDefenceAdvocate()
        {
            var conference = _context.Test.Conference;
            var endpoint = _context.Test.Conference.Endpoints.First(x=> !string.IsNullOrEmpty(x.DefenceAdvocate));
            var request = new EndpointConsultationRequest
            {
                ConferenceId = conference.Id,
                EndpointId = endpoint.Id,
                RequestedById = Guid.NewGuid()
            };

            SerialiseEndpointConsultationRequest(request);
        }
        
        [Given(@"I have a start endpoint consultation with a not linked defence advocate")]
        public void GivenIHaveAStartEndpointConsultationWithANotLinkedDefenceAdvocate()
        {
            var conference = _context.Test.Conference;
            var endpoint = _context.Test.Conference.Endpoints.First(x=> !string.IsNullOrEmpty(x.DefenceAdvocate));
            var defenceAdvocate =
                _context.Test.Conference.Participants.First(x => !x.Username.Equals(endpoint.DefenceAdvocate));

            var request = new EndpointConsultationRequest
            {
                ConferenceId = conference.Id,
                EndpointId = endpoint.Id,
                RequestedById = defenceAdvocate.Id
            };

            SerialiseEndpointConsultationRequest(request);
        }

[Given(@"I have a valid start consultation request")]
        public async Task GivenIHaveAValidStartConsultationRequest()
        {
            var conference = await ApiClientResponse.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            var judge =
                conference.Participants.First(x => x.UserRole == Contract.Enums.UserRole.Judge);

            var request = new StartConsultationRequest()
            {
                ConferenceId = conference.Id,
                RequestedBy = judge.Id,
                RoomType =  Contract.Enums.VirtualCourtRoomType.JudgeJOH
            };

            SerialiseStartConsultationRequest(request);
        }
        
        [Given(@"the judge is in the waiting room")]
        public async Task GivenTheJudgeIsInTheWaitingRoom()
        {
            var conferenceResponse = await ApiClientResponse.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            var judgeResponse =
                conferenceResponse.Participants.First(x => x.UserRole == Contract.Enums.UserRole.Judge);

            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var conference = await db.Conferences
                .Include(x=> x.Participants)
                .SingleAsync(x => x.Id == conferenceResponse.Id);

            var judge = conference.Participants.First(x => x.Id == judgeResponse.Id);
            judge.UpdateCurrentRoom(RoomType.WaitingRoom);

            await db.SaveChangesAsync();
        }

        [Given(@"I have a valid leave consultation request")]
        public async Task GivenIHaveAValidLeaveConsultationRequest()
        {
            var conference = await ApiClientResponse.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            var judge =
                conference.Participants.First(x => x.UserRole == Contract.Enums.UserRole.Judge);

            var request = new LeaveConsultationRequest()
            {
                ConferenceId = conference.Id,
                ParticipantId = judge.Id
            };

            SerialiseLeavePrivateConsultationRequest(request);
        }

        [Given(@"the judge joh is in the consultation room")]
        public async Task GivenTheJudgeJohIsInTheConsultationRoom()
        {
            var conferenceResponse = await ApiClientResponse.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            var judgeResponse =
                conferenceResponse.Participants.First(x => x.UserRole == Contract.Enums.UserRole.Judge);
            var consultationRoom = new ConsultationRoom(conferenceResponse.Id, "name", VirtualCourtRoomType.JudgeJOH, false);

            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var conference = await db.Conferences
                .Include(x => x.Participants)
                .SingleAsync(x => x.Id == conferenceResponse.Id);

            var judge = conference.Participants.First(x => x.Id == judgeResponse.Id);
            judge.UpdateCurrentConsultationRoom(consultationRoom);

            await db.SaveChangesAsync();
        }

        private void SerialiseConsultationRequest(ConsultationRequestResponse request)
        {
            _context.Uri = ConsultationEndpoints.HandleConsultationRequest;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        private void SerialiseRespondToConsultationRequestResponse(ConsultationRequestResponse request)
        {
            _context.Uri = ConsultationEndpoints.HandleConsultationRequest;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        private void SerialiseLeaveConsultationRequest(LeaveConsultationRequest request)
        {
            _context.Uri = ConsultationEndpoints.LeaveConsultationRequest;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private void SerialiseLeavePrivateConsultationRequest(LeaveConsultationRequest request)
        {
            _context.Uri = ConsultationEndpoints.LeaveConsultationRequest;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private void SerialiseEndpointConsultationRequest(EndpointConsultationRequest request)
        {
            _context.Uri = ConsultationEndpoints.EndpointConsultationRequest;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        private void SerialiseStartConsultationRequest(StartConsultationRequest request)
        {
            _context.Uri = ConsultationEndpoints.StartPrivateConsultationRequest;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        private ConsultationRequestResponse SetupConsultationRequest(bool withAnswer)
        {
            var request = new ConsultationRequestResponse();

            var participants = _context.Test.Conference.GetParticipants().Where(x =>
                x.UserRole == UserRole.Individual || x.UserRole == UserRole.Representative).ToList();

            request.ConferenceId = _context.Test.Conference.Id;
            request.RequestedBy = participants[0].Id;
            request.RequestedFor = participants[1].Id;

            if (withAnswer)
                request.Answer = ConsultationAnswer.Accepted;
            request.RoomLabel = RoomType.ConsultationRoom.ToString();
            return request;
        }

        private async Task<LeaveConsultationRequest> SetupLeaveConsultationRequest(bool inConsultationRoom)
        {
            var request = new LeaveConsultationRequest
            {
                ConferenceId = _context.Test.Conference.Id,
                ParticipantId = _context.Test.Conference.Participants[0].Id
            };

            if (!inConsultationRoom)
            {
                return request;
            }

            var participantId = _context.Test.Conference.Participants[0].Id;
            await using (var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions))
            {
                var conference = await db.Conferences
                    .Include("Participants")
                    .SingleAsync(x => x.Id == _context.Test.Conference.Id);

                var room = new ConsultationRoom(_context.Test.Conference.Id, "TestRoom", VirtualCourtRoomType.Participant, false);
                var participant = conference.Participants.Single(x => x.Id == participantId);
                participant.UpdateCurrentRoom(RoomType.ConsultationRoom);
                participant.UpdateCurrentConsultationRoom(room);

                await db.SaveChangesAsync();

                request.ParticipantId = participantId;
            }

            return request;
        }

        private ConsultationRequestResponse SetupRespondToConsultationRequestResponse()
        {
            var request = new ConsultationRequestResponse();

            var participants = _context.Test.Conference.GetParticipants().Where(x =>
                x.UserRole == UserRole.Individual || x.UserRole == UserRole.Representative).ToList();

            request.ConferenceId = _context.Test.Conference.Id;
            request.RequestedFor = participants[0].Id;
            request.RequestedBy = participants[0].Id;
            request.Answer = ConsultationAnswer.Accepted;
            request.RoomLabel = RoomType.ConsultationRoom.ToString();

            return request;
        }
    }
}
