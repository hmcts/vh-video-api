using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;

namespace VideoApi.IntegrationTests.Database.Commands;

public class RemoveTelephoneParticipantCommandTests : DatabaseTestsBase
{
    private RemoveTelephoneParticipantCommandHandler _handler;
    private Guid _newConferenceId;
    
    [SetUp]
    public void Setup()
    {
        var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
        _handler = new RemoveTelephoneParticipantCommandHandler(context);
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
        var command = new RemoveTelephoneParticipantCommand(conferenceId, participantId);
        Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
    }
    
    [Test]
    public async Task Should_throw_telephone_participant_not_found_exception_when_telephone_participant_does_not_exist()
    {
        var seededConference = await TestDataManager.SeedConference();
        _newConferenceId = seededConference.Id;

        var participantId = Guid.NewGuid();
        var command = new RemoveTelephoneParticipantCommand(_newConferenceId, participantId);
        Assert.ThrowsAsync<TelephoneParticipantNotFoundException>(() => _handler.Handle(command));
    }
    
    [Test]
    public async Task Should_remove_telephone_participant_from_conference()
    {
        var phoneNumber = "1234567890";
        var conference = new ConferenceBuilder()
            .WithTelephoneParticipant(phoneNumber).Build();
        var seededConference = await TestDataManager.SeedConference(conference);
        _newConferenceId = seededConference.Id;

        var telephoneParticipant = seededConference.GetTelephoneParticipants()[0];
        var command = new RemoveTelephoneParticipantCommand(_newConferenceId, telephoneParticipant.Id);
        await _handler.Handle(command);

        await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
        var updatedConference = await db.Conferences.Include(x => x.TelephoneParticipants)
            .SingleOrDefaultAsync(x => x.Id == _newConferenceId);

        updatedConference.GetTelephoneParticipants().Should().NotContain(x => x.Id == telephoneParticipant.Id);
    }
}
