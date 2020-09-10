using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Domain.Endpoints
{
    public class AssignDefenceAdvocateTests
    {
        [Test]
        public void should_assign_defence_advocate()
        {
            var endpoint = new Endpoint("old name", "123@sip.com", "1234");
            endpoint.DefenceAdvocate.Should().BeNull();

            var defenceUsername = "a@test.com";
            endpoint.AssignDefenceAdvocate(defenceUsername);

            endpoint.DefenceAdvocate.Should().Be(defenceUsername);
        }
    }
}
