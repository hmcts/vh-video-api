using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Faker;
using TechTalk.SpecFlow;
using Testing.Common.Assertions;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Contract.Enums;
using static Testing.Common.Helper.ApiUriFactory.ConferenceEndpoints;


namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ConferenceSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private readonly CallbackSteps _callbackSteps;
        private const string UpdatedKey = "UpdatedConference";

        public ConferenceSteps(TestContext injectedContext, ScenarioContext scenarioContext, CallbackSteps callbackSteps)
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
                CaseName = $"{_context.Test.ConferenceResponse.CaseName} UPDATED",
                CaseNumber = $"{_context.Test.ConferenceResponse.CaseNumber} UPDATED",
                CaseType = "Financial Remedy",
                HearingRefId = _context.Test.ConferenceResponse.HearingId,
                ScheduledDateTime = DateTime.Now.AddHours(1),
                ScheduledDuration = 12,
                HearingVenueName = "MyVenue",
                AudioRecordingRequired = true
            };

            _scenarioContext.Add(UpdatedKey, request);
            _context.Request = _context.Put(UpdateConference, request);
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
                new() {DisplayName = "one", SipAddress = $"1234567890{sipStem}", Pin = "1234", DefenceAdvocate = "Defence Sol"},
                new() {DisplayName = "two", SipAddress = $"2345678901{sipStem}", Pin = "5678", DefenceAdvocate = "Defence Bol"}
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
            _context.Request = _context.Post(BookNewConference, request);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue($"New conference is created but was {_context.Response.StatusCode} with error message '{_context.Response.Content}'");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            conference.Participants.SelectMany(x => x.LinkedParticipants)
                .Any(x => x.Type == LinkedParticipantType.Interpreter).Should().BeTrue();
            _context.Test.ConferenceResponse = conference;
        }

        [Given(@"I have a conference with an audio recording")]
        public void GivenIHaveAConferenceWithAudioRecording()
        {
            CreateConference(DateTime.UtcNow.AddMinutes(2),null, true);
        }

        [Given(@"I have multiple conferences with duplicate first names for judges")]
        public void GivenIHaveMultipleConferencesWithDuplicateFirstNamesForJudges ()
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
            _context.Request = _context.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
        }

        [Given(@"I have a valid delete conference request")]
        public void GivenIHaveAValidDeleteConferenceRequest()
        {
            _context.Request = _context.Delete(RemoveConference(_context.Test.ConferenceResponse.Id));
        }

        [Given(@"I have a get conferences today for a vho request")]
        public void GivenIHaveAValidGetTodaysConferencesRequest()
        {
            _context.Request = _context.Get(GetConferencesTodayForAdmin);
        }

        [Given(@"I have a get conferences today for a judge request")]
        public void GivenIHaveAGetConferenceTodayForAJudge()
        {
            var judge = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Judge);
            _context.Request = _context.Get(GetConferencesTodayForJudge(judge.Username));
        }
        
        [Given(@"I have a get conferences today for an individual request")]
        public void GivenIHaveAGetConferenceTodayForAnIndividual()
        {
            var individual = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole != UserRole.Judge);
            _context.Request = _context.Get(GetConferencesTodayForJudge(individual.Username));
        }

        [Given(@"I have a get expired conferences request")]
        public void GivenIHaveAGetExpiredConferencesRequest()
        {
            _context.Request = _context.Get(GetExpiredOpenConferences);
        }

        [Given(@"I have a get expired audiorecording conferences request")]
        public void GivenIHaveAGetExpiredAudiorecordingConferencesRequest()
        {
            _context.Request = _context.Get(GetExpiredAudiorecordingConferences);
        }

        [Given(@"I have a get details for a conference request by hearing id with a valid username")]
        public void GivenIHaveAGetDetailsForAConferenceRequestByHearingIdWithAValidUsername()
        {
            _context.Request = _context.Get(GetConferenceByHearingRefId(_context.Test.ConferenceResponse.HearingId));
        }

        [Then(@"the conference details have been updated")]
        public void ThenICanSeeTheConferenceDetailsHaveBeenUpdated()
        {
            _context.Request = _context.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details are retrieved");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            var expected = _scenarioContext.Get<UpdateConferenceRequest>(UpdatedKey);
            conference.CaseName.Should().Be(expected.CaseName);
            conference.CaseNumber.Should().Be(expected.CaseNumber);
            conference.CaseType.Should().Be(expected.CaseType);
            conference.ScheduledDateTime.Should().Be(expected.ScheduledDateTime.ToUniversalTime());
            conference.ScheduledDuration.Should().Be(expected.ScheduledDuration);
            conference.HearingVenueName.Should().Be(expected.HearingVenueName);
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

        [Then(@"the conference details should be retrieved with jvs endpoints")]
        public void ThenTheConferenceDetailsShouldBeRetrievedWithJvsEndpoints()
        {
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            _context.Test.ConferenceResponse = conference;
            AssertConferenceDetailsResponse.ForConference(conference);
            AssertConferenceDetailsResponse.ForConferenceEndpoints(conference);
        }

        [Then(@"the admin response should contain the conference")]
        public void ThenTheAdminResponseShouldContainTheConference()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ConferenceForAdminResponse>>(_context.Response.Content);
            conferences.Should().NotBeNull();
            var conference = conferences.Single(x => x.CaseName.StartsWith(_context.Test.ConferenceResponse.CaseName));

            foreach (var participant in conference.Participants)
            {
                var expectedParticipant =
                    _context.Test.ConferenceResponse.Participants.Single(x => x.Username.Equals(participant.Username));
                participant.Should().BeEquivalentTo(expectedParticipant, x => x.ExcludingMissingMembers());
            }
        }

        [Then(@"a list containing only todays hearings conference details should be retrieved")]
        public void ThenAListOfTheConferenceDetailsShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ConferenceForAdminResponse>>(_context.Response.Content);
            conferences.Should().NotBeNull();
            conferences.Exists(x => x.CaseName.StartsWith(_context.Test.CaseName)).Should().BeTrue();
            foreach (var conference in conferences)
            {
                if (conference.CaseName.StartsWith(_context.Test.CaseName))
                {
                    AssertConferenceForAdminResponse.ForConference(conference);
                    foreach (var participant in conference.Participants)
                        AssertParticipantResponse.ForParticipant(participant);
                    conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.Now.DayOfYear);
                }
            }

            _context.Test.ConferenceResponses = conferences.Where(x => x.CaseName.StartsWith(_context.Test.CaseName)).ToList();
            conferences.Exists(x => x.Id.Equals(_context.Test.TomorrowsConference)).Should().BeFalse();
        }

        [Then(@"a list containing only judge todays hearings conference details should be retrieved")]
        public void ThenAListOfTheConferenceDetailsForJudgeShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ConferenceForHostResponse>>(_context.Response.Content);
            conferences.Should().NotBeNull();
            foreach (var conference in conferences)
            {
                AssertConferenceForJudgeResponse.ForConference(conference);
                foreach (var participant in conference.Participants)
                    AssertParticipantForJudgeResponse.ForParticipant(participant);
                conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.Now.DayOfYear);
            }

            _context.Test.ConferenceJudgeResponses = conferences.Where(x => x.CaseName.StartsWith("Automated Test Hearing")).ToList();
        }

        [Then(@"a list containing only individual todays hearings conference details should be retrieved")]
        public void ThenAListOfTheConferenceDetailsForIndividualShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ConferenceForIndividualResponse>>(_context.Response.Content);
            conferences.Should().NotBeNull();
            foreach (var conference in conferences)
            {
                AssertConferenceForIndividualResponse.ForConference(conference);
                conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.Now.DayOfYear);
            }

            _context.Test.ConferenceIndividualResponses = conferences.Where(x => x.CaseName.StartsWith("Automated Test Hearing")).ToList();
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

        [Then(@"the summary of conference details should be retrieved")]
        public void ThenTheSummaryOfConferenceDetailsShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.Deserialise<List<ConferenceForAdminResponse>>(_context.Response.Content);
            conferences.Should().NotBeNull();
            _context.Test.ConferenceResponse.Id = conferences[0].Id;
            foreach (var conference in conferences)
            {
                AssertConferenceForAdminResponse.ForConference(conference);
                foreach (var participant in conference.Participants)
                    AssertParticipantResponse.ForParticipant(participant);
            }
        }

        [Then(@"the conference should be removed")]
        public void ThenTheConferenceShouldBeRemoved()
        {
            _context.Request = _context.Get(GetConferenceDetailsById(_context.Test.ConferenceResponse.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            _context.Test.ConferenceResponse.Id = Guid.Empty;
        }
        
        [Given(@"I have a get judges in hearings today")]
        public void GivenIHaveAGetJudgesInHearingsToday()
        {
            _context.Request = _context.Get(GetJudgesInHearingsToday());
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
            var judgeInHearings = ApiRequestHelper.Deserialise<List<ParticipantInHearingResponse>>(_context.Response.Content);
            judgeInHearings.Should().NotBeNull();
            var judge = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Judge);
            var expectedHearing = judgeInHearings.First(x => x.ConferenceId.Equals(_context.Test.ConferenceResponse.Id));
            expectedHearing.Id.Should().Be(judge.Id);
            expectedHearing.Status.Should().Be(ParticipantState.InHearing);
            expectedHearing.UserRole.Should().Be(judge.UserRole);
            expectedHearing.Username.Should().Be(judge.Username);
        }

        private void CreateMultipleConferences(bool addDuplicateFirstNames)
        {
            var judge1 = $"Automation_{Name.First()}{ RandomNumber.Next()}";
            var judge2 = $"Automation_{Name.First()}{ RandomNumber.Next()}";

            for (var i = 0; i < 2; i++)
            {
                CreateConference(DateTime.Now.ToLocalTime().AddMinutes(2), addDuplicateFirstNames ? judge1 : null);
                _context.Test.ConferenceDetailsResponses.Add(_context.Test.ConferenceResponse);
            }
            for (var i = 0; i < 2; i++)
            {
                CreateConference(DateTime.Now.ToLocalTime().AddMinutes(2), addDuplicateFirstNames ? judge2 : null);
                _context.Test.ConferenceDetailsResponses.Add(_context.Test.ConferenceResponse);
            }
        }

        private void CreateConference(DateTime date, string judgeFirstName = null, bool audioRequired = false)
        { 
            CreateNewConferenceRequest(date, judgeFirstName, audioRequired);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue($"New conference is created but was {_context.Response.StatusCode} with error message '{_context.Response.Content}'");
            var conference = ApiRequestHelper.Deserialise<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            _context.Test.ConferenceResponse = conference;
        }

        private void UpdateConferenceStateToClosed(Guid conferenceId)
        {
            _context.Request = _context.Put(CloseConference(conferenceId), new object());
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference is closed");
        }

        private void CreateNewConferenceRequest(DateTime date, string judgeFirstName = null, bool audioRequired = false, List<AddEndpointRequest> endpoints = null)
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
            _context.Request = _context.Post(BookNewConference, request);
        }

        private void CloseAndCheckConferenceClosed(Guid conferenceId)
        {
            UpdateConferenceStateToClosed(conferenceId);
            _context.Request = _context.Get(GetConferenceDetailsById(conferenceId));
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
