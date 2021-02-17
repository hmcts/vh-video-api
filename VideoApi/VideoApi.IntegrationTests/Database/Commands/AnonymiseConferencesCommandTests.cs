using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AnonymiseConferencesCommandTests : DatabaseTestsBase
    {
        private AnonymiseConferencesCommandHandler _handler;
        private Guid _conference1Id;
        private Guid _conference2Id;
        private Guid _conference3Id;
        private List<Domain.Conference> conferenceList;
        private GetConferenceByIdQueryHandler _handlerGetConferenceByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AnonymiseConferencesCommandHandler(context);
            _conference1Id = Guid.Empty;
            _conference2Id = Guid.Empty;
            _conference3Id = Guid.Empty;
            _handlerGetConferenceByIdQueryHandler = new GetConferenceByIdQueryHandler(context);
        }

        [Test]
        public async Task Should_anonymise_data_older_than_three_months()
        {
            conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var hearingClosed3Months = utcDate.AddMonths(-3).AddMinutes(-50);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: hearingClosed3Months)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMonths(-3).AddMinutes(-10));
            _conference1Id = conference1.Id;
            conferenceList.Add(conference1);
            var conference1Rep = conference1.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);

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
            var representative = conference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            representative.DisplayName.Should().NotBe(conference1Rep.DisplayName);
            representative.FirstName.Should().NotBe(conference1Rep.FirstName);
            representative.LastName.Should().NotBe(conference1Rep.LastName);
            representative.Username.Should().NotBe(conference1Rep.Username);
            representative.Representee.Should().NotBe(conference1Rep.Representee);
            representative.ContactEmail.Should().NotBe(conference1Rep.ContactEmail);
            representative.ContactTelephone.Should().NotBe(conference1Rep.ContactTelephone);
        }

        [Test]
        public async Task Should_not_anonymise_data_older_than_two_months_and_less_than_three_months()
        {
            conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var hearingclosed1Month = utcDate.AddMonths(-1).AddMinutes(-50);

            var conference2 = new ConferenceBuilder(true, scheduledDateTime: hearingclosed1Month)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference2, DateTime.UtcNow.AddMonths(-1).AddMinutes(-10));
            _conference2Id = conference2.Id;
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
            conferenceList = new List<Domain.Conference>();
            var utcDate = DateTime.UtcNow;
            var futureHearing = utcDate.AddMonths(1);

            var conference3 = new ConferenceBuilder(true, scheduledDateTime: futureHearing)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.InSession)
                .Build();
            _conference3Id = conference3.Id;
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
            conferenceList = new List<Domain.Conference>();
            var conferenceType = typeof(Domain.Conference);
            var utcDate = DateTime.UtcNow;
            var hearingClosed3Months = utcDate.AddMonths(-3).AddMinutes(-50);

            var conference1 = new ConferenceBuilder(true, scheduledDateTime: hearingClosed3Months)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithParticipant(UserRole.Judge, null)
                .WithConferenceStatus(ConferenceState.Closed)
                .Build();
            conferenceType.GetProperty("ClosedDateTime").SetValue(conference1, DateTime.UtcNow.AddMonths(-3).AddMinutes(-10));
            _conference1Id = conference1.Id;
            conferenceList.Add(conference1);
            var conference1Rep = conference1.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);

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
            var anonymisedRepresentative = anonymisedConference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            anonymisedRepresentative.DisplayName.Should().NotBe(conference1Rep.DisplayName);
            anonymisedRepresentative.FirstName.Should().NotBe(conference1Rep.FirstName);
            anonymisedRepresentative.LastName.Should().NotBe(conference1Rep.LastName);
            anonymisedRepresentative.Username.Should().NotBe(conference1Rep.Username);
            anonymisedRepresentative.Representee.Should().NotBe(conference1Rep.Representee);
            anonymisedRepresentative.ContactEmail.Should().NotBe(conference1Rep.ContactEmail);
            anonymisedRepresentative.ContactTelephone.Should().NotBe(conference1Rep.ContactTelephone);

            command = new AnonymiseConferencesCommand();
            await _handler.Handle(command);

            command.RecordsUpdated.Should().Be(-1);

            var notAnonymisedConference = await _handlerGetConferenceByIdQueryHandler.Handle(new GetConferenceByIdQuery(conference1.Id));
            notAnonymisedConference.Should().NotBeNull();

            notAnonymisedConference.CaseName.Should().Be(anonymisedConference.CaseName);
            var notAnonymisedRepresentative = anonymisedConference.Participants.FirstOrDefault(p => p.UserRole == UserRole.Representative);
            notAnonymisedRepresentative.DisplayName.Should().Be(anonymisedRepresentative.DisplayName);
            notAnonymisedRepresentative.FirstName.Should().Be(anonymisedRepresentative.FirstName);
            notAnonymisedRepresentative.LastName.Should().Be(anonymisedRepresentative.LastName);
            notAnonymisedRepresentative.Username.Should().Be(anonymisedRepresentative.Username);
            notAnonymisedRepresentative.Representee.Should().Be(anonymisedRepresentative.Representee);
            notAnonymisedRepresentative.ContactEmail.Should().Be(anonymisedRepresentative.ContactEmail);
            notAnonymisedRepresentative.ContactTelephone.Should().Be(anonymisedRepresentative.ContactTelephone);
            
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
