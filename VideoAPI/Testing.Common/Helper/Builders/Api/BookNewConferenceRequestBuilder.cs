using System;
using System.Collections.Generic;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Api
{
    public class BookNewConferenceRequestBuilder
    {
        private readonly BookNewConferenceRequest _bookNewConferenceRequest;

        public BookNewConferenceRequestBuilder(string caseName)
        {
            var fromRandomNumber = new Random();
            _bookNewConferenceRequest = Builder<BookNewConferenceRequest>.CreateNew()
                .With(x => x.HearingRefId = Guid.NewGuid())
                .With(x => x.CaseType = "Civil Money Claims")
                .With(x => x.ScheduledDateTime = DateTime.Now.ToLocalTime().AddMinutes(2))
                .With(x => x.CaseNumber = $"{GenerateRandom.CaseNumber(fromRandomNumber)}")
                .With(x => x.CaseName = $"{caseName} {GenerateRandom.Letters(fromRandomNumber)}")
                .With(x => x.ScheduledDuration = 120)
                .With(x => x.Participants = new List<ParticipantRequest>())
                .With(x => x.AudioRecordingRequired = false)
                .Build();
        }

        public BookNewConferenceRequestBuilder WithJudge(string firstName = null)
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.First()}{Faker.RandomNumber.Next()}")
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.Username = $"Automation_{Internet.Email()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = UserRole.Judge)
                .With(x => x.ParticipantRefId = Guid.NewGuid())
                .Build();

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                participant.FirstName = firstName;
            }

            _bookNewConferenceRequest.Participants.Add(participant);

            return this;
        }

        public BookNewConferenceRequestBuilder WithRepresentative(string caseTypeGroup = null)
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.FullName()}")
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.Username = $"Automation_{Internet.Email()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = UserRole.Representative)
                .With(x => x.Representee = "Person")
                .With(x => x.ParticipantRefId = Guid.NewGuid())
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
                .With(x => x.Name = $"Automation_{Name.FullName()}")
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.Username = $"Automation_{Internet.Email()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = UserRole.Individual)
                .With(x => x.ParticipantRefId = Guid.NewGuid())
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

        public BookNewConferenceRequestBuilder WithDate(DateTime date)
        {
            _bookNewConferenceRequest.ScheduledDateTime = date;
            return this;
        }

        public BookNewConferenceRequest Build()
        {
            return _bookNewConferenceRequest;
        }

        public BookNewConferenceRequestBuilder WithAudiorecordingRequired(bool audioRecordingRequired = true)
        {
            _bookNewConferenceRequest.AudioRecordingRequired = audioRecordingRequired;
            return this;
        }

        public BookNewConferenceRequestBuilder WithEndpoint(string displayName, string sip, string pin)
        {
            _bookNewConferenceRequest.Endpoints ??= new List<AddEndpointRequest>();
            
            _bookNewConferenceRequest.Endpoints.Add(new AddEndpointRequest {DisplayName = displayName, SipAddress = sip, Pin = pin});
            return this;
        }
    }
}
