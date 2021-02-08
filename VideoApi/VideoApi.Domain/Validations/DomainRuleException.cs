using System;

namespace VideoApi.Domain.Validations
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class DomainRuleException : Exception
    {
        public DomainRuleException(ValidationFailures validationFailures)
        {
            ValidationFailures = validationFailures;
        }

        public DomainRuleException(string name, string message)
        {
            ValidationFailures.Add(new ValidationFailure(name, message));
        }

        public ValidationFailures ValidationFailures { get; protected set; } = new ValidationFailures();
    }
}