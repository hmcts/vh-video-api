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
        private readonly Conference _conference;
        private readonly BuilderSettings _builderSettings;

        public ConferenceBuilder(bool ignoreId = false, Guid? knownHearingRefId = null)
        {
            _builderSettings = new BuilderSettings();
            if (ignoreId)
            {
                _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<Alert, long>(x => x.Id);
            }
            
            var hearingRefId = knownHearingRefId ?? Guid.NewGuid();
            var scheduleDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var caseType = "Civil Money Claims";
            var caseNumber = $"Test{Guid.NewGuid():N}";
            var caseName = "Auto vs Manual";
            var scheduledDuration = 120;
            _conference = new Conference(hearingRefId, caseType, scheduleDateTime, caseNumber, caseName, scheduledDuration);
        }
        
        public ConferenceBuilder WithParticipants(int numberOfParticipants)
        {
            var participants = new Builder(_builderSettings).CreateListOfSize<Participant>(numberOfParticipants).All().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Internet.Email(), UserRole.Individual,
                    "Claimant")).Build();

            foreach (var participant in participants)
            {
                _conference.AddParticipant(participant);
            }
            
            return this;
        }

        public ConferenceBuilder WithParticipant(UserRole userRole, string caseTypeGroup, string username = null)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                username = Internet.Email();
            }
            var participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), username, userRole,
                    caseTypeGroup)).Build();
            
            participant.UpdateParticipantStatus(ParticipantState.Available);
            _conference.AddParticipant(participant);

            return this;
        }
        
        public ConferenceBuilder WithAlert(string body, AlertType type)
        {
            _conference.AddAlert(type, body);
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

        public ConferenceBuilder WithMeetingRoom()
        {
            var adminUri = $"https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri = $"https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                $"https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = $"join.poc.hearings.hmcts.net";
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
    }
}