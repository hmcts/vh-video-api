﻿using System.Collections.Generic;
using Bogus;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands;
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
        private static readonly Faker Faker = new();

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
            
            processedParticipant.DisplayName.Should().NotBe(participantCopyBeforeAnonymisation.DisplayName);
            processedParticipant.ContactEmail.Should().NotBe(participantCopyBeforeAnonymisation.ContactEmail);
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
            
            processedParticipant.DisplayName.Should().Be(anonymisedParticipantBeforeAnonymisation.DisplayName);
            processedParticipant.ContactEmail.Should().Be(anonymisedParticipantBeforeAnonymisation.ContactEmail);
            processedParticipant.Username.Should().Be(anonymisedParticipantBeforeAnonymisation.Username);
        }

        [Test]
        public async Task Does_Not_Throw_Exception_When_No_Participants_Are_Found()
        {
            await _commandHandler.Handle(new AnonymiseParticipantWithUsernameCommand
                { Username = "usernotincontext" });


            var processedParticipant =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _participantToAnonymise.Id);
            
            processedParticipant.DisplayName.Should().Be(_participantToAnonymise.DisplayName);
            processedParticipant.ContactEmail.Should().Be(_participantToAnonymise.ContactEmail);
            processedParticipant.Username.Should().Be(_participantToAnonymise.Username);
        }
    }
}
