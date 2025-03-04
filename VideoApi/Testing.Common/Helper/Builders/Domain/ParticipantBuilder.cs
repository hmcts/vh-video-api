using System;
using Bogus;
using FizzWare.NBuilder;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Domain
{
    public class ParticipantBuilder
    {
        private readonly BuilderSettings _builderSettings;

        private static readonly Faker Faker = new();

        private UserRole _userRole;
        private string _hearingRole;
        private TestCallResult _testCallResult;

        public ParticipantBuilder(bool ignoreId = false)
        {
            _userRole = UserRole.Individual;
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

        public ParticipantBuilder WithHearingRole(string hearingRole)
        {
            _hearingRole = hearingRole;
            return this;
        }

        public static  string DetermineHearingRole(UserRole role, string hearingRole)
        {
            return role switch
            {
                UserRole.Judge => "Judge",
                UserRole.Representative => $"{hearingRole} LIP",
                _ => hearingRole
            };
        }

        public Participant Build()
        {
            _hearingRole ??= DetermineHearingRole(_userRole, _hearingRole);
            var name = Faker.Name.FullName();

            var participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                    new Participant(Guid.NewGuid(), name, $"{Faker.Random.Number(0, 99999999)}@hmcts.net", _userRole,
                        _hearingRole, $"{Faker.Random.Number(0, 99999999)}@hmcts.net"))
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
