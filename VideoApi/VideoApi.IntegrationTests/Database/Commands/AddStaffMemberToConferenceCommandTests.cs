using System;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using System.Linq;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Enums;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Extensions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AddStaffMemberToConferenceCommandTests : DatabaseTestsBase
    {
        private AddStaffMemberToConferenceCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddStaffMemberToConferenceCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task Should_add_participant_to_conference()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var beforeCount = seededConference.GetParticipants().Count;

            var addStaffMemberRequest = new AddStaffMemberRequestBuilder(UserRole.StaffMember).Build();
            var participant = new Participant(addStaffMemberRequest.Name.Trim(), addStaffMemberRequest.FirstName.Trim(), addStaffMemberRequest.LastName.Trim(),
                addStaffMemberRequest.DisplayName.Trim(), addStaffMemberRequest.Username.ToLowerInvariant().Trim(), addStaffMemberRequest.UserRole.MapToDomainEnum(),
                addStaffMemberRequest.HearingRole, addStaffMemberRequest.ContactEmail);
            
            var command = new AddStaffMemberToConferenceCommand(_newConferenceId, participant);
            await _handler.Handle(command);

            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var confParticipants = conference.GetParticipants();
            confParticipants.Any(x => x.Username == participant.Username).Should().BeTrue();
            var afterCount = conference.GetParticipants().Count;
            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var participant = new ParticipantBase();

            var command = new AddStaffMemberToConferenceCommand(conferenceId, participant);
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
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
