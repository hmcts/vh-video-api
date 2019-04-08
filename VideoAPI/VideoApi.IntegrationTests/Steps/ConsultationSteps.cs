using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
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
        
        public ConsultationSteps(ApiTestContext apiTestContext, ConferenceTestContext conferenceTestContext) : base(apiTestContext)
        {
            _conferenceTestContext = conferenceTestContext;
        }
        
        [Given(@"I have a (.*) raise consultation request")]
        [Given(@"I have an (.*) raise consultation request")]
        public void GivenIHaveARaiseConsultationRequest(Scenario scenario)
        {
            var request = SetupRaiseConsultationRequest();
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

            ApiTestContext.Uri = _endpoints.RaiseConsultationRequest;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a raise consultation request with an invalid (.*)")]
        public void GivenIHaveARaiseConsultationRequestWithAnInvalidParticipant(string participant)
        {
            var request = SetupRaiseConsultationRequest();
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

            ApiTestContext.Uri = _endpoints.RaiseConsultationRequest;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) respond consultation request")]
        [Given(@"I have an (.*) respond consultation request")]
        public void GivenIHaveARespondConsultationRequest(Scenario scenario)
        {
            var request = SetupReplyConsultationRequest();
            switch (scenario)
            {
                case Scenario.Valid: break;
                case Scenario.Invalid:
                {
                    request.ConferenceId = Guid.Empty;
                    request.RequestedFor = Guid.Empty;
                    request.RequestedBy = Guid.Empty;
                    request.Answer = ConsultationRequestAnswer.None;
                    break;
                }
                case Scenario.Nonexistent:
                {
                    request.ConferenceId = Guid.NewGuid();
                }
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.AnswerConsultationRequest;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a respond consultation request with an invalid (.*)")]
        public void GivenIHaveARespondConsultationRequestWithAnInvalidParticipant(string participant)
        {
            var request = SetupReplyConsultationRequest();
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
            
            ApiTestContext.Uri = _endpoints.AnswerConsultationRequest;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        private ConsultationRequest SetupRaiseConsultationRequest()
        {
            var raiseConsultationRequest = new ConsultationRequest();
            var seededConference = _conferenceTestContext.SeededConference;
            
            if (seededConference == null) return raiseConsultationRequest;
            
            var participants = seededConference.GetParticipants().Where(x =>
                x.UserRole == UserRole.Individual || x.UserRole == UserRole.Representative).ToList();

            raiseConsultationRequest.ConferenceId = seededConference.Id;
            raiseConsultationRequest. RequestedBy = participants[0].Id;
            raiseConsultationRequest.RequestedFor = participants[1].Id;

            return raiseConsultationRequest;
        }
        
        private ConsultationResultRequest SetupReplyConsultationRequest()
        {
            var replyConsultationRequest = new ConsultationResultRequest();
            var seededConference = _conferenceTestContext.SeededConference;
            
            if (seededConference == null) return replyConsultationRequest;
            
            var participants = seededConference.GetParticipants().Where(x =>
                x.UserRole == UserRole.Individual || x.UserRole == UserRole.Representative).ToList();

            replyConsultationRequest.ConferenceId = seededConference.Id;
            replyConsultationRequest.RequestedBy = participants[0].Id;
            replyConsultationRequest.RequestedFor = participants[1].Id;
            replyConsultationRequest.Answer = ConsultationRequestAnswer.Accepted;
            
            return replyConsultationRequest;
        }
    }
}