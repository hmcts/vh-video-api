using System;
using System.Collections.Generic;
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
                _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<Task, long>(x => x.Id);
            }

            var hearingRefId = knownHearingRefId ?? Guid.NewGuid();

            var scheduleDateTime = scheduledDateTime ?? DateTime.UtcNow.AddMinutes(30);
            const string caseType = "Civil Money Claims";
            var caseNumber = $"{GenerateRandom.CaseNumber(new Random())}";
            const string caseName = CaseName;
            const int scheduledDuration = 120;
            _conference = new Conference(hearingRefId, caseType, scheduleDateTime, caseNumber, caseName,
                scheduledDuration, venueName, false, string.Empty);
        }

        public ConferenceBuilder WithParticipants(int numberOfParticipants)
        {
            var participants = new Builder(_builderSettings).CreateListOfSize<Participant>(numberOfParticipants).All()
                .WithFactory(() =>
                    new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Name.Last(), Name.FullName(),
                        $"Video_Api_Integration_Test_{Internet.Email()}", UserRole.Individual, "Claimant LIP", "Claimant", $"Video_Api_Integration_Test_{Internet.Email()}",
                        Phone.Number())).Build();

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
                username = $"Video_Api_Integration_Test_{Internet.Email()}";
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = Name.First();
            }

            var hearingRole = ParticipantBuilder.DetermineHearingRole(userRole, caseTypeGroup);
            var participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), firstName, Name.Last(), Name.FullName(), username,
                    userRole,  hearingRole, caseTypeGroup, $"Video_Api_Integration_Test_{Internet.Email()}", Phone.Number())).Build();

            if (userRole == UserRole.Representative)
            {
                participant.Representee = "Person";
            }

            if (roomType.HasValue)
            {
                participant.UpdateCurrentRoom(roomType);
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
        
        public ConferenceBuilder WithMeetingRoom(string pexipNode, string conferenceUsername)
        {
            var adminUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var judgeUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var participantUri = $"{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            _conference.UpdateMeetingRoom(adminUri, judgeUri, participantUri, pexipNode);
            return this;
        }

        public Conference Build()
        {
            return _conference;
        }

        public ConferenceBuilder WithConferenceStatus(ConferenceState conferenceState)
        {
            _conference.UpdateConferenceStatus(conferenceState);
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
    }
}
