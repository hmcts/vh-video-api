using System;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Domain;

namespace Testing.Common.Helper.Builders
{
    public class ParticipantBuilder
    {
        private readonly Participant _participant;

        public ParticipantBuilder(string hearingRole, string caseTypeGroup)
        {
            _participant = Builder<Participant>.CreateNew().WithFactory(() =>
                new Participant(Guid.NewGuid(), Name.FullName(), Name.First(), Internet.Email(), hearingRole,
                    caseTypeGroup)).Build();
        }

        public Participant Build()
        {
            return _participant;
        }
    }
}