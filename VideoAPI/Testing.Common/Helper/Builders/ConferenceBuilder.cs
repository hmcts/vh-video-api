using System;
using System.Collections.Generic;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Domain;

namespace Testing.Common.Helper.Builders
{
    public class ConferenceBuilder
    {
        private readonly Conference _conference;

        public ConferenceBuilder()
        {
            var hearingRefId = Guid.NewGuid();
            var scheduleDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var caseType = "Civil Money Claims";
            var caseNumber = "Test12345";
            _conference = new Conference(hearingRefId, caseType, scheduleDateTime, caseNumber);
        }

        public ConferenceBuilder WithParticipant(string hearingRole, string caseTypeGroup)
        {
            var participant = Builder<Participant>.CreateNew().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Internet.Email(), hearingRole,
                    caseTypeGroup)).Build();
            
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
    }
}