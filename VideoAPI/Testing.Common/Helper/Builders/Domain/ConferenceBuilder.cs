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

        public ConferenceBuilder(bool ignoreId = false)
        {
            _builderSettings = new BuilderSettings();
            if (ignoreId)
            {
                _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
            }
            
            var hearingRefId = Guid.NewGuid();
            var scheduleDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var caseType = "Civil Money Claims";
            var caseNumber = "Test12345";
            var caseName = "Auto vs Manual";
            _conference = new Conference(hearingRefId, caseType, scheduleDateTime, caseNumber, caseName);
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

        public ConferenceBuilder WithParticipants(IEnumerable<Participant> participants)
        {
            foreach (var participant in participants)
            {
                _conference.AddParticipant(participant);
            }

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