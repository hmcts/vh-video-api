using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Queries
{
    public class GetConferencesTodayForStaffMemberByHearingVenueNameQueryHandlerTests
    {
        private GetConferencesTodayForStaffMemberByHearingVenueNameQueryHandler _handler;
        private VideoApiDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _dbContext = new VideoApiDbContext(new DbContextOptionsBuilder<VideoApiDbContext>()
                .UseInMemoryDatabase("Test")
                .Options);
            _handler = new GetConferencesTodayForStaffMemberByHearingVenueNameQueryHandler(_dbContext);
        }

        [Test]
        public async Task Returns_Conferences_Set_For_Today_With_Specified_Hearing_Venues()
        {
            var conferenceForToday1 = new Conference(Guid.NewGuid(), string.Empty, DateTime.UtcNow, string.Empty, string.Empty, 6, "venue name 1", false, string.Empty);
            var conferenceForToday2 = new Conference(Guid.NewGuid(), string.Empty, DateTime.UtcNow, string.Empty, string.Empty, 6, "venue name 2", false, string.Empty);
            var conferenceForAnotherDay = new Conference(Guid.NewGuid(), string.Empty, DateTime.UtcNow.AddDays(1), string.Empty, string.Empty, 6, "venue name 3", false, string.Empty);

            conferenceForToday1.UpdateMeetingRoom("adminUri", "judgeUri", "participantUri", "pexipnode", "telelphoneconfid");
            conferenceForToday2.UpdateMeetingRoom("adminUri", "judgeUri", "participantUri", "pexipnode", "telelphoneconfid");
            conferenceForAnotherDay.UpdateMeetingRoom("adminUri", "judgeUri", "participantUri", "pexipnode", "telelphoneconfid");

            var listOfConferences = new List<Conference>
            {
                conferenceForToday1, conferenceForToday2, conferenceForAnotherDay
            };

            await _dbContext.Conferences.AddRangeAsync(listOfConferences);
            await _dbContext.SaveChangesAsync();

            var conferences = await _handler.Handle(new GetConferencesTodayForStaffMemberByHearingVenueNameQuery { HearingVenueNames = new List<string> { "venue name 1" } } );

            conferences.Count.Should().Be(1);
            var conf1 = conferences.Find(conf => conf.Id == conferenceForToday1.Id);
            var conf2 = conferences.Find(conf => conf.Id == conferenceForToday2.Id);
            var conf3 = conferences.Find(conf => conf.Id == conferenceForAnotherDay.Id);

            conf1.Should().NotBeNull();
            conf2.Should().BeNull();
            conf3.Should().BeNull();

            _dbContext.Conferences.RemoveRange(listOfConferences);
            await _dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task Returns_Conferences_Set_For_Today_With_Multiple_Specified_Hearing_Venues()
        {
            var venue1 = "venue name 1";
            var venue2 = "venue name 2";

            var specifiedVenues = new List<string> { venue1, venue2 };

            var conferenceForToday1 = new Conference(Guid.NewGuid(), string.Empty, DateTime.UtcNow, string.Empty, string.Empty, 6, venue1, false, string.Empty);
            var conferenceForToday2 = new Conference(Guid.NewGuid(), string.Empty, DateTime.UtcNow, string.Empty, string.Empty, 6, venue2, false, string.Empty);
            var conferenceForAnotherDay = new Conference(Guid.NewGuid(), string.Empty, DateTime.UtcNow.AddDays(1), string.Empty, string.Empty, 6, "venue name 3", false, string.Empty);

            conferenceForToday1.UpdateMeetingRoom("adminUri", "judgeUri", "participantUri", "pexipnode", "telelphoneconfid");
            conferenceForToday2.UpdateMeetingRoom("adminUri", "judgeUri", "participantUri", "pexipnode", "telelphoneconfid");
            conferenceForAnotherDay.UpdateMeetingRoom("adminUri", "judgeUri", "participantUri", "pexipnode", "telelphoneconfid");

            var listOfConferences = new List<Conference>
            {
                conferenceForToday1, conferenceForToday2, conferenceForAnotherDay
            };

            await _dbContext.Conferences.AddRangeAsync(listOfConferences);
            await _dbContext.SaveChangesAsync();

            var conferences = await _handler.Handle(new GetConferencesTodayForStaffMemberByHearingVenueNameQuery { HearingVenueNames = specifiedVenues });

            conferences.Count.Should().Be(2);
            var conf1 = conferences.Find(conf => conf.Id == conferenceForToday1.Id);
            var conf2 = conferences.Find(conf => conf.Id == conferenceForToday2.Id);
            var conf3 = conferences.Find(conf => conf.Id == conferenceForAnotherDay.Id);

            conf1.Should().NotBeNull();
            conf2.Should().NotBeNull();
            conf3.Should().BeNull();

            _dbContext.Conferences.RemoveRange(listOfConferences);
            await _dbContext.SaveChangesAsync();
        }
    }
}
