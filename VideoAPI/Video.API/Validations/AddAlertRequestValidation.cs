using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class AddAlertRequestValidation : AbstractValidator<AddAlertRequest>
    {
        public static readonly string NoBodyErrorMessage = "Please provide the body for the alert";
        public static readonly string NoAlertTypeErrorMessage = "Please provide the alert type";
        public static readonly string NoUsernameErrorMessage =
            "Please provide the username of the person updating the alert";

        public AddAlertRequestValidation()
        {
            RuleFor(x => x.Type).NotNull().WithMessage(NoAlertTypeErrorMessage);
            RuleFor(x => x.Body).NotEmpty().WithMessage(NoBodyErrorMessage);
            RuleFor(x => x.UpdatedBy).EmailAddress().When(x => x.AlertId.HasValue).WithMessage(NoUsernameErrorMessage);
        }
    }
}