using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.TestCallResults
{
    public class ValidateInstance
    {
        [Test]
        public void Should_initialise_correctly()
        {
            var passed = false;
            var score = TestScore.Bad;

            var testCallResult = new TestCallResult(passed, score);
            testCallResult.Passed.Should().Be(passed);
            testCallResult.Score.Should().Be(score);
        }
    }
}