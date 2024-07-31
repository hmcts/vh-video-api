using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Moq;
using Testing.Common.Extensions;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;
using VideoApi.Services;
using VideoApi.Services.Contracts;
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
            TestConference.SetProtectedProperty(nameof(TestConference.Supplier), Supplier.Kinly);
            TestConference2.SetProtectedProperty(nameof(TestConference.Supplier), Supplier.Vodafone);
            var kinlyConfiguration = new KinlyConfiguration
            {
                ConferencePhoneNumber = "KinlyConferencePhoneNumber",
                ConferencePhoneNumberWelsh = "KinlyConferencePhoneNumberWelsh"
            };
            var vodafoneConfiguration = new VodafoneConfiguration
            {
                ConferencePhoneNumber = "VodafoneConferencePhoneNumber",
                ConferencePhoneNumberWelsh = "VodafoneConferencePhoneNumberWelsh"
            };
            var kinlyPlatformService = new Mock<IVideoPlatformService>();
            var vodafonePlatformService = new Mock<IVideoPlatformService>();
            kinlyPlatformService.Setup(x => x.GetSupplierConfiguration()).Returns(kinlyConfiguration);
            vodafonePlatformService.Setup(x => x.GetSupplierConfiguration()).Returns(vodafoneConfiguration);
            SupplierPlatformServiceFactoryMock.Setup(x => x.Create(Supplier.Kinly)).Returns(kinlyPlatformService.Object);
            SupplierPlatformServiceFactoryMock.Setup(x => x.Create(Supplier.Vodafone)).Returns(vodafonePlatformService.Object);
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferencesTodayForAdminByHearingVenueNameQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetConferencesTodayForAdminByHearingVenueNameQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference, TestConference2 });

            _conferenceForAdminRequest.HearingVenueNames = hearingVenueNames;
            var result = await Controller.GetConferencesTodayForAdminByHearingVenueNameAsync(_conferenceForAdminRequest);

            result.As<OkObjectResult>().StatusCode.Should().Be((int) HttpStatusCode.OK);
            var response = ((OkObjectResult)result).Value as IEnumerable<ConferenceForAdminResponse>;
            response.Count().Should().Be(2);
            response.Should().ContainEquivalentOf(ConferenceForAdminResponseMapper.MapConferenceToAdminResponse(TestConference, kinlyConfiguration));
            response.Should().ContainEquivalentOf(ConferenceForAdminResponseMapper.MapConferenceToAdminResponse(TestConference2, vodafoneConfiguration));
        }
    }
}
