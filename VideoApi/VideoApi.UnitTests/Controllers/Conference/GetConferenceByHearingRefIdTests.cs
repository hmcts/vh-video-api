using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Extensions;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetConferenceByHearingRefIdTests : ConferenceControllerTestBase
    {
        [TestCase(Supplier.Kinly)]
        [TestCase(Supplier.Vodafone)]
        public async Task Should_return_ok_result_for_given_valid_hearing_ref_id(Supplier supplier)
        {
            TestConference.SetSupplier(supplier);
            var result = await Controller.GetConferenceByHearingRefIdAsync(Guid.NewGuid());
            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            VerifySupplierUsed(supplier, Times.Exactly(1));
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            QueryHandlerMock
             .Setup(x => x.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetNonClosedConferenceByHearingRefIdQuery>()))
             .ReturnsAsync(new List<VideoApi.Domain.Conference>());
            
            var result = await Controller.GetConferenceByHearingRefIdAsync(Guid.NewGuid());
            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
