using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.DAL.Commands;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class AnonymiseParticipantWithUsernameTests : ParticipantsControllerTestBase
    {
        [Test]
        public async Task Returns_Ok_For_Successful_Request()
        {
            var usernameToAnonymise = "john.doe@email.net";

            var response = await Controller.AnonymiseParticipantWithUsername(usernameToAnonymise) as OkResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            MockCommandHandler.Verify(
                commandHandler => commandHandler.Handle(
                    It.Is<AnonymiseParticipantWithUsernameCommand>(command => command.Username == usernameToAnonymise)),
                Times.Once);
        }
    }
}
