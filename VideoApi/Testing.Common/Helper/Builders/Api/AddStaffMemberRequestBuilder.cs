using Bogus;
using FizzWare.NBuilder;
using VideoApi.Contract.Consts;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

namespace Testing.Common.Helper.Builders.Api
{
    public class AddStaffMemberRequestBuilder
    {
        private readonly AddStaffMemberRequest _addStaffMemberRequest;

        private static readonly Faker Faker = new();
        
        public AddStaffMemberRequestBuilder(UserRole userRole)
        {
            _addStaffMemberRequest = Builder<AddStaffMemberRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Faker.Name.FullName()}")
                .With(x => x.FirstName = Faker.Name.FirstName())
                .With(x => x.LastName = Faker.Name.LastName())
                .With(x => x.Username = $"Automation_{Faker.Random.Number(0, 99999999)}@hmcts.net")
                .With(x => x.DisplayName = $"Automation_{Faker.Internet.UserName()}")
                .With(x => x.UserRole = userRole)
                .With(x => x.ContactEmail = $"Automation_{Faker.Random.Number(0, 99999999)}@hmcts.net")
                .With(x => x.HearingRole = HearingRoleName.StaffMember)
                .Build();
        }
        public AddStaffMemberRequest Build()
        {
            return _addStaffMemberRequest;
        }
    }
}
