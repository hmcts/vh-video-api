using System;
using Bogus;
using FizzWare.NBuilder;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

namespace Testing.Common.Helper.Builders.Api
{
    public class ParticipantRequestBuilder
    {
        private readonly ParticipantRequest _participantRequest;
        private static readonly Faker Faker = new();
        
        public ParticipantRequestBuilder(UserRole userRole)
        {
            _participantRequest = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Faker.Name.FullName()}")
                .With(x => x.ParticipantRefId = Guid.NewGuid())
                .With(x => x.Username = $"Automation_{Faker.Random.Number(0, 99999999 )}@hmcts.net")
                .With(x => x.DisplayName = $"Automation_{Faker.Internet.UserName()}")
                .With(x => x.UserRole = userRole)
                .Build();
        }
        
        public ParticipantRequest Build()
        {
            return _participantRequest;
        }
    }
}
