using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Consts;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.QuickLink
{
    public class GetQuickLinkParticipantByIdTests : QuickLinkControllerTestsBase
    {
        [Test]
        public async Task Should_call_query_handler_to_quick_link_participant()
        {
            //Arrange/Act
            var result = await Controller.GetQuickLinkParticipantByUserName(Guid.NewGuid() + QuickLinkParticipantConst.Domain);

            //Assert
            QueryHandler.Verify(x => x.Handle<GetQuickLinkParticipantByIdQuery, ParticipantBase>(
                It.IsAny<GetQuickLinkParticipantByIdQuery>()), Times.Once);
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            var response = result.As<OkObjectResult>().Value.As<ParticipantSummaryResponse>();
            QuickLinksParticipant.Username.Should().StartWith(response.Username);
        }
        
        [Test]
        public async Task Should_return_not_found_result_ifConference_is_not_found()
        {
            QueryHandler.Setup(x => x.Handle<GetQuickLinkParticipantByIdQuery, ParticipantBase>(
                It.IsAny<GetQuickLinkParticipantByIdQuery>())).ReturnsAsync((ParticipantBase)null);
            
            //Act
            var result =
                await Controller.GetQuickLinkParticipantByUserName(Guid.NewGuid() + QuickLinkParticipantConst.Domain);

            //Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}
