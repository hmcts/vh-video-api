using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsTests : ParticipantsControllerTestBase
    {
        [Test]
        public async Task Returns_Ok_For_Successful_Request()
        {
            var request = new AnonymiseQuickLinkParticipantWithHearingIdsRequest
                { HearingIds = new List<Guid> { Guid.NewGuid() } };

            var response = await Controller.AnonymiseQuickLinkParticipantWithHearingIds(request) as OkResult;

            response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            MockCommandHandler.Verify(
                commandHandler => commandHandler.Handle(
                    It.Is<AnonymiseQuickLinkParticipantWithHearingIdsCommand>(command => command.HearingIds == request.HearingIds)),
                Times.Once);
        }
    }
}
