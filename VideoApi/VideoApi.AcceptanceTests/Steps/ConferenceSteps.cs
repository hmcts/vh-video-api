using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Faker;
using TechTalk.SpecFlow;
using Testing.Common.Assertions;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using static Testing.Common.Helper.ApiUriFactory.ConferenceEndpoints;


namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ConferenceSteps
    {
        private const string UpdatedKey = "UpdatedConference";
        private readonly CallbackSteps _callbackSteps;
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        
        public ConferenceSteps(TestContext injectedContext, ScenarioContext scenarioContext,
            CallbackSteps callbackSteps)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
            _callbackSteps = callbackSteps;
        }
        
        [Given(@"I have an update conference request")]
        public void GivenIHaveAnUpdateConferenceRequest()
        {
            var request = new UpdateConferenceRequest
            {
                HearingRefId = _context.Test.ConferenceResponse.HearingId,
                ScheduledDateTime = DateTime.Now.AddHours(1),
                ScheduledDuration = 12,
                AudioRecordingRequired = true
            };
            
            _scenarioContext.Add(UpdatedKey, request);
            _context.Request = TestContext.Put(UpdateConference, request);
        }
        
        [Given(@"I have a valid book a new conference request")]
        public void GivenIHaveAValidBookANewConferenceRequest()
        {
            CreateNewConferenceRequest(DateTime.Now.ToLocalTime().AddMinutes(2));
        }
        
        [Given(@"I have a valid book a new conference request with jvs endpoints")]
        public void GivenIHaveAValidBookANewConferenceRequestWithJvsEndpoints()
        {
            var sipStem = GetSupplierSipAddressStem();
            
            var endpoints = new List<AddEndpointRequest>
            {
                new()
                {
                    DisplayName = "one", SipAddress = $"1234567890{sipStem}", Pin = "1234",
                    DefenceAdvocate = "Defence Sol", ConferenceRole = ConferenceRole.Host
                },
                new()
                {
                    DisplayName = "two", SipAddress = $"2345678901{sipStem}", Pin = "5678",
                    DefenceAdvocate = "Defence Bol", ConferenceRole = ConferenceRole.Host
                }
            };
            
            CreateNewConferenceRequest(DateTime.Now.ToLocalTime().AddMinutes(2), endpoints: endpoints);
        }
        
        [Given(@"I have a conference")]
        public void GivenIHaveAConference()
        {
            CreateConference(DateTime.Now.ToLocalTime().AddMinutes(2));
        }
        
        [Given(@"I have a conference with a linked participant")]
        public void GivenIHaveAConferenceWithALinkedParticipant()
        {
            var sipStem = GetSupplierSipAddressStem();
            var request = new BookNewConferenceRequestBuilder(_context.Test.CaseName, sipStem)
                .WithJudge()
                .WithIndividualAndInterpreter()
                .WithHearingRefId(Guid.NewGuid())
                .Build();
            _context.Request = TestContext.Post(BookNewConference, request);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should()
                .BeTrue(
                    $"New conference is created but was {_context.Response.StatusCode} with error message '{_context.Response.Content}'");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            conference.Participants.SelectMany(x => x.LinkedParticipants)
                .Any(x => x.Type == LinkedParticipantType.Interpreter).Should().BeTrue();
            _context.Test.ConferenceResponse = conference;
        }
        
        [Given(@"I have a conference with an audio recording")]
        public void GivenIHaveAConferenceWithAudioRecording()
        {
            CreateConference(DateTime.UtcNow.AddMinutes(2), null, true);
        }
        
        [Given(@"I have multiple conferences with duplicate first names for judges")]
        public void GivenIHaveMultipleConferencesWithDuplicateFirstNamesForJudges()
        {
            CreateMultipleConferences(true);
        }
        
        [Given(@"I have another conference")]
        public void GivenIHaveAnotherConference()
        {
            _context.Test.ConferenceIds.Add(_context.Test.ConferenceResponse.Id);
            CreateConference(DateTime.UtcNow);
            _context.Test.ConferenceIds.Add(_context.Test.ConferenceResponse.Id);
        }
        
        
        [Given(@"I have another conference without an audio recording")]
        public void GivenIHaveAnotherConferenceWithNoAudioRecording()
        {
            _context.Test.ConferenceIds.Add(_context.Test.ConferenceResponse.Id);
            
            CreateConference(DateTime.UtcNow, null, false);
            _context.Test.ConferenceIds.Add(_context.Test.ConferenceResponse.Id);
        }
        
        [Given(@"I close the last created conference")]
        public void GivenICloseTheLastCreatedConference()
        {
            var conferenceId = _context.Test.ConferenceIds[^1];
            if (conferenceId == Guid.Empty) throw new Exception("Could not delete the last conference created");
            CloseAndCheckConferenceClosed(conferenceId);
        }
        
        [Given(@"I close all conferences")]
        public void GivenICloseAllConferences()
        {
            _context.Test.ConferenceIds.ForEach(x =>
            {
                if (x == Guid.Empty) throw new Exception("Could not delete the conference created");
                CloseAndCheckConferenceClosed(x);
            });
        }
        
        [Given(@"All conferences have started")]
        public void GivenAllConferencesHaveStarted()
        {
            GivenICloseAllConferences();
        }
        
        [Given(@"I have a conference for tomorrow")]
        public void GivenIHaveAConferenceForTomorrow()
        {
            CreateConference(DateTime.UtcNow.AddDays(1));
            _context.Test.TomorrowsConference = _context.Test.ConferenceResponse.Id;
        }
        
        [Given(@"I have a conference for tomorrow with an audio recording")]
        public void GivenIHaveAConferenceForTomorrowWithAnAudioRecording()
        {
            CreateConference(DateTime.UtcNow.AddDays(1), null, true);
            _context.Test.TomorrowsConference = _context.Test.ConferenceResponse.Id;
        }
        
        [Given(@"I have a conference for yesterday")]
        public void GivenIHaveAConferenceForYesterday()
        {
            CreateConference(DateTime.UtcNow.AddDays(-1));
            _context.Test.ConferenceIds.Add(_context.Test.ConferenceResponse.Id);
        }
        
        [Given(@"I have a get details for a conference request with a valid conference id")]
        public void GivenIHaveAGetDetailsForAConferenceRequestWithAValidConferenceId()
        {
            _context.Request = TestContext.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
        }
        
        [Given(@"I have a valid delete conference request")]
        public void GivenIHaveAValidDeleteConferenceRequest()
        {
            _context.Request = TestContext.Delete(RemoveConference(_context.Test.ConferenceResponse.Id));
        }
        
        [Given(@"I have a get conferences today for a vho request")]
        public void GivenIHaveAValidGetTodaysConferencesRequest()
        {
            _context.Request = TestContext.Get(GetConferencesToday);
        }
        
        
        [Given(@"I have a get conferences today for an individual request")]
        public void GivenIHaveAGetConferenceTodayForAnIndividual()
        {
            var individual = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole != UserRole.Judge);
            _context.Request = TestContext.Get(GetConferencesTodayForIndividual(individual.Username));
        }
        
        [Given(@"I have a get expired conferences request")]
        public void GivenIHaveAGetExpiredConferencesRequest()
        {
            _context.Request = TestContext.Get(GetExpiredOpenConferences);
        }
        
        [Given(@"I have a get expired audiorecording conferences request")]
        public void GivenIHaveAGetExpiredAudiorecordingConferencesRequest()
        {
            _context.Request = TestContext.Get(GetExpiredAudiorecordingConferences);
        }
        
        [Given(@"I have a get details for a conference request by hearing id with a valid Hearing Id")]
        public void GivenIHaveAGetDetailsForAConferenceRequestByHearingIdWithAValidHearingId()
        {
            var requestBody = new GetConferencesByHearingIdsRequest
                { HearingRefIds = [_context.Test.ConferenceResponse.HearingId] };
            _context.Request = TestContext.Post(GetConferencesByHearingRefIds(), requestBody);
        }
        
        [Then(@"the conference details have been updated")]
        public void ThenICanSeeTheConferenceDetailsHaveBeenUpdated()
        {
            _context.Request = TestContext.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details are retrieved");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            var expected = _scenarioContext.Get<UpdateConferenceRequest>(UpdatedKey);
            conference.ScheduledDateTime.Should().Be(expected.ScheduledDateTime.ToUniversalTime());
            conference.ScheduledDuration.Should().Be(expected.ScheduledDuration);
            conference.IsWaitingRoomOpen.Should().BeTrue();
            conference.AudioRecordingRequired.Should().Be(expected.AudioRecordingRequired);
        }
        
        [Then(@"the conference details should be retrieved")]
        public void ThenTheConferenceDetailsShouldBeRetrieved()
        {
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            _context.Test.ConferenceResponse = conference;
            AssertConferenceDetailsResponse.ForConference(conference);
        }
        
        
        [Then(@"the conferences should be retrieved")]
        public void ThenTheConferencesShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ConferenceDetailsResponse>>(_context.Response.Content);
            conferences.Should().NotBeNull();
            _context.Test.ConferenceResponse = conferences[0];
            AssertConferenceCoreResponse.ForConference(conferences[0]);
        }
        
        [Then(@"the conference details should be retrieved with jvs endpoints")]
        public void ThenTheConferenceDetailsShouldBeRetrievedWithJvsEndpoints()
        {
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            _context.Test.ConferenceResponse = conference;
            AssertConferenceDetailsResponse.ForConference(conference);
            AssertConferenceDetailsResponse.ForConferenceEndpoints(conference);
        }
        
        [Then(@"a list containing only individual todays hearings conference details should be retrieved")]
        public void ThenAListOfTheConferenceDetailsForIndividualShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ConferenceCoreResponse>>(_context.Response.Content);
            conferences.Should().NotBeNull();
            foreach (var conference in conferences)
            {
                AssertConferenceCoreResponse.ForConference(conference);
                conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.Now.DayOfYear);
            }
            
            _context.Test.ConferenceResponses = conferences
                .Where(x => x.CaseName.StartsWith("Automated Test Hearing"))
                .Select(conference => conference as ConferenceDetailsResponse).ToList();
        }
        
        [Then(@"I have an empty list of expired conferences")]
        public void ThenAListOfNonClosedConferenceDetailsShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ExpiredConferencesResponse>>(_context.Response.Content);
            conferences.Should().NotContain(x => x.CurrentStatus == ConferenceState.Closed);
        }
        
        [Then(@"a list not containing the closed hearings should be retrieved")]
        public void ThenAListNotContainingTheClosedHearingsShouldBeRetrieved()
        {
            ValidateListOfConferences();
        }
        
        [Then(@"retrieved list should not include not expired hearings or without audiorecording")]
        public void ThenRetrievedListShouldNotIncludeNotExpiredHearingsOrWithoutAudiorecording()
        {
            ValidateListOfConferences();
        }
        
        private void ValidateListOfConferences()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ExpiredConferencesResponse>>(_context.Response.Content);
            conferences.Select(x => x.Id).Should().NotContain(_context.Test.ConferenceIds);
        }
        
        [Then(@"the conference should be removed")]
        public void ThenTheConferenceShouldBeRemoved()
        {
            _context.Request = TestContext.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            _context.Test.ConferenceResponse.Id = Guid.Empty;
        }
        
        [Given(@"the judge status is in hearing")]
        public void GivenTheConferenceIsInSession()
        {
            _callbackSteps.GivenIHaveAValidConferenceEventRequestForAJudge(EventType.Transfer);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [Then(@"the Judges in hearings should be retrieved")]
        public void ThenTheJudgeInHearingResponseShouldBeRetrieved()
        {
            var judgeInHearings =
                ApiRequestHelper.Deserialise<List<ParticipantInHearingResponse>>(_context.Response.Content);
            judgeInHearings.Should().NotBeNull();
            var judge = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Judge);
            var expectedHearing =
                judgeInHearings.First(x => x.ConferenceId.Equals(_context.Test.ConferenceResponse.Id));
            expectedHearing.Id.Should().Be(judge.Id);
            expectedHearing.Status.Should().Be(ParticipantState.InHearing);
            expectedHearing.UserRole.Should().Be(judge.UserRole);
            expectedHearing.Username.Should().Be(judge.Username);
        }
        
        private void CreateMultipleConferences(bool addDuplicateFirstNames)
        {
            var judge1 = $"Automation_{Name.First()}{RandomNumber.Next()}";
            var judge2 = $"Automation_{Name.First()}{RandomNumber.Next()}";
            
            for (var i = 0; i < 2; i++)
            {
                CreateConference(DateTime.Now.ToLocalTime().AddMinutes(2), addDuplicateFirstNames ? judge1 : null);
                _context.Test.ConferenceResponses.Add(_context.Test.ConferenceResponse);
            }
            
            for (var i = 0; i < 2; i++)
            {
                CreateConference(DateTime.Now.ToLocalTime().AddMinutes(2), addDuplicateFirstNames ? judge2 : null);
                _context.Test.ConferenceResponses.Add(_context.Test.ConferenceResponse);
            }
        }
        
        private void CreateConference(DateTime date, string judgeFirstName = null, bool audioRequired = false)
        {
            CreateNewConferenceRequest(date, judgeFirstName, audioRequired);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should()
                .BeTrue(
                    $"New conference is created but was {_context.Response.StatusCode} with error message '{_context.Response.Content}'");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            _context.Test.ConferenceResponse = conference;
        }
        
        private void UpdateConferenceStateToClosed(Guid conferenceId)
        {
            _context.Request = TestContext.Put(CloseConference(conferenceId), new object());
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference is closed");
        }
        
        private void CreateNewConferenceRequest(DateTime date, string judgeFirstName = null, bool audioRequired = false,
            List<AddEndpointRequest> endpoints = null)
        {
            var stem = GetSupplierSipAddressStem();
            var request = new BookNewConferenceRequestBuilder(_context.Test.CaseName, stem)
                .WithJudge(judgeFirstName)
                .WithJudicialOfficeHolder()
                .WithRepresentative("Applicant").WithIndividual("Applicant")
                .WithRepresentative("Respondent").WithIndividual("Respondent")
                .WithHearingRefId(Guid.NewGuid())
                .WithDate(date)
                .WithAudiorecordingRequired(audioRequired)
                .WithEndpoints(endpoints ?? new List<AddEndpointRequest>())
                .Build();
            _context.Request = TestContext.Post(BookNewConference, request);
        }
        
        private void CloseAndCheckConferenceClosed(Guid conferenceId)
        {
            UpdateConferenceStateToClosed(conferenceId);
            _context.Request = TestContext.Get(GetConferenceDetailsById(conferenceId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details are retrieved");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.CurrentStatus.Should().Be(ConferenceState.Closed);
        }
        
        private string GetSupplierSipAddressStem()
        {
            return _context.Config.VodafoneConfiguration.SipAddressStem;
        }
    }
}
