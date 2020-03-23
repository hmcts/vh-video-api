using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class UpdateConferenceTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_update_requested_conference_successfully()
        {
            var request = new UpdateConferenceRequest
            {
                HearingRefId = Guid.NewGuid(),
                CaseName = "CaseName",
                ScheduledDateTime = DateTime.Now,
                ScheduledDuration = 10,
                CaseType = "CaseType",
                CaseNumber = "CaseNo"
            };

            await Controller.UpdateConferenceAsync(request);

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateConferenceDetailsCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_notfound_with_conferencenotfoundexception()
        {
            CommandHandlerMock.Setup(c => c.Handle(It.IsAny<UpdateConferenceDetailsCommand>())).Throws(new ConferenceNotFoundException(Guid.NewGuid()));
            var request = new UpdateConferenceRequest();

            var result = await Controller.UpdateConferenceAsync(request);

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateConferenceDetailsCommand>()), Times.Once);
        }
    }
}
