using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class AddStaffMemberToConferenceRequestValidation : AbstractValidator<AddStaffMemberRequest>
    {
        
        public AddStaffMemberToConferenceRequestValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.UserRole).NotEmpty();
            RuleFor(x => x.ContactEmail).NotEmpty();
        }
    }
}
