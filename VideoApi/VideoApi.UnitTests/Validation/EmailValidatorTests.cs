using Faker;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Validations;

namespace VideoApi.UnitTests.Validation
{
    public class EmailValidatorTests
    {
        [Test]
        public void Should_pass_validation_with_good_email()
        {
            var email = Internet.Email();
            email.IsValidEmail().Should().BeTrue();
        }

        [Test]
        public void Should_fail_validation_when_empty()
        {
            var email = string.Empty;
            email.IsValidEmail().Should().BeFalse();
        }

        [Test]
        public void Should_fail_validation_when_format_is_invalid()
        {
            var email = "uhfiudshf";
            email.IsValidEmail().Should().BeFalse();
        }
    }
}
