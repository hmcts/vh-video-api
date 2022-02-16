using System.Collections.Generic;
using Faker;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class AnonymiseParticipantWithUsernameCommandTests
    {
        private AnonymiseParticipantWithUsernameCommandHandler _commandhandler;
        private VideoApiDbContext _context;
        private List<Participant> _participants = new List<Participant>();
        private Participant _participantToAnonymise, _anonymisedParticipant;
        private readonly string usernameToAnonymise = "usernameToAnonymise";

        [OneTimeSetUp]
        public void InitialSetUp()
        {
            _context = new VideoApiDbContext(new DbContextOptionsBuilder<VideoApiDbContext>()
                .UseInMemoryDatabase("Test")
                .Options);
            _commandhandler = new AnonymiseParticipantWithUsernameCommandHandler(_context);
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public async Task SetUp()
        {
            _participantToAnonymise = new Participant(Name.First(), Name.First(), Name.First(),
                Name.First(),
                usernameToAnonymise, UserRole.Individual, Name.First(), Name.First());
            _anonymisedParticipant = new Participant(Name.First(), Name.First(), Name.First(),
                Name.First(),
                Name.First(), UserRole.Individual, Name.First(), Name.First());
            _participants = new List<Participant>
                { _participantToAnonymise, _anonymisedParticipant };

            await _context.Participants.AddRangeAsync(_participants);

            await _context.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            _context.Participants.RemoveRange(_participants);

            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task Anonymises_Specified_Username()
        {
            var participantCopyBeforeAnonymisation = CreateParticipantCopyForAssertion(_participantToAnonymise);

            await _commandhandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await _context.Participants.SingleOrDefaultAsync(p => p.Id == _participantToAnonymise.Id);

            processedParticipant.Name.Should().NotBe(participantCopyBeforeAnonymisation.Name);
            processedParticipant.DisplayName.Should().NotBe(participantCopyBeforeAnonymisation.DisplayName);
            processedParticipant.FirstName.Should().NotBe(participantCopyBeforeAnonymisation.FirstName);
            processedParticipant.LastName.Should().NotBe(participantCopyBeforeAnonymisation.LastName);
            processedParticipant.ContactEmail.Should().NotBe(participantCopyBeforeAnonymisation.ContactEmail);
            processedParticipant.ContactTelephone.Should().NotBe(participantCopyBeforeAnonymisation.ContactTelephone);
            processedParticipant.Username.Should()
                .Contain(AnonymiseParticipantWithUsernameCommandHandler.AnonymisedUsernameSuffix);
        }

        [Test]
        public async Task Anonymises_All_Matching_Entries_With_Specified_Username()
        {
            var duplicateParticipantWithSameUsername = CreateParticipantWithUsername(usernameToAnonymise);
            _participants.Add(duplicateParticipantWithSameUsername);

            await _context.Participants.AddAsync(duplicateParticipantWithSameUsername);
            await _context.SaveChangesAsync();

            await _commandhandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var countOfParticipantsWithAnonymisedSuffix = await _context.Participants.CountAsync(p =>
                p.Username.Contains(AnonymiseParticipantWithUsernameCommandHandler.AnonymisedUsernameSuffix));
            countOfParticipantsWithAnonymisedSuffix.Should().Be(2);
        }

        [Test]
        public async Task Does_Not_Anonymise_Anonymised_Participant()
        {
            var anonymisedParticipant =
                await _context.Participants.SingleOrDefaultAsync(p => p.Id == _anonymisedParticipant.Id);
            anonymisedParticipant.Username =
                $"{usernameToAnonymise}{AnonymiseParticipantWithUsernameCommandHandler.AnonymisedUsernameSuffix}";
            await _context.SaveChangesAsync();

            var anonymisedParticipantBeforeAnonymisation = CreateParticipantCopyForAssertion(_anonymisedParticipant);

            await _commandhandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await _context.Participants.SingleOrDefaultAsync(p => p.Id == _anonymisedParticipant.Id);

            processedParticipant.Name.Should().Be(anonymisedParticipantBeforeAnonymisation.Name);
            processedParticipant.DisplayName.Should().Be(anonymisedParticipantBeforeAnonymisation.DisplayName);
            processedParticipant.FirstName.Should().Be(anonymisedParticipantBeforeAnonymisation.FirstName);
            processedParticipant.LastName.Should().Be(anonymisedParticipantBeforeAnonymisation.LastName);
            processedParticipant.ContactEmail.Should().Be(anonymisedParticipantBeforeAnonymisation.ContactEmail);
            processedParticipant.ContactTelephone.Should()
                .Be(anonymisedParticipantBeforeAnonymisation.ContactTelephone);
            processedParticipant.Username.Should().Be(anonymisedParticipantBeforeAnonymisation.Username);
        }

        [Test]
        public async Task Anonymises_Representee()
        {
            _participantToAnonymise.Representee = Name.First();
            _context.Participants.Update(_participantToAnonymise);
            await _context.SaveChangesAsync();

            var participantCopyBeforeAnonymisation = CreateParticipantCopyForAssertion(_participantToAnonymise);

            await _commandhandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await _context.Participants.SingleOrDefaultAsync(p => p.Id == _participantToAnonymise.Id);

            processedParticipant.Representee.Should().NotBe(participantCopyBeforeAnonymisation.Representee);
            processedParticipant.Name.Should().Equals(processedParticipant.Representee);
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task Does_Not_Anonymises_Representee(string representee)
        {
            _participantToAnonymise.Representee = representee;
            _context.Participants.Update(_participantToAnonymise);
            await _context.SaveChangesAsync();

            await _commandhandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await _context.Participants.SingleOrDefaultAsync(p => p.Id == _participantToAnonymise.Id);

            processedParticipant.Representee.Should().Be(representee);
        }

        private Participant CreateParticipantWithUsername(string username)
        {
            return new Participant(Name.First(),
                Name.First(),
                Name.First(),
                Name.First(),
                username, UserRole.Individual, Name.First(), Name.First());
        }

        private Participant CreateParticipantCopyForAssertion(Participant participant)
        {
            return new Participant
            {
                Name = participant.Name,
                DisplayName = participant.DisplayName,
                Username = participant.Username,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                Representee = participant.Representee
            };
        }
    }
}
