using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Common.Security;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Extensions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.QuickLink
{
    public class AddQuickLinkParticipantTests : QuickLinkControllerTestsBase
    {
        [Test]
        public async Task Should_call_query_handler_to_getConference()
        {
            //Arrange/Act
            await Controller.AddQuickLinkParticipant(HearingId, AddQuickLinkParticipantRequest);

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
            var result = await Controller.AddQuickLinkParticipant(HearingId, AddQuickLinkParticipantRequest) as NotFoundObjectResult;

            //Assert
            ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result);
            ClassicAssert.False((bool)result!.Value!);
        }

        [Test]
        public async Task Should_call_command_handler_to_add_participant_toConference()
        {
            //Arrange/Act
            await Controller.AddQuickLinkParticipant(HearingId, AddQuickLinkParticipantRequest);

            //Assert
            CommandHandler.Verify(x => x.Handle(It.Is<AddParticipantsToConferenceCommand>(x =>
                x.ConferenceId == Conference.Id &&
                (int)x.Participants[0].UserRole == (int)AddQuickLinkParticipantRequest.UserRole &&
                x.LinkedParticipants.Count == 0
            )), Times.Once);
        }

        [Test]
        public async Task Should_call_token_provider_to_generate_token()
        {
            //Arrange/Act
            await Controller.AddQuickLinkParticipant(HearingId, AddQuickLinkParticipantRequest);

            //Assert
            QuickLinksJwtTokenProvider.Verify(x => x.GenerateToken(
                AddQuickLinkParticipantRequest.Name,
                It.IsAny<string>(),
                AddQuickLinkParticipantRequest.UserRole.MapToDomainEnum()), Times.Once);
        }

        [Test]
        public async Task Should_call_command_handler_to_add_participant_token()
        {
            //Arrange/Act
            await Controller.AddQuickLinkParticipant(HearingId, AddQuickLinkParticipantRequest);

            //Assert
            CommandHandler.Verify(x => x.Handle(It.IsAny<AddQuickLinkParticipantTokenCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_ok_result_containing_token()
        {
            //Arrange/Act
            var result = await Controller.AddQuickLinkParticipant(HearingId, AddQuickLinkParticipantRequest) as OkObjectResult;

            //Assert
            ClassicAssert.IsInstanceOf<OkObjectResult>(result);
            var quickLinkResponse = result?.Value.Should().BeAssignableTo<AddQuickLinkParticipantResponse>().Which;
            quickLinkResponse?.Token.Should().Be(QuickLinksJwtDetails.Token);
            quickLinkResponse?.ConferenceId.Should().Be(Conference.Id);
            quickLinkResponse?.Participant.Should().NotBeNull();
            quickLinkResponse?.Participant.DisplayName.Should().Be(AddQuickLinkParticipantRequest.Name);
            quickLinkResponse?.Participant.UserRole.Should().Be(AddQuickLinkParticipantRequest.UserRole);
        }
    }
}
