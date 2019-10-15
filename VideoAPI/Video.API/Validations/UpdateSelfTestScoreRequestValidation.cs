using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class UpdateSelfTestScoreRequestValidation : AbstractValidator<UpdateSelfTestScoreRequest>
    {
        public static readonly string NoUsernameErrorMessage =
            "Please provide the username of the person updating the task";

        public UpdateSelfTestScoreRequestValidation()
        {
            
        }
    }
}