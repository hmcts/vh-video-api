using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Common.Security;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Extensions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.MagicLink
{
    public class AddMagicLinkParticipantTests : MagicLinkControllerTestsBase
    {
        [Test]
        public async Task Should_call_query_handler_to_getConference()
        {
            //Arrange/Act
            await Controller.AddMagicLinkParticipant(HearingId, AddMagicLinkParticipantRequest);

            //Assert
            QueryHandler.Verify(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_not_found_result_ifConference_is_not_found()
        {
            //Arrange
            QueryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ThrowsAsync(new ConferenceNotFoundException(HearingId));

            //Act
            var result = await Controller.AddMagicLinkParticipant(HearingId, AddMagicLinkParticipantRequest) as NotFoundObjectResult;

            //Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.False((bool)result.Value);
        }

        [Test]
        public async Task Should_call_command_handler_to_add_participant_toConference()
        {
            //Arrange/Act
            await Controller.AddMagicLinkParticipant(HearingId, AddMagicLinkParticipantRequest);

            //Assert
            CommandHandler.Verify(x => x.Handle(It.Is<AddParticipantsToConferenceCommand>(x =>
                x.ConferenceId == Conference.Id &&
                x.Participants[0].Name == AddMagicLinkParticipantRequest.Name &&
                (int)x.Participants[0].UserRole == (int)AddMagicLinkParticipantRequest.UserRole &&
                x.LinkedParticipants.Count == 0
            )), Times.Once);
        }

        [Test]
        public async Task Should_call_token_provider_to_generate_token()
        {
            //Arrange/Act
            await Controller.AddMagicLinkParticipant(HearingId, AddMagicLinkParticipantRequest);

            //Assert
            MagicLinksJwtTokenProvider.Verify(x => x.GenerateToken(
                AddMagicLinkParticipantRequest.Name,
                It.IsAny<string>(),
                AddMagicLinkParticipantRequest.UserRole.MapToDomainEnum()), Times.Once);
        }

        [Test]
        public async Task Should_call_command_handler_to_add_participant_token()
        {
            //Arrange/Act
            await Controller.AddMagicLinkParticipant(HearingId, AddMagicLinkParticipantRequest);

            //Assert
            CommandHandler.Verify(x => x.Handle(It.IsAny<AddMagicLinkParticipantTokenCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_ok_result_containing_token()
        {
            //Arrange/Act
            var result = await Controller.AddMagicLinkParticipant(HearingId, AddMagicLinkParticipantRequest) as OkObjectResult;

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(result.Value, MagicLinksJwtDetails.Token);
        }
    }
}
