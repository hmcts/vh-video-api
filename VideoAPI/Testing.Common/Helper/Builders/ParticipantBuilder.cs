using System;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Domain;

namespace Testing.Common.Helper.Builders
{
    public class ParticipantBuilder
    {
        private readonly BuilderSettings _builderSettings;

        private string _hearingRole;
        private string _caseTypeGroup;

        public ParticipantBuilder(bool ignoreId = false)
        {
            _hearingRole = "Claimant LIP";
            _caseTypeGroup = "Claimant";
                
            _builderSettings = new BuilderSettings();
            if (!ignoreId) return;
            
            _builderSettings.DisablePropertyNamingFor<Participant, long>(x => x.Id);
            _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
            _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
        }

        public ParticipantBuilder WithHearingRole(string hearingRole)
        {
            _hearingRole = hearingRole;
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
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Internet.Email(), _hearingRole,
                    _caseTypeGroup)).Build();
            return participant;
        }
    }
}