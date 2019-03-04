using System;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Domain;

namespace Testing.Common.Helper.Builders
{
    public class ParticipantBuilder
    {
        private readonly Participant _participant;
        private readonly BuilderSettings _builderSettings;

        public ParticipantBuilder(string hearingRole, string caseTypeGroup, bool ignoreId = false)
        {
            _builderSettings = new BuilderSettings();
            if (ignoreId)
            {
                _builderSettings.DisablePropertyNamingFor<Participant, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<ParticipantStatus, long>(x => x.Id);
                _builderSettings.DisablePropertyNamingFor<ConferenceStatus, long>(x => x.Id);
            }
            
            _participant = new Builder(_builderSettings).CreateNew<Participant>().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Internet.Email(), hearingRole,
                    caseTypeGroup)).Build();
        }

        public Participant Build()
        {
            return _participant;
        }
    }
}