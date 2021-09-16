using FluentValidation;
using VideoApi.Contract.Requests;
using System.Text.RegularExpressions;

namespace VideoApi.Validations
{
    public class AddQuickLinkParticipantRequestValidation : AbstractValidator<AddQuickLinkParticipantRequest>
    {
        public static readonly string NoNameErrorMessage = "Name is required";
        public static readonly string NoUserRoleErrorMessage = "UserRole is required";
        public static readonly string SpecialCharNameErrorMessage = @"Name must not contain the following characters ! ” # $ % & ( ) * + , . / : ; < = > ? @ [ \ ] ^ _ ` { | } ~";

        public static readonly Regex RegexName = new Regex(@"^[^!”#$%&()*+,./:;<=>?@[\\\]^_`{|}~]+$");

        public AddQuickLinkParticipantRequestValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(NoNameErrorMessage);
            RuleFor(x => x.Name).Matches(RegexName.ToString()).WithMessage(SpecialCharNameErrorMessage).When(x => !string.IsNullOrEmpty(x.Name) ); ;
            RuleFor(x => x.UserRole).NotEmpty().WithMessage(NoUserRoleErrorMessage);
        }
    }
}
