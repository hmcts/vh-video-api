using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class UpdateAlertRequestValidation : AbstractValidator<UpdateAlertRequest>
    {
        public static readonly string NoUsernameErrorMessage =
            "Please provide the username of the person updating the alert";

        public UpdateAlertRequestValidation()
        {
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage(NoUsernameErrorMessage);
        }
    }
}