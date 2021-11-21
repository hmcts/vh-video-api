using Faker;
using FizzWare.NBuilder;
using VideoApi.Contract.Consts;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

namespace Testing.Common.Helper.Builders.Api
{
    public class AddStaffMemberRequestBuilder
    {
        private readonly AddStaffMemberRequest _addStaffMemberRequest;
        
        public AddStaffMemberRequestBuilder(UserRole userRole)
        {
            _addStaffMemberRequest = Builder<AddStaffMemberRequest>.CreateNew()
                .With(x => x.Name = $"Automation_{Name.FullName()}")
                .With(x => x.FirstName = Name.First())
                .With(x => x.LastName = Name.Last())
                .With(x => x.Username = $"Automation_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.DisplayName = $"Automation_{Internet.UserName()}")
                .With(x => x.UserRole = userRole)
                .With(x => x.ContactEmail = $"Automation_{RandomNumber.Next()}@hmcts.net")
                .With(x => x.HearingRole = HearingRoleName.StaffMember)
                .Build();
        }
        public AddStaffMemberRequest Build()
        {
            return _addStaffMemberRequest;
        }
    }
}
