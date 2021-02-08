using System.Collections.Generic;

namespace VideoApi.Domain.Validations
{
    public class ValidationFailures : List<ValidationFailure>
    {
        public void AddFailure(string name, string message)
        {
            Add(new ValidationFailure(name, message));
        }
    }
}