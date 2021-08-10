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
using Testing.Common.Assertions;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;
using static Testing.Common.Helper.ApiUriFactory.ConferenceEndpoints;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class ConferenceBaseSteps : BaseSteps
    {
        private readonly TestContext _context;
        private ConferenceDetailsResponse _conferenceDetails;
        private readonly CommonSteps _commonSteps;

        public ConferenceBaseSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
        }

        [Given(@"I have a get conferences for a judge by username request with a (.*) username")]
        [Given(@"I have a get conferences for a judge by username request with an (.*) username")]
        public void GivenIHaveAGetConferenceForAJudgeByUsernameRequestWithA(Scenario scenario)
        {
            string username;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    username = _context.Test.Conference.Participants.First().Username;
                    break;
                }

                case Scenario.Nonexistent:
                    username = $"{RandomNumber.Next()}@hmcts.net";
                    break;
                case Scenario.Invalid:
                    username = "invalidemail";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = GetConferencesTodayForJudge(username);
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get conferences for an individual by username request with a (.*) username")]
        [Given(@"I have a get conferences for an individual by username request with an (.*) username")]
        public void GivenIHaveAGetDetailsForAConferenceRequestByUsernameWithAValidUsername(Scenario scenario)
        {
            string username;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    username = _context.Test.Conference.Participants.First().Username;
                    break;
                }

                case Scenario.Nonexistent:
                    username = $"{RandomNumber.Next()}@hmcts.net";
                    break;
                case Scenario.Invalid:
                    username = "invalidemail";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = GetConferencesTodayForIndividual(username);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get conferences for a vho request")]
        public void GivenIHaveAGetConferencesTodayForAVho()
        {
            _context.Uri = GetConferencesTodayForAdmin;
            _context.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I filter by (.*)")]
        public void GivenIFilterByVenues(string hearingNameList)
        {
            var hearingNames = hearingNameList.Split(",").Select(v => v.Trim());
            var queryParams = string.Join("&", hearingNames.Select(v => $"HearingVenueNames={v}"));
            _context.Uri = $"{GetConferencesTodayForAdmin}?{queryParams}";
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid close conference request")]
        public void GivenIHaveACloseConferenceRequest()
        {
            _context.Uri = CloseConference(_context.Test.Conference.Id);
            _context.HttpMethod = HttpMethod.Put;
        }

        [Given(@"I have a close conference request for a nonexistent conference id")]
        public void GivenIHaveACloseConferenceRequestForANonexistentConferenceId()
        {
            _context.Uri = CloseConference(Guid.NewGuid());
            _context.HttpMethod = HttpMethod.Put;
        }

        [Given(@"I have an invalid close conference request")]
        public void GivenIHaveAnInvalidCloseConferenceRequest()
        {
            var guid = Guid.NewGuid();
            var uri = CloseConference(guid);
            _context.Uri = uri.Replace(guid.ToString(), "nonexistent");
            _context.HttpMethod = HttpMethod.Put;
        }

        [Given(@"I have a get details for a conference request with a (.*) conference id")]
        [Given(@"I have a get details for a conference request with an (.*) conference id")]
        public void GivenIHaveAGetConferenceDetailsRequest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    conferenceId = _context.Test.Conference.Id;
                    break;
                }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = GetConferenceDetailsById(conferenceId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get details for a conference request with a (.*) hearing ref id")]
        [Given(@"I have a get details for a conference request with an (.*) hearing ref id")]
        public void GivenIHaveAGetConferenceDetailsByHearingRefIdRequest(Scenario scenario)
        {
            Guid hearingRefId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    hearingRefId = _context.Test.Conference.HearingRefId;
                    break;
                }

                case Scenario.Nonexistent:
                    hearingRefId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingRefId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = GetConferenceByHearingRefId(hearingRefId);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a (.*) book a new conference request")]
        [Given(@"I have an (.*) book a new conference request")]
        public void GivenIHaveABookANewConferenceRequest(Scenario scenario)
        {
            var request = new BookNewConferenceRequestBuilder(_context.Test.CaseName)
                .WithJudge()
                .WithRepresentative("Applicant").WithIndividual("Applicant")
                .WithRepresentative("Respondent").WithIndividual("Respondent")
                .Build();
            if (scenario == Scenario.Invalid)
            {
                request.Participants = new List<ParticipantRequest>();
                request.HearingRefId = Guid.Empty;
                request.CaseType = string.Empty;
                request.CaseNumber = string.Empty;
                request.ScheduledDuration = 0;
                request.ScheduledDateTime = DateTime.Now.AddDays(-5);
            }

            _context.Uri = BookNewConference;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a valid book a new conference request with linked participants")]
        public void GivenIHaveABookANewConferenceRequestWithLinkedParticipants()
        {
            var request = new BookNewConferenceRequestBuilder(_context.Test.CaseName)
                .WithJudge()
                .WithRepresentative("Applicant").WithIndividualAndInterpreter("Applicant")
                .WithRepresentative("Respondent").WithIndividual("Respondent")
                .Build();

            _context.Uri = BookNewConference;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Given(@"I have a valid book a new conference request with jvs endpoints")]
        public void GivenIHaveAValidBookANewConferenceRequestWithJvsEndpoints()
        {
            var request = new BookNewConferenceRequestBuilder(_context.Test.CaseName)
                .WithJudge()
                .WithRepresentative("Applicant").WithIndividual("Applicant")
                .WithRepresentative("Respondent").WithIndividual("Respondent")
                .WithEndpoints(new List<AddEndpointRequest>
                {
                    new AddEndpointRequest{DisplayName = "one", SipAddress = $"{Guid.NewGuid()}@hmcts.net", Pin = "1234", DefenceAdvocate = "Defence Sol"},
                    new AddEndpointRequest{DisplayName = "two", SipAddress = $"{Guid.NewGuid()}@hmcts.net", Pin = "5678", DefenceAdvocate = "Defence Bol"}
                })
                .Build();

            _context.Uri = BookNewConference;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) remove conference request")]
        [Given(@"I have an (.*) remove conference request")]
        public void GivenIHaveAValidRemoveHearingRequest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    conferenceId = _context.Test.Conference.Id;
                    break;
                }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            _context.Uri = RemoveConference(conferenceId);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a valid get expired open conferences by scheduled date request")]
        public void GivenIHaveAValidGetOpenConferencesByScheduledDateRequest()
        {
            _context.Uri = GetExpiredOpenConferences;
            _context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the conference details should be retrieved")]
        public async Task ThenAConferenceDetailsShouldBeRetrieved()
        {
            _conferenceDetails = await Response.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            _conferenceDetails.Should().NotBeNull();
            AssertConferenceDetailsResponse.ForConference(_conferenceDetails);
        }

        [Then(@"the conference details should be retrieved with jvs endpoints")]
        public async Task ThenAConferenceDetailsShouldBeRetrievedWithJvsEndpoints()
        {
            _conferenceDetails = await Response.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            _conferenceDetails.Should().NotBeNull();
            AssertConferenceDetailsResponse.ForConference(_conferenceDetails);
            AssertConferenceDetailsResponse.ForConferenceEndpoints(_conferenceDetails);
        }

        [Then(@"the conference should be closed")]
        public async Task ThenTheConferenceShouldBeClosed()
        {
            _context.Uri = GetConferenceDetailsById(_context.Test.Conference.Id); 
            _context.HttpMethod = HttpMethod.Get;
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _conferenceDetails = await Response.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            _conferenceDetails.CurrentStatus.Should().Be(ConferenceState.Closed);
        }

        [Then(@"the conference data should be anonymised")]
        public async Task ThenTheConferenceDataShouldBeAnonymised()
        {
            Conference updatedConference;
            var representative = (Participant)_context.Test.Conference.Participants.First(p => p.UserRole == UserRole.Representative);
            await using (var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences.Include(p=>p.Participants).SingleOrDefaultAsync(x => x.Id == _context.Test.Conference.Id);
            }
            updatedConference.Should().NotBeNull();
            updatedConference.CaseName.Should().NotBe(_context.Test.Conference.CaseName);
            var updatedParticipant = (Participant)updatedConference.Participants.First(p => p.UserRole == UserRole.Representative);
            updatedParticipant.DisplayName.Should().NotBe(representative.DisplayName);
            updatedParticipant.FirstName.Should().NotBe(representative.FirstName);
            updatedParticipant.LastName.Should().NotBe(representative.LastName);
            updatedParticipant.Username.Should().NotBe(representative.Username);
            updatedParticipant.Representee.Should().NotBe(representative.Representee);
            updatedParticipant.ContactEmail.Should().NotBe(representative.ContactEmail);
            updatedParticipant.ContactTelephone.Should().NotBe(representative.ContactTelephone);
        }

        [Then(@"the summary of conference details should be retrieved for judges")]
        public async Task ThenTheSummaryOfConferenceDetailsShouldBeRetrieved()
        {
            var conferences = await Response.GetResponses<List<ConferenceForJudgeResponse>>(_context.Response.Content);
            conferences.Should().NotBeNullOrEmpty();
            foreach (var conference in conferences)
            {
                conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.Now.DayOfYear);
                AssertConferenceForJudgeResponse.ForConference(conference);
                conference.Participants.Should().NotBeNullOrEmpty();
                foreach (var participant in conference.Participants)
                {
                    AssertParticipantForJudgeResponse.ForParticipant(participant);
                }
            }
        }
        
        [Then(@"the summary of conference details should be retrieved for individuals")]
        public async Task ThenTheSummaryOfConferenceDetailsShouldBeRetrievedForIndividuals()
        {
            var conferences = await Response.GetResponses<List<ConferenceForIndividualResponse>>(_context.Response.Content);
            conferences.Should().NotBeNullOrEmpty();
            foreach (var conference in conferences)
            {
                conference.ScheduledDateTime.DayOfYear.Should().Be(DateTime.Now.DayOfYear);
                AssertConferenceForIndividualResponse.ForConference(conference);
            }
        }

        [Then(@"only todays conferences should be retrieved for vho")]
        public async Task ThenOnlyTodaysConferencesShouldBeRetrieved()
        {
            var conferences = await Response.GetResponses<List<ConferenceForAdminResponse>>(_context.Response.Content);
            conferences.Should().NotBeNullOrEmpty();
            foreach (var conference in conferences)
            {
                AssertConferenceForAdminResponse.ForConference(conference);
                conference.ScheduledDateTime.Day.Should().Be(DateTime.Now.Day);
            }
        }

        [Then(@"the conference should be removed")]
        public async Task ThenTheHearingShouldBeRemoved()
        {
            Conference removedConference;
            await using (var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions))
            {
                removedConference = await db.Conferences.SingleOrDefaultAsync(x => x.Id == _context.Test.Conference.Id);
            }
            removedConference.Should().BeNull();
        }

        [Then(@"the conference should be updated")]
        public async Task ThenTheHearingShouldBeUpdated()
        {
            Conference updatedConference;
            await using (var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions))
            {
                updatedConference = await db.Conferences.SingleOrDefaultAsync(x => x.Id == _context.Test.Conference.Id);
            }
            updatedConference.Should().NotBeNull();
            updatedConference.AudioRecordingRequired.Should().BeTrue();
        }

        [Then(@"an empty list is retrieved")]
        public async Task ThenAnEmptyListIsRetrieved()
        {
            var conferences = await Response.GetResponses<List<ExpiredConferencesResponse>>(_context.Response.Content);
            conferences.Should().BeEmpty();
        }

        [Then(@"a list without closed conferences is retrieved")]
        public async Task ThenAListWithoutClosedConferencesIsRetrieved()
        {
            var conferences = await Response.GetResponses<List<ExpiredConferencesResponse>>(_context.Response.Content);
            conferences.Count.Should().BeGreaterThan(0);
            conferences.Any(x => x.Id.Equals(_context.Test.YesterdayClosedConference.Id)).Should().BeFalse();
        }

        [When(@"I save the conference details")]
        public async Task WhenISaveTheConferenceDetails()
        {
            _conferenceDetails = await Response.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            _conferenceDetails.Should().NotBeNull();
        }

        [Then(@"the response should be the same")]
        public async Task ThenTheResponseShouldBeTheSame()
        {
            var conference = await Response.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            conference.Should().BeEquivalentTo(_conferenceDetails);
        }

        [Given(@"I have a (.*) update a conference request")]
        public void GivenIHaveAValidUpdateAConferenceRequest(Scenario scenario)
        {
            UpdateConferenceRequest request;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var scheduledDateTime = _context.Test.Conference.ScheduledDateTime.AddMinutes(1);
                    request = new UpdateConferenceRequest
                    {
                        CaseName = _context.Test.Conference.CaseName,
                        ScheduledDateTime = scheduledDateTime,
                        CaseNumber = _context.Test.Conference.CaseNumber,
                        HearingRefId = _context.Test.Conference.HearingRefId,
                        ScheduledDuration = _context.Test.Conference.ScheduledDuration + 10,
                        CaseType = _context.Test.Conference.CaseType,
                        AudioRecordingRequired = true
                    };
                    break;
                }

                case Scenario.Nonexistent:
                    request = new UpdateConferenceRequest
                    {
                        HearingRefId = Guid.NewGuid(),
                        CaseName = "CaseName",
                        ScheduledDateTime = DateTime.Now,
                        ScheduledDuration = 10,
                        CaseType = "CaseType",
                        CaseNumber = "CaseNo"
                    };
                    break;
                case Scenario.Invalid:
                    request = new UpdateConferenceRequest();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }


            _context.Uri = UpdateConference;
            _context.HttpMethod = HttpMethod.Put;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Then(@"I get (.*) hearing\(s\)")]
        public async Task ThenIGetXNumberOfHearings(int number)
        {
            var conferences = await Response.GetResponses<List<ConferenceForAdminResponse>>(_context.Response.Content);
            conferences.Count.Should().Be(number);
        }

        [Given(@"I have a request to anonymise the data")]
        public void GivenIHaveARequestToAnonymiseTheData()
        {
            _context.Uri = AnonymiseConferences;
            _context.HttpMethod = HttpMethod.Patch;
        }

        [Given(@"I have a request to remove heartbeats for conferences")]
        public void GivenIHaveARequestToRemoveHeartbeatsForConferences()
        {
            _context.Uri = RemoveHeartbeatsForconferences;
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Then(@"the heartbeats should be deleted")]
        public async Task ThenTheHeartbeatsShouldBeDeleted()
        {
            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var heartbeats = await db.Heartbeats.Where(x => x.ConferenceId == _context.Test.Conference.Id).ToListAsync();
            heartbeats.Should().NotBeNull();
            heartbeats.Count.Should().Be(0);
        }

        [Then(@"the heartbeats should not be deleted")]
        public async Task ThenTheHeartbeatsShouldNotBeDeleted()
        {
            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var heartbeats = await db.Heartbeats.Where(x => x.ConferenceId == _context.Test.Conference.Id).ToListAsync();
            heartbeats.Should().NotBeNull();
            heartbeats.Count.Should().Be(3);
        }
    }
}
