using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Domain
{
    public class ConferenceBuilder
    {
        private const string CaseName = "Video Api Integration Test";
        private readonly Conference _conference;
        private readonly BuilderSettings _builderSettings;

        public ConferenceBuilder(bool ignoreId = false, Guid? knownHearingRefId = null,
            DateTime? scheduledDateTime = null, string venueName = "MyVenue")
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
            _conference = new Conference(hearingRefId, caseType, scheduleDateTime, caseNumber, caseName,
                scheduledDuration, venueName, false, "ingesturl");
        }

        public ConferenceBuilder WithParticipants(int numberOfParticipants)
        {
            var participants = new Builder(_builderSettings).CreateListOfSize<Participant>(numberOfParticipants).All()
                .WithFactory(() =>
                    new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Name.Last(), Name.FullName(),
                        $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", UserRole.Individual, "Litigant in person", "Applicant", $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net",
                        Phone.Number()))
                .All().With(x=> x.CurrentConsultationRoomId = null)
                .Build();

            foreach (var participant in participants)
            {
                _conference.AddParticipant(participant);
            }

            return this;
        }

        public ConferenceBuilder WithParticipants(IEnumerable<Participant> participants)
        {
            foreach (var participant in participants)
            {
                _conference.AddParticipant(participant);
            }

            return this;
        }

        public ConferenceBuilder WithParticipant(UserRole userRole, string caseTypeGroup,
            string username = null, string firstName = null, RoomType? roomType = null,
            ParticipantState participantState = ParticipantState.None)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                username = $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net";
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = Name.First();
            }

            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseTypeGroup);
            var participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username,
                    userRole,  hearingRole, caseTypeGroup, $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", Phone.Number()))
                .And(x=> x.TestCallResultId = null)
                .And(x=> x.CurrentConsultationRoomId = null)
                .Build();

            if (userRole == UserRole.Representative)
            {
                participant.Representee = "Person";
            }

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
            _conference.AddParticipant(participant);

            return this;
        }

        public ConferenceBuilder WithEndpoint(string displayName, string sipAddress, string defenceAdvocate = null)
        {
            var endpoint = new Endpoint(displayName, sipAddress, "1234", defenceAdvocate);
            _conference.AddEndpoint(endpoint);

            return this;
        }

        public ConferenceBuilder WithEndpoints(List<Endpoint> endpoints)
        {
            endpoints.ForEach(x => _conference.AddEndpoint(x));

            return this;
        }

        public ConferenceBuilder WithMeetingRoom(string pexipNode, string conferenceUsername, bool setTelephoneConferenceId = true)
        {
            var adminUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var judgeUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var participantUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var ticks = DateTime.UtcNow.Ticks.ToString();
            var telephoneConferenceId = setTelephoneConferenceId ? ticks[^8..] : null;
            _conference.UpdateMeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);
            return this;
        }

        public Conference Build()
        {
            return _conference;
        }

        public ConferenceBuilder WithConferenceStatus(ConferenceState conferenceState, DateTime? timeStamp = null)
        {
            if (conferenceState == ConferenceState.InSession && !_conference.ActualStartTime.HasValue)
            {
                _conference.ActualStartTime = DateTime.UtcNow;
            }

            if (conferenceState == ConferenceState.Closed)
            {
                _conference.ClosedDateTime = DateTime.UtcNow;
            }
            timeStamp ??= DateTime.UtcNow;
            _conference.State = conferenceState;
            _conference.ConferenceStatuses.Add(new ConferenceStatus(conferenceState, timeStamp));
            return this;
        }

        public ConferenceBuilder WithMessages(int numberOfMessages)
        {
            var messages = new Builder(_builderSettings).CreateListOfSize<InstantMessage>(numberOfMessages).All()
                .WithFactory(() =>
                    new InstantMessage("Username", "Test InstantMessage", "ReceiverUsername")).Build();

            foreach (var message in messages)
            {
                _conference.AddInstantMessage(message.From, message.MessageText, message.To);
            }

            return this;
        }

        public ConferenceBuilder WithAudioRecordingRequired(bool required)
        {
            _conference.AudioRecordingRequired = required;
            return this;
        }

        public ConferenceBuilder WithInterpreterRoom()
        {
            if (_conference.Participants.Count < 2)
            {
                WithParticipants(2);
            }
            var room = new Builder(_builderSettings).CreateNew<ParticipantRoom>().WithFactory(() =>
                new ParticipantRoom(_conference.Id, "InterpreterRoom1", VirtualCourtRoomType.Civilian)).Build();

            var nonJudges = _conference.Participants.Where(x => x is Participant && !((Participant)x).IsJudge()).ToList();

            if (_conference.Participants.Count(x => x.LinkedParticipants.Any()) >= 2)
            {
                nonJudges = _conference.Participants.Where(x => x.LinkedParticipants.Any()).ToList();
            }

            room.AddParticipant(new RoomParticipant(nonJudges[0].Id));
            room.AddParticipant(new RoomParticipant(nonJudges[1].Id));

            room.SetProtectedProperty(nameof(room.Id), new Random().Next());

            foreach (var roomParticipant in room.RoomParticipants)
            {
                roomParticipant.Room = room;
                roomParticipant.RoomId = room.Id;
                var participant = _conference.Participants.First(x => x.Id == roomParticipant.ParticipantId);
                participant.RoomParticipants.Add(roomParticipant);
                roomParticipant.Participant = participant;
            }

            _conference.SetProtectedField("_rooms", new List<Room> {room});

            return this;
        }

       

        public ConferenceBuilder WithLinkedParticipant(UserRole userRole, string caseTypeGroup,
            string username = null, string firstName = null, RoomType? roomType = null,
            ParticipantState participantState = ParticipantState.None)
        {
            var username1 = $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net";
            var username2 = $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net";

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = Name.First();
            }

            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseTypeGroup);
            var participant1 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username1,
                    userRole, hearingRole, caseTypeGroup, $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", Phone.Number()))
                .And(x => x.TestCallResultId = null)
                .And(x => x.CurrentConsultationRoomId = null)
                .Build();

            var participant2 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username2,
                        userRole, hearingRole, caseTypeGroup, $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", Phone.Number()))
                .And(x => x.TestCallResultId = null)
                .And(x => x.CurrentConsultationRoomId = null)
                .Build();

            var linkedParticipants1 = new List<LinkedParticipant>();
            var linkedId = participant2.Id;
            var linkedParticipant1Participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username1,
                    userRole, hearingRole, caseTypeGroup, $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", Phone.Number()))
                .Build();
            linkedParticipants1.Add(new LinkedParticipant(linkedParticipant1Participant, linkedId, LinkedParticipantType.Interpreter));
            participant1.LinkedParticipants = linkedParticipants1;
            foreach (var linkedParticipant in participant1.LinkedParticipants)
            {
                linkedParticipant.Participant.UpdateParticipantStatus(ParticipantState.Available);
            }

            var linkedParticipants2 = new List<LinkedParticipant>();
            linkedId = participant1.Id;
            var linkedParticipant2Participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username1,
                        userRole, hearingRole, caseTypeGroup, $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", Phone.Number()))
                .Build();
            linkedParticipants2.Add(new LinkedParticipant(linkedParticipant2Participant, linkedId, LinkedParticipantType.Interpreter));
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
            _conference.AddParticipant(participant1);
            _conference.AddParticipant(participant2);

            return this;
        }


        public ConferenceBuilder WithInterpreterLinkedParticipant(UserRole userRole, string caseTypeGroup,
            string username = null, string firstName = null, RoomType? roomType = null,
            ParticipantState participantState = ParticipantState.None)
        {
            var username1 = $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net";
            var username2 = $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net";

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = Name.First();
            }

            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseTypeGroup);
            var participant1 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username1,
                    userRole, hearingRole, caseTypeGroup, $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", Phone.Number()))
                .And(x => x.TestCallResultId = null)
                .And(x => x.CurrentConsultationRoomId = null)
                .Build();

            var participant2 = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username2,
                        UserRole.Individual, "Interpreter", "Claimant", $"Video_Api_Integration_Test_{RandomNumber.Next()}@hmcts.net", Phone.Number()))
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
            _conference.AddParticipant(participant1);
            _conference.AddParticipant(participant2);

            return this;
        }
    }
}
