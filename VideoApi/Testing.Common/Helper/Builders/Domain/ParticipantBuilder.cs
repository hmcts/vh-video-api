using System;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Domain
{
    public class ParticipantBuilder
    {
        private readonly BuilderSettings _builderSettings;

        private UserRole _userRole;
        private string _caseTypeGroup;
        private string _hearingRole;
        private TestCallResult _testCallResult;

        public ParticipantBuilder(bool ignoreId = false)
        {
            _userRole = UserRole.Individual;
            _caseTypeGroup = "Applicant";
            _builderSettings = new BuilderSettings();
            if (!ignoreId) return;

            _builderSettings.DisablePropertyNamingFor<Participant, long?>(x => x.TestCallResultId);
            _builderSettings.DisablePropertyNamingFor<Participant, long?>(x => x.CurrentConsultationRoomId);
            _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
            _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
        }

        public ParticipantBuilder WithUserRole(UserRole userRole)
        {
            _userRole = userRole;
            return this;
        }

        public ParticipantBuilder WithCaseTypeGroup(string caseTypeGroup)
        {
            _caseTypeGroup = caseTypeGroup;
            return this;
        }
        
        public ParticipantBuilder WithHearingRole(string hearingRole)
        {
            _hearingRole = hearingRole;
            return this;
        }

        public static  string DetermineHearingRole(UserRole role, string caseTypeGroup)
        {
            return role switch
            {
                UserRole.Judge => "Judge",
                UserRole.Representative => $"{caseTypeGroup} LIP",
                _ => caseTypeGroup
            };
        }

        public ParticipantBuilder WithSelfTestScore(bool passed, TestScore score)
        {
            _testCallResult = new TestCallResult(passed, score);
            return this;
        }

        public Participant Build()
        {
            _hearingRole ??= DetermineHearingRole(_userRole, _caseTypeGroup);
            var name = Name.FullName();

            var participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), name, Name.First(), Name.Last(), name, $"{RandomNumber.Next()}@hmcts.net", _userRole,
                        _hearingRole, _caseTypeGroup, $"{RandomNumber.Next()}@hmcts.net", Phone.Number()))
                .With(x => x.CurrentRoom = null)
                .Build();

            if (_testCallResult != null)
            {
                participant.UpdateTestCallResult(_testCallResult.Passed, _testCallResult.Score);
            }

            return participant;
        }
    }
}
