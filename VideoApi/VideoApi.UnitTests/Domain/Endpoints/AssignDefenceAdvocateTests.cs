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
            var defenceAdvocate = "Defence Sol";
            var endpoint = new Endpoint("old name", "123@sip.com", "1234", defenceAdvocate);

            var newDefenceAdvocate = "a@hmcts.net";
            endpoint.AssignDefenceAdvocate(newDefenceAdvocate);

            endpoint.DefenceAdvocate.Should().Be(newDefenceAdvocate);
        }
    }
}
