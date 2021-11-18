using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetConferenceByHearingVenueNameForStaffMemberTests : ConferenceControllerTestBase
    {
        private readonly string _hearingVenueName = "MyVenue";
        private readonly ConferenceForStaffMembertWithSelectedVenueRequest _request = new ConferenceForStaffMembertWithSelectedVenueRequest
        {
            HearingVenueNames = new List<string>(),
        };

        [Test]
        public async Task Should_return_ok_result_for_given_conference_id()
        {
            var hearingVenueNames = new List<string>(new string[] { _hearingVenueName });

            _request.HearingVenueNames = hearingVenueNames;

            QueryHandlerMock
            .Setup(x =>
                x.Handle<GetConferencesTodayForStaffMemberByHearingVenueNameQuery, List<VideoApi.Domain.Conference>>(
                    It.IsAny<GetConferencesTodayForStaffMemberByHearingVenueNameQuery>()))
            .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference });

            var result = await Controller.GetConferencesTodayForStaffMemberByHearingVenueName(_request);

            result.As<OkObjectResult>().StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
