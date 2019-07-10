using System;
using System.Collections.Generic;
using RestSharp;
using Testing.Common.Configuration;
using Testing.Common.Helper;
using VideoApi.Common.Configuration;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.AcceptanceTests.Contexts
{
    public class TestContext
    {
        public RestRequest Request { get; set; }
        public IRestResponse Response { get; set; }
        public string BearerToken { get; set; }
        public string BaseUrl { get; set; }
        public string Json { get; set; }
        public TestSettings TestSettings { get; set; }
        public ServicesConfiguration ServicesConfiguration { get; set; }
        public Guid NewConferenceId { get; set; }
        public Guid NewHearingRefId { get; set; }
        public ConferenceDetailsResponse NewConference { get; set; }
        public long NewTaskId { get; set; }
        public List<ConferenceSummaryResponse> NewConferences { get; set; }
        public List<Guid> NewConferenceIds { get; set; }

        public TestContext()
        {
            NewConferences = new List<ConferenceSummaryResponse>();
            NewConferenceIds = new List<Guid>();
        }

        public RestClient Client()
        {
            var client = new RestClient(BaseUrl);
            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Authorization", $"Bearer {BearerToken}");
            return client;
        }

        public RestRequest Get(string path) => new RestRequest(path, Method.GET);

        public RestRequest Post(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.POST);
            request.AddParameter("Application/json", ApiRequestHelper.SerialiseRequestToSnakeCaseJson(requestBody),
                ParameterType.RequestBody);
            return request;
        }

        public RestRequest Delete(string path) => new RestRequest(path, Method.DELETE);

        public RestRequest Put(string path, object requestBody)
        {
            var request = new RestRequest(path, Method.PUT);
            request.AddParameter("Application/json", ApiRequestHelper.SerialiseRequestToSnakeCaseJson(requestBody),
                ParameterType.RequestBody);
            return request;
        }

        public RestRequest Patch(string path, object requestBody = null)
        {
            var request = new RestRequest(path, Method.PATCH);
            request.AddParameter("Application/json", ApiRequestHelper.SerialiseRequestToSnakeCaseJson(requestBody),
                ParameterType.RequestBody);
            return request;
        }
    }
}