using System;
using System.Threading.Tasks;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Queries;

public class GetConferenceByTelephoneIdQueryTests : DatabaseTestsBase
{
    private GetConferencesByTelephoneIdQueryHandler _handler;
    private Guid _newConferenceId;
    
    [SetUp]
    public void Setup()
    {
        var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
        _handler = new GetConferencesByTelephoneIdQueryHandler(context);
        _newConferenceId = Guid.Empty;
    }
    
    [Test]
    public async Task Should_return_conference_on_existing_telephone_id()
    {
        string telephoneId = "123456";
        var seededConference = await TestDataManager.SeedConference(Supplier.Vodafone, telephoneId);
        _newConferenceId = seededConference.Id;
        var conference = await _handler.Handle(new GetConferencesByTelephoneIdQuery(telephoneId));
        conference.Should().NotBeNull();
        conference.Count.Should().Be(1);
        conference[0].Id.Should().Be(_newConferenceId);
    }
    
    [Test]
    public async Task Should_return_empty_list_on_non_existing_telephone_id()
    {
        var conference = await _handler.Handle(new GetConferencesByTelephoneIdQuery("0000000000000"));
        conference.Should().NotBeNull();
        conference.Count.Should().Be(0);
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
