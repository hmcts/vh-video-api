using Faker;
using FizzWare.NBuilder;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace Testing.Common.Helper.Builders.Api
{
    public class ParticipantRequestBuilder
    {
        private readonly ParticipantRequest _participantRequest;
        
        public ParticipantRequestBuilder(UserRole userRole)
        {
            _participantRequest = Builder<ParticipantRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.FullName()}")
                .With(x => x.Username = $"Automation_{Internet.Email()}")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = userRole)
                .Build();
        }
        
        public ParticipantRequest Build()
        {
            return _participantRequest;
        }
    }
}