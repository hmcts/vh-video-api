using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Common.Assertions;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ConferenceSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ConferenceEndpoints _endpoints = new ApiUriFactory().ConferenceEndpoints;

        public ConferenceSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"I have a valid book a new conference request")]
        public void GivenIHaveAValidBookANewConferenceRequest()
        {
            var request = new BookNewConferenceRequestBuilder()
                .WithJudge()
                .WithRepresentative("Claimant").WithIndividual("Claimant")
                .WithRepresentative("Defendant").WithIndividual("Defendant")
                .WithVideoHearingsOfficer()
                .Build();
            _context.Request = _context.Post(_endpoints.BookNewConference, request);
        }

        [Given(@"I have a get details for a conference request with a valid conference id")]
        public void GivenIHaveAGetDetailsForAConferenceRequestWithAValidConferenceId()
        {
            _context.Request = _context.Get(_endpoints.GetConferenceDetailsById(Guid.Parse("68870634-bb30-4bdc-a6cc-5aba9e626aa1")));
        }


        [Then(@"the conference details should be retrieved")]
        public void ThenTheConferenceDetailsShouldBeRetrieved()
        {
            var conference = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(_context.Json);
            AssertConferenceDetailsResponse.ForConference(conference);
        }
    }
}
