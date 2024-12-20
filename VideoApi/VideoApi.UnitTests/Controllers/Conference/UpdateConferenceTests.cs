using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Services.Dtos;
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

            var query = new GetNonClosedConferenceByHearingRefIdQuery(new [] {request.HearingRefId});
            QueryHandlerMock
                .Setup(x => x.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<VideoApi.Domain.Conference>>(query))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference });

            VideoPlatformServiceMock.Setup(v => v.UpdateVirtualCourtRoomAsync(It.IsAny<Guid>(), It.IsAny<bool>(),
                It.IsAny<List<EndpointDto>>(), It.IsAny<VideoApi.Domain.Enums.ConferenceRoomType>(), It.IsAny<VideoApi.Domain.Enums.AudioPlaybackLanguage>()));

            await Controller.UpdateConferenceAsync(request);

            VideoPlatformServiceMock.Setup(v => v.UpdateVirtualCourtRoomAsync(It.IsAny<Guid>(), It.IsAny<bool>(),
                It.IsAny<List<EndpointDto>>(), It.IsAny<VideoApi.Domain.Enums.ConferenceRoomType>(), It.IsAny<VideoApi.Domain.Enums.AudioPlaybackLanguage>()));
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateConferenceDetailsCommand>()), Times.Once);
            VerifySupplierUsed(TestConference.Supplier, Times.Exactly(1));
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            var request = new UpdateConferenceRequest
            {
                HearingRefId = Guid.NewGuid(),
            };
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetNonClosedConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference>());


            var result = await Controller.UpdateConferenceAsync(request);
            var typedResult = (NotFoundResult) result;
            typedResult.Should().NotBeNull();
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
