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
        
        public BookNewConferenceRequestBuilder()
        {
            _bookNewConferenceRequest = Builder<BookNewConferenceRequest>.CreateNew()
                .With(x => x.ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30))
                .With(x => x.Participants = new List<ParticipantRequest>())
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
        
        public BookNewConferenceRequest Build()
        {
            return _bookNewConferenceRequest;
        }
    }
}