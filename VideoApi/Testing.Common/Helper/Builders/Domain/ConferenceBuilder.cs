using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Bogus;
using FizzWare.NBuilder;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Domain
{
    public class ConferenceBuilder
    {
        private const string CaseName = "Video Api Integration Test";
        private static readonly Faker Faker = new();
        private readonly BuilderSettings _builderSettings;
        public readonly Conference Conference;
        
        public ConferenceBuilder(bool ignoreId = false, Guid? knownHearingRefId = null,
            DateTime? scheduledDateTime = null, string venueName = "MyVenue", Supplier supplier = Supplier.Vodafone)
        {
            _builderSettings = new BuilderSettings();
            if (ignoreId)
            {
                _builderSettings.DisablePropertyNamingFor<Participant, long?>(x => x.TestCallResultId);
                _builderSettings.DisablePropertyNamingFor<Participant, long?>(x => x.CurrentConsultationRoomId);
                _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<Task, long>(x => x.Id);
            }

            var hearingRefId = knownHearingRefId ?? Guid.NewGuid();

            var scheduleDateTime = scheduledDateTime ?? DateTime.Today.AddHours(9).AddMinutes(30);
            const string caseType = "Generic";
            var randomGenerator = RandomNumberGenerator.Create(); // Compliant for security-sensitive use cases
            var data = new byte[2];
            randomGenerator.GetBytes(data);

            var caseNumber = $"{BitConverter.ToString(data)}";
            const string caseName = CaseName;
            const int scheduledDuration = 120;
            Conference = new Conference(hearingRefId, caseType, scheduleDateTime, caseNumber, caseName, scheduledDuration, venueName, false, "ingesturl", supplier);
        }
        
        public ConferenceBuilder WithParticipants(int numberOfParticipants)
        {
            var participants = new Builder(_builderSettings).CreateListOfSize<Participant>(numberOfParticipants).All()
                .WithFactory(() =>
                    new Participant(Guid.NewGuid(), Faker.Name.FullName(),
                        $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com", UserRole.Individual, $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com",
                        Faker.Phone.PhoneNumber()))
                .All()
                .With(x=> x.CurrentConsultationRoomId = null)
                .And(x => x.EndpointId = null)
                .And(x => x.Endpoint = null)
                .Build();

            foreach (var participant in participants)
            {
                Conference.AddParticipant(participant);
            }

            return this;
        }
        
        public ConferenceBuilder WithParticipants(IEnumerable<Participant> participants)
        {
            foreach (var participant in participants)
            {
                Conference.AddParticipant(participant);
            }

            return this;
        }
        
        public ConferenceBuilder WithParticipant(UserRole userRole, string caseTypeGroup,
            string username = null, string firstName = null, RoomType? roomType = null,
            ParticipantState participantState = ParticipantState.None)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                username = $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com";
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = Faker.Name.FirstName();
            }

            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseTypeGroup);
            var participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Faker.Name.FullName(), username, userRole,  hearingRole, $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com"))
                .And(x=> x.TestCallResultId = null)
                .And(x=> x.CurrentConsultationRoomId = null)
                .And(x => x.EndpointId = null)
                .And(x => x.Endpoint = null)
                .Build();

            if (roomType.HasValue)
            {
                participant.UpdateCurrentRoom(roomType);
                if (roomType == RoomType.ConsultationRoom)
                {
                    participant.CurrentConsultationRoomId = 1;
                    participant.UpdateCurrentConsultationRoom(new ConsultationRoom(Guid.Empty, "Room1", VirtualCourtRoomType.Participant, false));
                }
            }

            participant.UpdateParticipantStatus(participantState == ParticipantState.None
                ? ParticipantState.Available
                : participantState);
            Conference.AddParticipant(participant);

            return this;
        }
        
        /// <summary>
        /// Use after WithParticipant when using withLinkedParticipants parameter
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="sipAddress"></param>
        /// <param name="withLinkedParticipant"></param>
        /// <returns></returns>
        public ConferenceBuilder WithEndpoint(string displayName, string sipAddress, bool withLinkedParticipant = false)
        {
            var endpoint = new Endpoint(displayName, sipAddress, "1234");
            if (withLinkedParticipant)
            {
                var participantsToLink = Conference.Participants.FirstOrDefault(x => x.UserRole == UserRole.Representative) 
                    ?? Conference.Participants.First(x => x.UserRole != UserRole.Judge);
                
                endpoint.AddParticipantLink(participantsToLink);
            }
            Conference.AddEndpoint(endpoint);
            return this;
        }
        
        public ConferenceBuilder WithEndpointAndMultipleParticipants(string displayName, string sipAddress, int numberOfParticipants)
        {
            var endpoint = new Endpoint(displayName, sipAddress, "1234");
            var participantsToLink = Conference.Participants.Where(x => x.UserRole != UserRole.Judge).Take(numberOfParticipants).ToList();
            foreach (var participant in participantsToLink)
                endpoint.AddParticipantLink(participant);
            
            Conference.AddEndpoint(endpoint);
            return this;
        }
        
        public ConferenceBuilder WithTelephoneParticipant(string phoneNumber)
        {
            var telephoneParticipant = new TelephoneParticipant(Guid.NewGuid(), phoneNumber, Conference);
            Conference.AddTelephoneParticipant(telephoneParticipant);

            return this;
        }
        
        public ConferenceBuilder WithMeetingRoom(string pexipNode, string conferenceUsername, bool setTelephoneConferenceId = true, string newTelephoneConferenceId = null)
        {
            var adminUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var judgeUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var participantUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var ticks = DateTime.UtcNow.Ticks.ToString();
            var telephoneConferenceId = newTelephoneConferenceId ?? (setTelephoneConferenceId ? ticks[^8..] : null);
            Conference.UpdateMeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);
            return this;
        }
        
        public Conference Build()
        {
            return Conference;
        }
        
        public ConferenceBuilder WithConferenceStatus(ConferenceState conferenceState, DateTime? timeStamp = null)
        {
            if (conferenceState == ConferenceState.InSession && !Conference.ActualStartTime.HasValue)
            {
                Conference.ActualStartTime = DateTime.UtcNow;
            }

            if (conferenceState == ConferenceState.Closed)
            {
                Conference.ClosedDateTime = DateTime.UtcNow;
            }
            timeStamp ??= DateTime.UtcNow;
            Conference.State = conferenceState;
            Conference.ConferenceStatuses.Add(new ConferenceStatus(conferenceState, timeStamp));
            return this;
        }
        
        public ConferenceBuilder WithMessages(int numberOfMessages)
        {
            var messages = new Builder(_builderSettings).CreateListOfSize<InstantMessage>(numberOfMessages).All()
                .WithFactory(() =>
                    new InstantMessage("Username", "Test InstantMessage", "ReceiverUsername")).Build();

            foreach (var message in messages)
            {
                Conference.AddInstantMessage(message.From, message.MessageText, message.To);
            }

            return this;
        }
        
        public ConferenceBuilder WithAudioRecordingRequired(bool required)
        {
            Conference.AudioRecordingRequired = required;
            return this;
        }
        
        public ConferenceBuilder WithInterpreterRoom()
        {
            if (Conference.Participants.Count < 2)
            {
                WithParticipants(2);
            }
            var room = new Builder(_builderSettings).CreateNew<ParticipantRoom>().WithFactory(() =>
                new ParticipantRoom(Conference.Id, "InterpreterRoom1", VirtualCourtRoomType.Civilian)).Build();

            var nonJudges = Conference.Participants.Where(x => x is Participant && !((Participant)x).IsJudge()).ToList();

            if (Conference.Participants.Count(x => x.LinkedParticipants.Any()) >= 2)
            {
                nonJudges = Conference.Participants.Where(x => x.LinkedParticipants.Any()).ToList();
            }

            room.AddParticipant(new RoomParticipant(nonJudges[0].Id));
            room.AddParticipant(new RoomParticipant(nonJudges[1].Id));

            room.SetProtectedProperty(nameof(room.Id), new Random().Next());

            foreach (var roomParticipant in room.RoomParticipants)
            {
                roomParticipant.Room = room;
                roomParticipant.RoomId = room.Id;
                var participant = Conference.Participants.First(x => x.Id == roomParticipant.ParticipantId);
                participant.RoomParticipants.Add(roomParticipant);
                roomParticipant.Participant = participant;
            }

            Conference.SetProtectedField("_rooms", new List<Room> {room});

            return this;
        }
        
        public ConferenceBuilder WithLinkedParticipant(UserRole userRole, string caseTypeGroup,
            string username = null, string firstName = null, RoomType? roomType = null,
            ParticipantState participantState = ParticipantState.None)
        {
            var username1 = $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com";
            var username2 = $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com";

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = Faker.Name.FirstName();
            }

            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseTypeGroup);
            var participant1 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Faker.Name.FullName(), username1,
                    userRole, hearingRole, $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com"))
                .And(x => x.TestCallResultId = null)
                .And(x => x.CurrentConsultationRoomId = null)
                .And(x => x.EndpointId = null)
                .Build();

            var participant2 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), Faker.Name.FullName(), username2,
                        userRole, hearingRole, $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com"))
                .And(x => x.TestCallResultId = null)
                .And(x => x.CurrentConsultationRoomId = null)
                .And(x => x.EndpointId = null)
                .Build();

            var linkedParticipants1 = new List<LinkedParticipant>();
            var linked = participant2;
            var linkedParticipant1Participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Faker.Name.FullName(), username1,
                    userRole, hearingRole, $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com"))
                .Build();
            linkedParticipants1.Add(new LinkedParticipant(linkedParticipant1Participant, linked, LinkedParticipantType.Interpreter));
            participant1.LinkedParticipants = linkedParticipants1;
            foreach (var linkedParticipant in participant1.LinkedParticipants)
            {
                linkedParticipant.Participant.UpdateParticipantStatus(ParticipantState.Available);
            }

            var linkedParticipants2 = new List<LinkedParticipant>();
            linked = participant1;
            var linkedParticipant2Participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), Faker.Name.FullName(), username1,
                        userRole, hearingRole, $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com"))
                .Build();
            linkedParticipants2.Add(new LinkedParticipant(linkedParticipant2Participant, linked, LinkedParticipantType.Interpreter));
            participant2.LinkedParticipants = linkedParticipants2;
            foreach (var linkedParticipant in participant2.LinkedParticipants)
            {
                linkedParticipant.Participant.UpdateParticipantStatus(ParticipantState.Available);
            }


            participant1.UpdateParticipantStatus(participantState == ParticipantState.None
                ? ParticipantState.Available
                : participantState);
            participant2.UpdateParticipantStatus(participantState == ParticipantState.None
                ? ParticipantState.Available
                : participantState);
            Conference.AddParticipant(participant1);
            Conference.AddParticipant(participant2);

            return this;
        }
        
        public ConferenceBuilder WithInterpreterLinkedParticipant(UserRole userRole, string caseTypeGroup,
            string username = null, string firstName = null, RoomType? roomType = null,
            ParticipantState participantState = ParticipantState.None)
        {
            var username1 = $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com";
            var username2 = $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com";

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = Faker.Name.FirstName();
            }

            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseTypeGroup);
            var participant1 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Faker.Name.FullName(), username1,
                    userRole, hearingRole, $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com"))
                .And(x => x.TestCallResultId = null)
                .And(x => x.CurrentConsultationRoomId = null)
                .Build();

            var participant2 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), Faker.Name.FullName(), username2,
                        UserRole.Individual, "Interpreter", $"Video_Api_Integration_Test_{Faker.Random.Number(0, 99999999 )}@email.com"))
                .And(x => x.TestCallResultId = null)
                .And(x => x.CurrentConsultationRoomId = null)
                .Build();

            var linkedParticipants1 = new List<LinkedParticipant>();
            var participantId = participant1.Id;
            var linkedId = participant2.Id;
            linkedParticipants1.Add(new LinkedParticipant(participantId, linkedId, LinkedParticipantType.Interpreter));
            participant1.LinkedParticipants = linkedParticipants1;

            var linkedParticipants2 = new List<LinkedParticipant>();
            participantId = participant2.Id;
            linkedId = participant1.Id;
            linkedParticipants2.Add(new LinkedParticipant(participantId, linkedId, LinkedParticipantType.Interpreter));
            participant2.LinkedParticipants = linkedParticipants2;


            participant1.UpdateParticipantStatus(participantState == ParticipantState.None
                ? ParticipantState.Available
                : participantState);
            participant2.UpdateParticipantStatus(participantState == ParticipantState.None
                ? ParticipantState.Available
                : participantState);
            Conference.AddParticipant(participant1);
            Conference.AddParticipant(participant2);

            return this;
        }
    }
}
