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
    }
}
