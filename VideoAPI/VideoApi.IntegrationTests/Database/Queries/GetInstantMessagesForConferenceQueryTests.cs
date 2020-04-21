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
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var vhOfficer = "VH Officer";
            conference.AddInstantMessage(vhOfficer, "InstantMessage 1");
            conference.AddInstantMessage(judge.DisplayName, "InstantMessage 2");
            conference.AddInstantMessage(judge.DisplayName, "InstantMessage 3");
            conference.AddInstantMessage(vhOfficer, "InstantMessage 4");

            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;


            var query = new GetInstantMessagesForConferenceQuery(_newConferenceId);
            var results = await _handler.Handle(query);
            results.Count.Should().Be(conference.GetInstantMessageHistory().Count);
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
