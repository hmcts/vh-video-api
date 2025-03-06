using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AnonymiseConferencesCommandTests : DatabaseTestsBase
    {
        private AnonymiseConferencesCommandHandler _handler;
        private List<Conference> conferenceList;
        private GetConferenceByIdQueryHandler _handlerGetConferenceByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AnonymiseConferencesCommandHandler(context);
            _handlerGetConferenceByIdQueryHandler = new GetConferenceByIdQueryHandler(context);
        }

        [Test]
        public async Task Should_anonymise_data_older_than_three_months()
        {
            conferenceList = new List<Conference>();
            var conferenceType = typeof(Conference);
            var utcDate = DateTime.UtcNow;
            var hearingClosed3Months = utcDate.AddMonths(-3).AddMinutes(-50);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: hearingClosed3Months)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMonths(-3).AddMinutes(-10));
            conferenceList.Add(conference1);
            var conference1Rep = (Participant)conference1.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);

            foreach (var c in conferenceList)
            {
                await TestDataManager.SeedConference(c);
            }
            var command = new AnonymiseConferencesCommand();
            await _handler.Handle(command);

            command.RecordsUpdated.Should().Be(3);

            var conference = await _handlerGetConferenceByIdQueryHandler.Handle(new GetConferenceByIdQuery(conference1.Id));
            conference.Should().NotBeNull();

            conference.CaseName.Should().NotBe(conference1.CaseName);
            var representative = (Participant)conference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            representative.DisplayName.Should().NotBe(conference1Rep.DisplayName);
            representative.Username.Should().NotBe(conference1Rep.Username);
            representative.ContactEmail.Should().NotBe(conference1Rep.ContactEmail);
        }

        [Test]
        public async Task Should_not_anonymise_data_older_than_two_months_and_less_than_three_months()
        {
            conferenceList = new List<Conference>();
            var conferenceType = typeof(Conference);
            var utcDate = DateTime.UtcNow;
            var hearingclosed1Month = utcDate.AddMonths(-1).AddMinutes(-50);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: hearingclosed1Month)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference2, DateTime.UtcNow.AddMonths(-1).AddMinutes(-10));
            conferenceList.Add(conference2);
            var conference2Rep = conference2.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);

            foreach (var c in conferenceList)
            {
                await TestDataManager.SeedConference(c);
            }
            var command = new AnonymiseConferencesCommand();
            await _handler.Handle(command);

            command.RecordsUpdated.Should().Be(-1);

            var conference = await _handlerGetConferenceByIdQueryHandler.Handle(new GetConferenceByIdQuery(conference2.Id));
            conference.Should().NotBeNull();

            conference.CaseName.Should().Be(conference2.CaseName);
            var representative = conference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            representative.DisplayName.Should().Be(conference2Rep.DisplayName);
        }

        [Test]
        public async Task Should_not_anonymise_data_for_future_hearings()
        {
            conferenceList = new List<Conference>();
            var utcDate = DateTime.UtcNow;
            var futureHearing = utcDate.AddMonths(1);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: futureHearing)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            conferenceList.Add(conference3);
            var conference3Rep = conference3.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);

            foreach (var c in conferenceList)
            {
                await TestDataManager.SeedConference(c);
            }
            var command = new AnonymiseConferencesCommand();
            await _handler.Handle(command);

            command.RecordsUpdated.Should().Be(-1);

            var conference = await _handlerGetConferenceByIdQueryHandler.Handle(new GetConferenceByIdQuery(conference3.Id));
            conference.Should().NotBeNull();

            conference.CaseName.Should().Be(conference3.CaseName);
            var representative = conference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            representative.DisplayName.Should().Be(conference3Rep.DisplayName);
        }

        [Test]
        public async Task Should_not_anonymise_data_that_has_been_anonymised()
        {
            conferenceList = new List<Conference>();
            var conferenceType = typeof(Conference);
            var utcDate = DateTime.UtcNow;
            var hearingClosed3Months = utcDate.AddMonths(-3).AddMinutes(-50);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: hearingClosed3Months)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMonths(-3).AddMinutes(-10));
            conferenceList.Add(conference1);
            var conference1Rep = (Participant)conference1.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);

            foreach (var c in conferenceList)
            {
                await TestDataManager.SeedConference(c);
            }
            var command = new AnonymiseConferencesCommand();
            await _handler.Handle(command);

            command.RecordsUpdated.Should().Be(3);

            var anonymisedConference = await _handlerGetConferenceByIdQueryHandler.Handle(new GetConferenceByIdQuery(conference1.Id));
            anonymisedConference.Should().NotBeNull();

            anonymisedConference.CaseName.Should().NotBe(conference1.CaseName);
            var anonymisedRepresentative = (Participant)anonymisedConference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            anonymisedRepresentative.DisplayName.Should().NotBe(conference1Rep.DisplayName);
            anonymisedRepresentative.Username.Should().NotBe(conference1Rep.Username);
            anonymisedRepresentative.ContactEmail.Should().NotBe(conference1Rep.ContactEmail);

            command = new AnonymiseConferencesCommand();
            await _handler.Handle(command);

            command.RecordsUpdated.Should().Be(-1);

            var notAnonymisedConference = await _handlerGetConferenceByIdQueryHandler.Handle(new GetConferenceByIdQuery(conference1.Id));
            notAnonymisedConference.Should().NotBeNull();

            notAnonymisedConference.CaseName.Should().Be(anonymisedConference.CaseName);
            var notAnonymisedRepresentative = (Participant)anonymisedConference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            notAnonymisedRepresentative.DisplayName.Should().Be(anonymisedRepresentative.DisplayName);
            notAnonymisedRepresentative.Username.Should().Be(anonymisedRepresentative.Username);
            notAnonymisedRepresentative.ContactEmail.Should().Be(anonymisedRepresentative.ContactEmail);
            
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for AnonymiseConferencesCommandHandler");
            foreach (var c in conferenceList)
            {
                await TestDataManager.RemoveConference(c.Id);
            }
        }
    }
}
