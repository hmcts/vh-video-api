using System;
using System.Collections.Generic;
using System.Linq;
using Faker;
using FizzWare.NBuilder;
using FluentAssertions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Domain
{
    public class ConferenceBuilder
    {
        private readonly Conference _conference;
        private readonly BuilderSettings _builderSettings;

        public ConferenceBuilder(bool ignoreId = false, Guid? knownHearingRefId = null, DateTime? scheduledDateTime = null)
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
            const string caseName = "Video Api Integration Test";
            const int scheduledDuration = 120;
            _conference = new Conference(hearingRefId, caseType, scheduleDateTime, caseNumber, caseName, 
                scheduledDuration, "MyVenue");
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
        
        public ConferenceBuilder WithParticipants(IEnumerable<Participant> participants)
        {
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

            if (userRole == UserRole.Representative)
            {
                participant.Representee = "Person";
            }
            
            participant.UpdateParticipantStatus(ParticipantState.Available);
            _conference.AddParticipant(participant);

            return this;
        }
        
        public ConferenceBuilder WithJudgeTask(string body)
        {
            var judge = _conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            judge.Should().NotBeNull("Conference does not have a judge");

            _conference.AddTask(judge.Id, TaskType.Judge, body);
            return this;
        }
        
        public ConferenceBuilder WithParticipantTask(string body)
        {
            var individual = _conference.GetParticipants().First(x =>
                x.UserRole == UserRole.Individual || x.UserRole == UserRole.Representative);
            individual.Should().NotBeNull("Conference does not have an individual");
            _conference.AddTask(individual.Id, TaskType.Participant, body);
            return this;
        }

        
        public ConferenceBuilder WithHearingTask(string body)
        {
            _conference.AddTask(_conference.Id, TaskType.Hearing, body);
            return this;
        }

        public ConferenceBuilder WithTask(string body, TaskType taskType)
        {
            _conference.AddTask(_conference.Id, taskType, body);
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
            var messages = new Builder(_builderSettings).CreateListOfSize<Message>(numberOfMessages).All().WithFactory(() =>
                new Message("Username", "Test Message")).Build();

            foreach (var message in messages)
            {
                _conference.AddMessage(message.From, message.MessageText);
            }

            return this;
        }
    }
}
