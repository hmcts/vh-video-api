using System;
using System.Linq;
using Faker;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = VideoApi.Domain.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetMessagesForConferenceQueryTests : DatabaseTestsBase
    {
        private GetMessagesForConferenceQueryHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetMessagesForConferenceQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async System.Threading.Tasks.Task should_retrieve_all_messages()
        {
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Judge, "Judge")
                .Build();

            var judge = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            conference.AddMessage(Internet.Email(), judge.Username, "Message 1");
            conference.AddMessage(judge.Username, Internet.Email(), "Message 2");
            conference.AddMessage(judge.Username, Internet.Email(), "Message 3");
            conference.AddMessage(Internet.Email(), judge.Username, "Message 4");

            var seededConference = await TestDataManager.SeedConference(conference);
            _newConferenceId = seededConference.Id;


            var query = new GetMessagesForConferenceQuery(_newConferenceId);
            var results = await _handler.Handle(query);
            results.Count.Should().Be(conference.GetMessages().Count);
            results.Should().BeInDescendingOrder(x => x.TimeStamp);
        }

        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var query = new GetMessagesForConferenceQuery(conferenceId);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(query));
        }

        [TearDown]
        public async System.Threading.Tasks.Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}