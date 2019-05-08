using System;
using System.Collections.Generic;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Contract.Requests;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Api
{
    public class BookNewConferenceRequestBuilder
    {
        private readonly BookNewConferenceRequest _bookNewConferenceRequest;

        public BookNewConferenceRequestBuilder()
        {
            var fromRandomNumber = new Random();
            _bookNewConferenceRequest = Builder<BookNewConferenceRequest>.CreateNew()
                .With(x => x.HearingRefId = Guid.NewGuid())
                .With(x => x.CaseType = "Civil Money Claims")
                .With(x => x.ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30))
                .With(x => x.CaseNumber = $"{GenerateRandom.CaseNumber(fromRandomNumber)}")
                .With(x => x.CaseName = $"Automated Test Hearing {GenerateRandom.Letters(fromRandomNumber)}")
                .With(x => x.ScheduledDuration = 120)
                .With(x => x.State = ConferenceState.NotStarted)
                .With(x => x.Participants = new List<ParticipantRequest>())
                .With(x => x.ConferenceStatuses = new List<ConferenceStatus>())
                .With(x => x.Tasks = new List<Task>())
                .Build();
        }

        public BookNewConferenceRequestBuilder WithJudge()
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = Name.FullName())
                .With(x => x.Username = Internet.Email())
                .With(x => x.DisplayName = Internet.UserName())
                .With(x => x.UserRole = UserRole.Judge)
                .Build();
            _bookNewConferenceRequest.Participants.Add(participant);
            
            return this;
        }

        public BookNewConferenceRequestBuilder WithRepresentative(string caseTypeGroup = null)
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = Name.FullName())
                .With(x => x.Username = Internet.Email())
                .With(x => x.DisplayName = Internet.UserName())
                .With(x => x.UserRole = UserRole.Representative)
                .Build();

            if (!string.IsNullOrWhiteSpace(caseTypeGroup))
            {
                participant.CaseTypeGroup = caseTypeGroup;
            }
            
            _bookNewConferenceRequest.Participants.Add(participant);
            return this;
        }
        
        public BookNewConferenceRequestBuilder WithIndividual(string caseTypeGroup = null)
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = Name.FullName())
                .With(x => x.Username = Internet.Email())
                .With(x => x.DisplayName = Internet.UserName())
                .With(x => x.UserRole = UserRole.Individual)
                .Build();

            if (!string.IsNullOrWhiteSpace(caseTypeGroup))
            {
                participant.CaseTypeGroup = caseTypeGroup;
            }
            
            _bookNewConferenceRequest.Participants.Add(participant);
            return this;
        }
        
        public BookNewConferenceRequestBuilder WithVideoHearingsOfficer(string caseTypeGroup = null)
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = Name.FullName())
                .With(x => x.Username = Internet.Email())
                .With(x => x.DisplayName = Internet.UserName())
                .With(x => x.UserRole = UserRole.VideoHearingsOfficer)
                .Build();

            if (!string.IsNullOrWhiteSpace(caseTypeGroup))
            {
                participant.CaseTypeGroup = caseTypeGroup;
            }
            
            _bookNewConferenceRequest.Participants.Add(participant);
            return this;
        }

        public BookNewConferenceRequestBuilder WithHearingRefId(Guid hearingRefId)
        {
            _bookNewConferenceRequest.HearingRefId = hearingRefId;
            return this;
        }

        public BookNewConferenceRequestBuilder WithAlert(string body, TaskType type)
        {
            _bookNewConferenceRequest.Tasks.Add(new Task(body, type));
            return this;
        }

        public BookNewConferenceRequestBuilder WithTasks()
        {
            const string body = "Automated Test Complete Task";
            const string user = "Automation@test.com";
            var newTask = new Task(body, TaskType.Judge);
            _bookNewConferenceRequest.Tasks.Add(newTask);
            newTask.CompleteTask(user);
            newTask = new Task(body, TaskType.Participant);
            _bookNewConferenceRequest.Tasks.Add(newTask);
            newTask = new Task(body, TaskType.Hearing);
            _bookNewConferenceRequest.Tasks.Add(newTask);
            return this;
        }

        public BookNewConferenceRequestBuilder WithConferenceStatus(ConferenceState conferenceState)
        {
            _bookNewConferenceRequest.ConferenceStatuses.Add(new ConferenceStatus(conferenceState));
            return this;
        }

        public BookNewConferenceRequestBuilder WithMeetingRoom(string pexipNode, string conferenceUsername)
        {
            var adminUri = $"https://{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var judgeUri = $"https://{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            var participantUri =
                $"https://{pexipNode}/viju/#/?conference={conferenceUsername}&output=embed";
            _bookNewConferenceRequest.MeetingRoom = new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode);
            return this;
        }

        public BookNewConferenceRequest Build()
        {
            return _bookNewConferenceRequest;
        }
    }
}