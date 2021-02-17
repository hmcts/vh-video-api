using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetMessagesForConferenceQueryTests : DatabaseTestsBase
    {
        private GetInstantMessagesForConferenceQueryHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetInstantMessagesForConferenceQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_retrieve_all_messages()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var vhOfficer = "VH Officer";
            conference.AddInstantMessage(vhOfficer, "InstantMessage 1", judge.DisplayName);
            conference.AddInstantMessage(judge.DisplayName, "InstantMessage 2", vhOfficer);
            conference.AddInstantMessage(judge.DisplayName, "InstantMessage 3", vhOfficer);
            conference.AddInstantMessage(vhOfficer, "InstantMessage 4", judge.DisplayName);

            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;


            var query = new GetInstantMessagesForConferenceQuery(_newConferenceId, vhOfficer);
            var results = await _handler.Handle(query);
            results.Count.Should().Be(conference.GetInstantMessageHistory().Count);
            results.Should().BeInDescendingOrder(x => x.TimeStamp);
        }

        [Test]
        public async Task Should_retrieve_all_messages_for_the_participant()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Applicant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var participant = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var vhOfficer = "VH Officer";

            conference.AddInstantMessage(vhOfficer, "InstantMessage 1", judge.DisplayName);
            conference.AddInstantMessage(judge.DisplayName, "InstantMessage 2", vhOfficer);

            conference.AddInstantMessage(participant.Username, "Hello VHO", vhOfficer);
            conference.AddInstantMessage(vhOfficer, "Hello ParticipantOne", participant.Username);

            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;

            var query = new GetInstantMessagesForConferenceQuery(_newConferenceId, participant.Username);
            var results = await _handler.Handle(query);
            results.Count.Should().Be(conference.GetInstantMessageHistoryFor(participant.Username).Count);
            results.Should().BeInDescendingOrder(x => x.TimeStamp);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}
