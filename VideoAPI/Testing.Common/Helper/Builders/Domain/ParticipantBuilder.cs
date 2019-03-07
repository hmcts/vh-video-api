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

        public ParticipantBuilder(bool ignoreId = false)
        {
            _userRole = UserRole.Individual;
            _caseTypeGroup = "Claimant";
                
            _builderSettings = new BuilderSettings();
            if (!ignoreId) return;
            
            _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
            _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
        }

        public ParticipantBuilder WithHearingRole(UserRole userRole)
        {
            _userRole = userRole;
            return this;
        }
        
        public ParticipantBuilder WithCaseTypeGroup(string caseTypeGroup)
        {
            _caseTypeGroup = caseTypeGroup;
            return this;
        }

        public Participant Build()
        {
            var participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Internet.Email(), _userRole,
                    _caseTypeGroup)).Build();
            return participant;
        }
    }
}