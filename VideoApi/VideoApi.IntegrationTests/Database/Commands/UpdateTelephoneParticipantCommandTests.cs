using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Commands;

public class UpdateTelephoneParticipantCommandTests : DatabaseTestsBase
{
    private UpdateTelephoneParticipantCommandHandler _handler;
    private Guid _newConferenceId;
    
    [SetUp]
    public void Setup()
    {
        var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
        _handler = new UpdateTelephoneParticipantCommandHandler(context);
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
        var participantId = Guid.NewGuid();
        var command = new UpdateTelephoneParticipantCommand(conferenceId, participantId, RoomType.HearingRoom,
            TelephoneState.Connected);
        Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
    }
    
    [Test]
    public async Task Should_throw_telephone_participant_not_found_exception_when_telephone_participant_does_not_exist()
    {
        var seededConference = await TestDataManager.SeedConference();
        _newConferenceId = seededConference.Id;

        var participantId = Guid.NewGuid();
        var command = new UpdateTelephoneParticipantCommand(_newConferenceId, participantId, RoomType.HearingRoom,
            TelephoneState.Connected);
        Assert.ThrowsAsync<TelephoneParticipantNotFoundException>(() => _handler.Handle(command));
    }
    
    [Test]
    public async Task Should_update_telephone_participant_in_conference()
    {
        // arrange
        var phoneNumber = "1234567890";
        var conference = new ConferenceBuilder()
            .WithTelephoneParticipant(phoneNumber).Build();
        var seededConference = await TestDataManager.SeedConference(conference);
        _newConferenceId = seededConference.Id;

        // act
        var telephoneParticipant = seededConference.GetTelephoneParticipants().First(x=> x.TelephoneNumber == phoneNumber);
        var command = new UpdateTelephoneParticipantCommand(_newConferenceId, telephoneParticipant.Id, RoomType.HearingRoom,
            TelephoneState.Connected);
        await _handler.Handle(command);

        // assert
        await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
        var updatedConference = await db.Conferences.Include(x => x.TelephoneParticipants)
            .SingleOrDefaultAsync(x => x.Id == _newConferenceId);

        updatedConference.GetTelephoneParticipants().Should().Contain(x => x.Id == telephoneParticipant.Id);
        var updatedParticipant = updatedConference.GetTelephoneParticipants()
            .Single(x => x.Id == telephoneParticipant.Id);
        updatedParticipant.CurrentRoom.Should().Be(RoomType.HearingRoom);
        updatedParticipant.State.Should().Be(TelephoneState.Connected);
    }
}
