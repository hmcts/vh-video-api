using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands;

public class AddTelephoneParticipantCommandTests : DatabaseTestsBase
{
    private AddTelephoneParticipantCommandHandler _handler;
    private Guid _newConferenceId;

    [SetUp]
    public void Setup()
    {
        var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
        _handler = new AddTelephoneParticipantCommandHandler(context);
        _newConferenceId = Guid.Empty;
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

    [Test]
    public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
    {
        var conferenceId = Guid.NewGuid();
        var command = new AddTelephoneParticipantCommand(conferenceId, Guid.NewGuid(), "1234567890");
        Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
    }

    [Test]
    public async Task should_add_telephone_participant_to_a_conference()
    {
        var seededConference = await TestDataManager.SeedConference();
        _newConferenceId = seededConference.Id;

        var telephoneNumber = "1234567890";
        var command = new AddTelephoneParticipantCommand(_newConferenceId, Guid.NewGuid(), telephoneNumber);
        await _handler.Handle(command);

        await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
        var updatedConference = await db.Conferences.SingleOrDefaultAsync(x => x.Id == _newConferenceId);

        updatedConference.GetTelephoneParticipants().Should().Contain(x => x.TelephoneNumber == telephoneNumber);
        var telephoneParticipant = updatedConference.GetTelephoneParticipants()
            .Single(x => x.TelephoneNumber == telephoneNumber);
        telephoneParticipant.State.Should().Be(TelephoneState.Connected);
        telephoneParticipant.CurrentRoom.Should().Be(RoomType.WaitingRoom);
    }
}
