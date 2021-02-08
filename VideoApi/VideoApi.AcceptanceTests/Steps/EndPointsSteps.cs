using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.AcceptanceTests.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Contract.Enums;
using static Testing.Common.Helper.ApiUriFactory.EPEndpoints;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class EndPointsSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private static string _addEndPointRequest = "AddEndPointRequest";
        private static string _removedEndPointId = "RemovedEndPointId";
        private static string _updatedEndPointId = "UpdatedEndPointId";
        private static string _updateEndPointRequest = "UpdateEndPointRequest";

        public EndPointsSteps(TestContext injectedContext, ScenarioContext scenarioContext)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have add endpoint to a conference request with a (.*) conference id")]
        public void GivenIHaveAddEndpointToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);
            var addEndpointRequest = PrepareAddEndpointRequest();
            _scenarioContext.Add(_addEndPointRequest, addEndpointRequest);
            _context.Request = _context.Post(AddEndpointsToConference(conferenceId), addEndpointRequest);
        }

        [Given(@"I have add endpoint with invalid data to a conference request with a (.*) conference id")]
        public void GivenIHaveAddEndpointWithInvalidDataToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);

            _context.Request = _context.Post(AddEndpointsToConference(conferenceId), new AddEndpointRequest()
            {
                DisplayName = string.Empty,
            });
        }

        [Given(@"I have remove endpoint to a conference request with a (.*) conference id")]
        public void GivenIHaveRemoveEndpointToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);
            var endpoints = GetEndPoints();
            var sipAddress = endpoints.First().SipAddress;
            _scenarioContext.Add(_removedEndPointId, sipAddress);
            _context.Request = _context.Delete(RemoveEndpointsFromConference(conferenceId, sipAddress));
        }

        [Given(@"I have remove nonexistent endpoint to a conference request with a (.*) conference id")]
        public void GivenIHaveRemoveNonexistentEndpointToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);
            _context.Request = _context.Delete(RemoveEndpointsFromConference(conferenceId, "sip@sip.com"));
        }

        [Given(@"I have update nonexistent endpoint to a conference request with a (.*) conference id")]
        public void GivenIHaveUpdateNonexistentEndpointToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);
            var updatedEndpointRequest = PrepareUpdateEndpointRequest();
            _scenarioContext.Add(_updateEndPointRequest, updatedEndpointRequest);
            _context.Request = _context.Patch(UpdateEndpoint(conferenceId, "sip@sip.com"), updatedEndpointRequest);
        }


        [Given(@"I have endpoints stored against a conference")]
        public void GivenIHaveEndpointsStoredAgainstAConference()
        {
            var conferenceId = GetConferenceIdForTest(Scenario.Valid);
            var addEndpointRequest = PrepareAddEndpointRequest();
            var request = _context.Post(AddEndpointsToConference(conferenceId), addEndpointRequest);
            _context.Client().Execute(request);
        }

        [Given(@"I have update endpoint to a conference request with a (.*) conference id")]
        public void GivenIHaveUpdateEndpointToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);
            var endpoints = GetEndPoints();
            var sipAddress = endpoints.First().SipAddress;
            _scenarioContext.Add(_updatedEndPointId, sipAddress);

            var updateEndpointRequest = PrepareUpdateEndpointRequest();
            _scenarioContext.Add(_updateEndPointRequest, updateEndpointRequest);
            
            _context.Request = _context.Patch(UpdateEndpoint(conferenceId, sipAddress), updateEndpointRequest);
        }

        [Given(@"I have update endpoint with invalid data to a conference request with a (.*) conference id")]
        public void GivenIHaveUpdateEndpointWithInvalidDataToAConferenceRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);
            var endpoints = GetEndPoints();
            var sipAddress = endpoints.First().SipAddress;
            _context.Request = _context.Patch(UpdateEndpoint(conferenceId, sipAddress), new UpdateEndpointRequest()
            {
                DisplayName = string.Empty,
            });
        }

        [Given(@"I have a get endpoints for a endpoints request with a (.*) conference id")]
        public void GivenIHaveAGetEndpointsForAEndpointsRequestWithAValidConferenceId(Scenario scenario)
        {
            var conferenceId = GetConferenceIdForTest(scenario);
            _context.Request = _context.Get(GetEndpointsForConference(conferenceId));
        }

        [Then(@"the endpoint should be added")]
        public void ThenTheEndpointShouldBeAdded()
        {
            var endpoints = GetEndPoints();
            var requestUsed = _scenarioContext.Get<AddEndpointRequest>(_addEndPointRequest);
            var endpointAdded = endpoints.First(ep => ep.DisplayName == requestUsed.DisplayName);

            endpointAdded.Should().NotBeNull();
            endpointAdded.DisplayName.Should().Be(requestUsed.DisplayName);
            endpointAdded.SipAddress.Should().Be(requestUsed.SipAddress);
            endpointAdded.Pin.Should().Be(requestUsed.Pin);
            endpointAdded.DefenceAdvocate.Should().Be(requestUsed.DefenceAdvocate);
        }

        [Then(@"the endpoint should be deleted")]
        public void ThenTheEndpointShouldBeDeleted()
        {
            var endpoints = GetEndPoints();
            var sipAddress = _scenarioContext.Get<string>(_removedEndPointId);
            endpoints.FirstOrDefault(ep => ep.SipAddress == sipAddress).Should().BeNull();
        }

        [Then(@"the endpoint should be updated")]
        public void ThenTheEndpointShouldBeUpdated()
        {
            var endpoints = GetEndPoints();
            var sipAddress = _scenarioContext.Get<string>(_updatedEndPointId);
            var requestUsed = _scenarioContext.Get<UpdateEndpointRequest>(_updateEndPointRequest);
            
            var endpointUpdated = endpoints.First(ep => ep.SipAddress == sipAddress);
            endpointUpdated.Should().NotBeNull();
            endpointUpdated.DisplayName.Should().Be(requestUsed.DisplayName);
        }

        [Then(@"the endpoints should be retrieved")]
        public void ThenTheEndpointsShouldBeRetrieved()
        {
            var endpoints = RequestHelper.Deserialise<List<Endpoint>>(_context.Response.Content);
            endpoints.Should().NotBeNull();
        }
        
        [Then(@"the endpoint status should be (.*)")]
        public void ThenTheEndpointsStateShouldBe(EndpointState state)
        {
            var endpoint = GetEndPoints().First(x => x.Id == _context.Test.ParticipantId);
            endpoint.Status.Should().Be(state);
        }

        private Guid GetConferenceIdForTest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                    conferenceId = _context.Test.ConferenceResponse.Id;
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            return conferenceId;
        }

        public IList<EndpointResponse> GetEndPoints()
        {
            _context.Request = _context.Get(GetEndpointsForConference(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue();
            var endpoints = RequestHelper.Deserialise<IList<EndpointResponse>> (_context.Response.Content);
            endpoints.Should().NotBeNull();
            return endpoints;
        }

        private AddEndpointRequest PrepareAddEndpointRequest()
        {
            return new AddEndpointRequest
            {
                DisplayName = $"DisplayName{Guid.NewGuid()}",
                SipAddress = $"{Guid.NewGuid()}@videohearings.net",
                Pin = "1234", 
                DefenceAdvocate = "Defence Sol"
            };
        }

        private UpdateEndpointRequest PrepareUpdateEndpointRequest()
        {
            return new UpdateEndpointRequest()
            {
                DisplayName = $"DisplayName{Guid.NewGuid()}_Update",
            };
        }
    }
}
