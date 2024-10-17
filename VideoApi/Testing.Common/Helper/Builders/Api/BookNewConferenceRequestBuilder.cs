using System;
using System.Collections.Generic;
using System.Linq;
using Faker;
using FizzWare.NBuilder;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

namespace Testing.Common.Helper.Builders.Api
{
    public class BookNewConferenceRequestBuilder
    {
        private readonly BookNewConferenceRequest _bookNewConferenceRequest;
        private readonly string _sipAddressStem;

        public BookNewConferenceRequestBuilder(string caseName, string sipAddressStem = null)
        {
            _sipAddressStem = sipAddressStem;
            var fromRandomNumber = new Random();
            _bookNewConferenceRequest = Builder<BookNewConferenceRequest>.CreateNew()
                .With(x => x.HearingRefId = Guid.NewGuid())
                .With(x => x.CaseType = "Generic")
                .With(x => x.ScheduledDateTime = DateTime.Now.ToLocalTime().AddMinutes(2))
                .With(x => x.CaseNumber = $"{GenerateRandom.CaseNumber(fromRandomNumber)}")
                .With(x => x.CaseName = $"{caseName} {GenerateRandom.Letters(fromRandomNumber)}")
                .With(x => x.ScheduledDuration = 120)
                .With(x => x.Participants = new List<ParticipantRequest>())
                .With(x => x.AudioRecordingRequired = false)
                .With(x => x.Endpoints = new List<AddEndpointRequest>())
                .With(x => x.ConferenceRoomType = ConferenceRoomType.VMR)
                .With(x => x.AudioPlaybackLanguage = AudioPlaybackLanguage.EnglishAndWelsh)
                .Build();
        }

        public BookNewConferenceRequestBuilder WithJudge(string firstName = null)
        {
            return WithJudicialParticipant(UserRole.Judge, firstName);
        }
        
        public BookNewConferenceRequestBuilder WithJudicialOfficeHolder(string firstName = null)
        {
            return WithJudicialParticipant(UserRole.JudicialOfficeHolder, firstName);
        }

        private BookNewConferenceRequestBuilder WithJudicialParticipant(UserRole role, string firstName = null)
        {
            var hearingRole = role == UserRole.Judge ? "Judge" : "PanelMember";
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.First()}{RandomNumber.Next()}")
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = role)
                .With(x => x.HearingRole =  hearingRole)
                .With(x => x.ParticipantRefId = Guid.NewGuid())
                .With(x => x.ContactEmail = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.Username = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")
                .Build();

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                participant.FirstName = firstName;
            }

            _bookNewConferenceRequest.Participants.Add(participant);

            return this;
        }

        public BookNewConferenceRequestBuilder WithRepresentative(string caseTypeGroup = "Applicant")
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.FullName()}")
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = UserRole.Representative)
                .With(x => x.CaseTypeGroup = caseTypeGroup)
                .With(x => x.HearingRole = ParticipantBuilder.DetermineHearingRole(VideoApi.Domain.Enums.UserRole.Representative, caseTypeGroup))
                .With(x => x.Representee = "Person")
                .With(x => x.ParticipantRefId = Guid.NewGuid())
                .With(x => x.ContactEmail = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.Username = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")

                .Build();
            _bookNewConferenceRequest.Participants.Add(participant);
            return this;
        }

        public BookNewConferenceRequestBuilder WithIndividual(string caseTypeGroup = "Applicant")
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.FullName()}")
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = UserRole.Individual)
                .With(x => x.CaseTypeGroup = caseTypeGroup)
                .With(x => x.HearingRole = ParticipantBuilder.DetermineHearingRole(VideoApi.Domain.Enums.UserRole.Representative, caseTypeGroup))
                .With(x => x.ParticipantRefId = Guid.NewGuid())
                .With(x => x.ContactEmail = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.Username = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")
                .Build();

            _bookNewConferenceRequest.Participants.Add(participant);
            return this;
        }
        
        public BookNewConferenceRequestBuilder WithIndividualAndInterpreter(string caseTypeGroup = "Applicant")
        {
            var participant = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.FullName()}")
                .With(x => x.FirstName = $"Automation_{Name.First()}")
                .With(x => x.LastName = $"Automation_{Name.Last()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = UserRole.Individual)
                .With(x => x.CaseTypeGroup = caseTypeGroup)
                .With(x => x.HearingRole = ParticipantBuilder.DetermineHearingRole(VideoApi.Domain.Enums.UserRole.Representative, caseTypeGroup))
                .With(x => x.ParticipantRefId = Guid.NewGuid())
                .With(x => x.ContactEmail = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.Username = $"Automation_Video_APi_{RandomNumber.Next()}@hmcts.net")
                .Build();

            var interpreter =
                _bookNewConferenceRequest.Participants.First(x => x.ParticipantRefId != participant.ParticipantRefId);
            
            participant.LinkedParticipants.Add(new LinkedParticipantRequest()
            {
                ParticipantRefId = participant.ParticipantRefId,
                LinkedRefId = interpreter.ParticipantRefId,
                Type = LinkedParticipantType.Interpreter
            });
            
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
            if (_sipAddressStem != null)
            {
                sip = $"{sip}{_sipAddressStem}";
            }
            _bookNewConferenceRequest.Endpoints.Add(new AddEndpointRequest
                {DisplayName = displayName, SipAddress = sip, Pin = pin, DefenceAdvocate = "Defence Sol"});
            return this;
        }

        public BookNewConferenceRequestBuilder WithEndpoints(List<AddEndpointRequest> endpoints)
        {
            _bookNewConferenceRequest.Endpoints.AddRange(endpoints);
            return this;
        }

        public BookNewConferenceRequestBuilder WithConferenceRoomType(ConferenceRoomType roomType)
        {
            _bookNewConferenceRequest.ConferenceRoomType = roomType;
            return this;
        }
        
        public BookNewConferenceRequestBuilder WithAudioPlaybackLanguage(AudioPlaybackLanguage audioPlaybackLanguage)
        {
            _bookNewConferenceRequest.AudioPlaybackLanguage = audioPlaybackLanguage;
            return this;
        }
    }
}
