using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using VideoApi.Contract.Requests;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class GetConferencesTodayByHearingVenueNameTests : ConferenceControllerTestBase
    {
        private readonly  string _hearingVenueName = "MyVenue";
        private readonly ConferenceForAdminRequest _conferenceForAdminRequest = new ConferenceForAdminRequest
        {
            HearingVenueNames = new List<string>(),
        };
        [Test]
        public async Task Should_return_ok_result_for_given_conference_id()
        {

            var hearingVenueNames = new List<string>(new string[] { _hearingVenueName });

            _conferenceForAdminRequest.HearingVenueNames = hearingVenueNames;
            var result = await Controller.GetConferencesTodayForAdminByHearingVenueNameAsync(_conferenceForAdminRequest);

            result.As<OkObjectResult>().StatusCode.Should().Be((int) HttpStatusCode.OK);
        }
    }
}
