using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
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
        private VideoApiDbContext _context;
        private AnonymiseParticipantWithUsernameCommandHandler _commandhandler;
        private string usernameToAnonymise = "usernameToAnonymise";
        private Participant _participantToAnonymise, _anonymisedParticipant, _participantInFutureHearing;

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
            _participantToAnonymise = new Participant(Faker.Name.First(), Faker.Name.First(), Faker.Name.First(),
                Faker.Name.First(),
                usernameToAnonymise, UserRole.Individual, Faker.Name.First(), Faker.Name.First());
            _anonymisedParticipant = new Participant(Faker.Name.First(), Faker.Name.First(), Faker.Name.First(),
                Faker.Name.First(),
                Faker.Name.First(), UserRole.Individual, Faker.Name.First(), Faker.Name.First());
            _participantInFutureHearing = new Participant(Faker.Name.First(), Faker.Name.First(), Faker.Name.First(),
                Faker.Name.First(),
                Faker.Name.First(), UserRole.Individual, Faker.Name.First(), Faker.Name.First());

            await _context.Participants.AddRangeAsync(_participantToAnonymise, _anonymisedParticipant,
                _participantInFutureHearing);

            await _context.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            _context.Participants.RemoveRange(_participantToAnonymise, _anonymisedParticipant,
                _participantInFutureHearing);

            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task AnonymiseParticipantWithUsernameCommand_Anonymises_Specified_Username()
        {
            var participantCopyBeforeAnonymisation = new Participant 
                { 
                    Name = _participantToAnonymise.Name, 
                    DisplayName = _participantToAnonymise.DisplayName,
                    Username = _participantToAnonymise.Username,
                    FirstName = _participantToAnonymise.FirstName,
                    LastName = _participantToAnonymise.LastName,
                    ContactEmail = _participantToAnonymise.ContactEmail,
                    ContactTelephone = _participantToAnonymise.ContactTelephone,
                };
            
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
    }
}
