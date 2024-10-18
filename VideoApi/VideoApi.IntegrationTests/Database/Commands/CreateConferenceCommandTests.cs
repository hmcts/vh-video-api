using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Extensions;
using AudioPlaybackLanguage = VideoApi.Domain.Enums.AudioPlaybackLanguage;
using ConferenceRoomType = VideoApi.Domain.Enums.ConferenceRoomType;
using Supplier = VideoApi.Domain.Enums.Supplier;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class CreateConferenceCommandTests : DatabaseTestsBase
    {
        private CreateConferenceCommandHandler _handler;
        private Guid _newConferenceId;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new CreateConferenceCommandHandler(context);
            _newConferenceId = Guid.Empty;
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
        }

        [Test]
        public async Task Should_save_new_conference()
        {
            var hearingRefId = Guid.NewGuid();
            const string caseType = "Generic";
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const string caseNumber = "AutoTest Create Command 1234";
            const string caseName = "AutoTest vs Manual Test";
            const int scheduledDuration = 120;
            var participant = new ParticipantBuilder(true).Build();
            var participants = new List<Participant> {participant};
            const string hearingVenueName = "MyVenue";
            const string ingestUrl = "https://localhost/ingesturl";
            const bool audioRecordingRequired = true;
            var endpoints = new List<Endpoint>
            {
                new Endpoint("name1", GetSipAddress(), "1234", "Defence Sol"),
                new Endpoint("name2", GetSipAddress(), "5678", "Defence Old")
            };
            const Supplier supplier = Supplier.Vodafone;
            const ConferenceRoomType conferenceRoomType = ConferenceRoomType.VA;
            const AudioPlaybackLanguage audioPlaybackLanguage = AudioPlaybackLanguage.English;

            var command =
                new CreateConferenceCommand(hearingRefId, caseType, scheduledDateTime, caseNumber, caseName,
                    scheduledDuration, participants, hearingVenueName, audioRecordingRequired, ingestUrl, endpoints, new List<LinkedParticipantDto>(),
                    supplier, conferenceRoomType, audioPlaybackLanguage);
            
            await _handler.Handle(command);

            command.NewConferenceId.Should().NotBeEmpty();
            command.HearingRefId.Should().Be(hearingRefId);
            command.CaseType.Should().Be(caseType);
            command.ScheduledDateTime.Should().Be(scheduledDateTime);
            command.CaseNumber.Should().Be(caseNumber);
            command.CaseName.Should().Be(caseName);
            command.ScheduledDuration.Should().Be(scheduledDuration);
            command.HearingVenueName.Should().Be(hearingVenueName);
            command.AudioRecordingRequired.Should().Be(audioRecordingRequired);
            command.IngestUrl.Should().Be(ingestUrl);
            command.Participants.Should().NotBeNullOrEmpty();
            command.Participants.Count.Should().Be(1);
            command.Participants[0].Should().BeEquivalentTo(participant);
            command.Supplier.Should().Be(supplier);
            command.ConferenceRoomType.Should().Be(conferenceRoomType);
            command.AudioPlaybackLanguage.Should().Be(audioPlaybackLanguage);
            _newConferenceId = command.NewConferenceId;
            
            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            conference.Should().NotBeNull();
            conference.Supplier.Should().Be(supplier);
            conference.AudioPlaybackLanguage.Should().Be(AudioPlaybackLanguage.English);
        }

        [Test]
        public async Task Should_save_new_conference_with_linked_participants()
        {
            var hearingRefId = Guid.NewGuid();
            const string caseType = "Generic";
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const string caseNumber = "AutoTest Create Command 1234";
            const string caseName = "AutoTest vs Manual Test";
            const int scheduledDuration = 120;
            
            var participantA = new ParticipantBuilder(true).Build();
            var participantB = new ParticipantBuilder(true).Build();

            var linkedParticipants = new List<LinkedParticipantDto>()
            {
                new LinkedParticipantDto() { ParticipantRefId = participantA.ParticipantRefId, LinkedRefId = participantB.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()},
                new LinkedParticipantDto() { ParticipantRefId = participantB.ParticipantRefId, LinkedRefId = participantA.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()}
            };

            var participants = new List<Participant>() {participantA, participantB};
            
            const string hearingVenueName = "MyVenue";
            const string ingestUrl = "https://localhost/ingesturl";
            const bool audioRecordingRequired = true;
            var endpoints = new List<Endpoint>
            {
                new Endpoint("name1", GetSipAddress(), "1234", "Defence Sol"),
                new Endpoint("name2", GetSipAddress(), "5678", "Defence Old")
            };
            
            
            var command =
                new CreateConferenceCommand(hearingRefId, caseType, scheduledDateTime, caseNumber, caseName,
                    scheduledDuration, participants, hearingVenueName, audioRecordingRequired, ingestUrl, endpoints, linkedParticipants,
                    Supplier.Kinly, ConferenceRoomType.VMR, AudioPlaybackLanguage.EnglishAndWelsh);
            
            await _handler.Handle(command);
            
            command.Participants.Should().NotBeNullOrEmpty();
            command.Participants.Count.Should().Be(2);
            command.Participants[0].Should().BeEquivalentTo(participantA);
            
            _newConferenceId = command.NewConferenceId;
            
            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var confParticipants = conference.GetParticipants();
            var linkCount = confParticipants.Sum(x => x.LinkedParticipants.Count);
            linkCount.Should().Be(2);
            
            // verify correct links have been added
            var participantAFromContext = confParticipants.Single(x => x.Id == participantA.Id);
            var participantBFromContext = confParticipants.Single(x => x.Id == participantB.Id);
            participantAFromContext.LinkedParticipants.Should().Contain(x => x.LinkedId == participantBFromContext.Id);
            participantBFromContext.LinkedParticipants.Should().Contain(x => x.LinkedId == participantAFromContext.Id);
        }
                
        [Test]
        public void Should_throw_participant_link_exception_when_id_doesnt_match()
        {
            var hearingRefId = Guid.NewGuid();
            const string caseType = "Generic";
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            const string caseNumber = "AutoTest Create Command 1234";
            const string caseName = "AutoTest vs Manual Test";
            const int scheduledDuration = 120;
            
            var participantA = new ParticipantBuilder(true).Build();
            var participantB = new ParticipantBuilder(true).Build();

            var fakeIdA = Guid.NewGuid();
            var fakeIdB = Guid.NewGuid();
            
            var linkedParticipants = new List<LinkedParticipantDto>()
            {
                new LinkedParticipantDto() { ParticipantRefId = fakeIdA, LinkedRefId = participantB.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()},
                new LinkedParticipantDto() { ParticipantRefId = fakeIdB, LinkedRefId = participantA.ParticipantRefId, Type = LinkedParticipantType.Interpreter.MapToDomainEnum()}
            };

            var participants = new List<Participant>() {participantA, participantB};
            
            const string hearingVenueName = "MyVenue";
            const string ingestUrl = "https://localhost/ingesturl";
            const bool audioRecordingRequired = true;
            var endpoints = new List<Endpoint>
            {
                new Endpoint("name1", GetSipAddress(), "1234", "Defence Sol"),
                new Endpoint("name2", GetSipAddress(), "5678", "Defence Old")
            };

            var command =
                new CreateConferenceCommand(hearingRefId, caseType, scheduledDateTime, caseNumber, caseName,
                    scheduledDuration, participants, hearingVenueName, audioRecordingRequired, ingestUrl, endpoints, linkedParticipants,
                    Supplier.Kinly, ConferenceRoomType.VMR, AudioPlaybackLanguage.EnglishAndWelsh);
            
            var exception = Assert.ThrowsAsync<ParticipantLinkException>(() => _handler.Handle(command));
            exception.LinkRefId.Should().Be(participantB.ParticipantRefId);
            exception.ParticipantRefId.Should().Be(fakeIdA);
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
        
        private string GetSipAddress()
        {
            var random = new Random();
            var address = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                address += random.Next(0, 9).ToString();
            }
            return address;
        }
    }
}
