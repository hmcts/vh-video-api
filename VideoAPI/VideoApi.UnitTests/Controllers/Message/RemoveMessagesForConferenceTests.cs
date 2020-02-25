using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using Task = System.Threading.Tasks.Task;

using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.DAL.Commands;

namespace VideoApi.UnitTests.Controllers.Message
{
    public class RemoveMessagesForConferenceTests : MessageControllerTestBase
    {
        [Test]
        public async Task Should_return_badrequest_with_invalid_conference_id()
        {
            var result = await messageController.RemoveChatMessagesForConference(Guid.Empty);

            var typedResult = (BadRequestObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            queryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((Conference)null);

            var result = await messageController.RemoveChatMessagesForConference(Guid.NewGuid());

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_remove_messages_from_conference()
        {
            var conferenceId = TestConference.Id;

            queryHandler
              .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
              .ReturnsAsync(TestConference);

            await messageController.RemoveChatMessagesForConference(conferenceId);

            queryHandler.Verify(m => m.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            commandHandler.Verify(c => c.Handle(It.IsAny<RemoveMessagesForConferenceCommand>()), Times.Once);
        }



    }
}
