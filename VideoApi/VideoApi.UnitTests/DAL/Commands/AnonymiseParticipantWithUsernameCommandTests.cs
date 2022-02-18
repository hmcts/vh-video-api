using System.Collections.Generic;
using Faker;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class AnonymiseParticipantWithUsernameCommandTests : EfCoreSetup
    {
        private readonly string usernameToAnonymise = "usernameToAnonymise";
        private AnonymiseParticipantWithUsernameCommandHandler _commandHandler;
        private List<Participant> _participants = new List<Participant>();
        private Participant _participantToAnonymise, _anonymisedParticipant;

        [SetUp]
        public async Task SetUp()
        {
            _commandHandler = new AnonymiseParticipantWithUsernameCommandHandler(videoApiDbContext);
            _participantToAnonymise = DomainModelFactoryForTests.CreateParticipantWithUsername(usernameToAnonymise);
            _anonymisedParticipant = DomainModelFactoryForTests.CreateParticipant();
            _participants = new List<Participant>
                { _participantToAnonymise, _anonymisedParticipant };

            await videoApiDbContext.Participants.AddRangeAsync(_participants);

            await videoApiDbContext.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            videoApiDbContext.Participants.RemoveRange(_participants);

            await videoApiDbContext.SaveChangesAsync();
        }

        [Test]
        public async Task Anonymises_Specified_Username()
        {
            var participantCopyBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion(_participantToAnonymise);

            await _commandHandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _participantToAnonymise.Id);

            processedParticipant.Name.Should().NotBe(participantCopyBeforeAnonymisation.Name);
            processedParticipant.DisplayName.Should().NotBe(participantCopyBeforeAnonymisation.DisplayName);
            processedParticipant.FirstName.Should().NotBe(participantCopyBeforeAnonymisation.FirstName);
            processedParticipant.LastName.Should().NotBe(participantCopyBeforeAnonymisation.LastName);
            processedParticipant.ContactEmail.Should().NotBe(participantCopyBeforeAnonymisation.ContactEmail);
            processedParticipant.ContactTelephone.Should().NotBe(participantCopyBeforeAnonymisation.ContactTelephone);
            processedParticipant.Username.Should()
                .Contain(Constants.AnonymisedUsernameSuffix);
        }

        [Test]
        public async Task Anonymises_All_Matching_Entries_With_Specified_Username()
        {
            var duplicateParticipantWithSameUsername =
                DomainModelFactoryForTests.CreateParticipantWithUsername(usernameToAnonymise);
            _participants.Add(duplicateParticipantWithSameUsername);

            await videoApiDbContext.Participants.AddAsync(duplicateParticipantWithSameUsername);
            await videoApiDbContext.SaveChangesAsync();

            await _commandHandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var countOfParticipantsWithAnonymisedSuffix = await videoApiDbContext.Participants.CountAsync(p =>
                p.Username.Contains(Constants.AnonymisedUsernameSuffix));
            countOfParticipantsWithAnonymisedSuffix.Should().Be(2);
        }

        [Test]
        public async Task Does_Not_Anonymise_Anonymised_Participant()
        {
            var anonymisedParticipant =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _anonymisedParticipant.Id);
            anonymisedParticipant.Username =
                $"{usernameToAnonymise}{Constants.AnonymisedUsernameSuffix}";
            await videoApiDbContext.SaveChangesAsync();

            var anonymisedParticipantBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion(_anonymisedParticipant);

            await _commandHandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _anonymisedParticipant.Id);

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
            videoApiDbContext.Participants.Update(_participantToAnonymise);
            await videoApiDbContext.SaveChangesAsync();

            var participantCopyBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion(_participantToAnonymise);

            await _commandHandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _participantToAnonymise.Id);

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
            videoApiDbContext.Participants.Update(_participantToAnonymise);
            await videoApiDbContext.SaveChangesAsync();

            await _commandHandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = usernameToAnonymise });

            var processedParticipant =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _participantToAnonymise.Id);

            processedParticipant.Representee.Should().Be(representee);
        }
    }
}
