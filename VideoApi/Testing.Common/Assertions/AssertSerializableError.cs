using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Testing.Common.Assertions
{
    public static class AssertSerializableError
    {
        public static void ContainsValidationErrors(this ObjectResult objectResult)
        {
            ((ValidationProblemDetails)objectResult.Value).Errors.Should().NotBeEmpty();
        }
        
        public static void ContainsKeyAndErrorMessage(this ValidationProblemDetails error, string key, string errorMessage)
        {
            error.Should().NotBeNull();
            error.Errors.ContainsKey(key).Should().BeTrue();
            error.Errors[key][0].Should().Be(errorMessage);
        }
    }
}
