using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Testing.Common.Assertions
{
    public static class AssertSerializableError
    {
        public static void ContainsKeyAndErrorMessage(this SerializableError error, string key, string errorMessage)
        {
            error.Should().NotBeNull();
            error.ContainsKey(key).Should().BeTrue();
            ((string[])error[key])[0].Should().Be(errorMessage);
        }
    }
}
