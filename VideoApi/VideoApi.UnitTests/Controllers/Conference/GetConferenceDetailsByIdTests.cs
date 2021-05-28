using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
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

            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_not_found_with_no_matching_conference()
        {
            var hearingVenueNames = new List<string>(new string[] { It.IsAny<string>() });

            _conferenceForAdminRequest.HearingVenueNames = hearingVenueNames;
            var result = await Controller.GetConferencesTodayForAdminByHearingVenueNameAsync(_conferenceForAdminRequest);

            var typedResult = (OkObjectResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
