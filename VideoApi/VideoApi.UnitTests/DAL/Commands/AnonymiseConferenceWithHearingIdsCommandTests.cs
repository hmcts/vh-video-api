using System;
using System.Collections.Generic;
using System.Linq;
using Faker;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class AnonymiseConferenceWithHearingIdsCommandTests
    {
        private AnonymiseConferenceWithHearingIdsCommandHandler _commandHandler;
        private VideoApiDbContext _context;
        private List<Conference> _conferences;
        private List<Participant> _participants;
        private Conference _conference1, _conference2;

        [OneTimeSetUp]
        public void InitialSetUp()
        {
            _context = new VideoApiDbContext(new DbContextOptionsBuilder<VideoApiDbContext>()
                .UseInMemoryDatabase("Test")
                .Options);
            _commandHandler = new AnonymiseConferenceWithHearingIdsCommandHandler(_context);
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public async Task SetUp()
        {
            _conference1 = CreateConference();
            _conference2 = CreateConference();
            _conferences = new List<Conference> { _conference1, _conference2 };
            _participants = new List<Participant>();

            await _context.Participants.AddRangeAsync(_participants);
            await _context.Conferences.AddRangeAsync(_conference1, _conference2);

            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Participants.RemoveRange(_participants);
            _context.Conferences.RemoveRange(_conferences);
        }

        [Test]
        public async Task Anonymises_CaseName_With_Specified_Hearing_Ids()
        {
            var caseNameBeforeAnonymisationForConference1 = _conference1.CaseName;
            var caseNameBeforeAnonymisationForConference2 = _conference2.CaseName;
            
            await _commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
            {
                HearingIds = new List<Guid>()
                {
                    _conference1.HearingRefId,
                    _conference2.HearingRefId
                }
            });

            var processedConferences = _context.Conferences.Where(c => c.Id == _conference1.Id || c.Id == _conference2.Id).ToList();

            foreach (var conference in processedConferences)
            {
                conference.CaseName.Should().NotContain(caseNameBeforeAnonymisationForConference1).And
                    .NotContain(caseNameBeforeAnonymisationForConference2);
            }
        }

        [Test]
        public async Task Does_Not_Anonymise_When_Conference_Associated_With_An_Anonymised_Participant()
        {
            var participant= new Participant { Username = $"someUser{Constants.AnonymisedUsernameSuffix}" };
            _participants.Add(participant);
            _context.Participants.Add(participant);
            _conference1.AddParticipant(participant);

            _context.Update(_conference1);
            
            await _context.SaveChangesAsync();
            
            var caseNameBeforeAnonymisationForConference1 = _conference1.CaseName;
            
            await _commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
            {
                HearingIds = new List<Guid>()
                {
                    _conference1.HearingRefId
                }
            });

            var processedConference = await _context.Conferences.FirstOrDefaultAsync(c => c.Id == _conference1.Id);

            processedConference.CaseName.Should().Be(caseNameBeforeAnonymisationForConference1);
        }
        
        private Conference CreateConference() => new Conference(Guid.NewGuid(), Name.First(), DateTime.UtcNow,
            Name.First(),
            Name.First(), 6, Lorem.GetFirstWord(), false, Name.First());
    }
}
