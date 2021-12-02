using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
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
            await Controller.GetQuickLinkParticipantByUserName(Guid.NewGuid()+QuickLinkParticipantConst.Domain);

            //Assert
            QueryHandler.Verify(x => x.Handle<GetQuickLinkParticipantByIdQuery, ParticipantBase>(
                It.IsAny<GetQuickLinkParticipantByIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_not_found_result_ifConference_is_not_found()
        {
           //Act
            var result = await Controller.GetQuickLinkParticipantByUserName(Guid.NewGuid()+QuickLinkParticipantConst.Domain);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            var response = result.As<OkObjectResult>().Value.As<ParticipantSummaryResponse>();
            response.Username.Should().Equals(QuickLinksParticipant.Username);
        }
    }
}
