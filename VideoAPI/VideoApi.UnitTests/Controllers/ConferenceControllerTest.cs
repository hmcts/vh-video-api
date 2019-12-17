using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers
{
    [TestFixture]
    public class ConferenceControllerTest
    {
        private readonly Mock<IQueryHandler> _queryHandler;
        private readonly Mock<ICommandHandler> _commandHandler;
        private readonly Mock<IVideoPlatformService> _videoPlatformService;
        private readonly Mock<IOptions<ServicesConfiguration>> _servicesConfiguration;
        private readonly Mock<ILogger<ConferenceController>> _logger;

        private readonly ConferenceController _conferenceController;

        public ConferenceControllerTest()
        {
            _queryHandler = new Mock<IQueryHandler>();
            _commandHandler = new Mock<ICommandHandler>();
            _videoPlatformService = new Mock<IVideoPlatformService>();
            _logger = new Mock<ILogger<ConferenceController>>();
            _servicesConfiguration = new Mock<IOptions<ServicesConfiguration>>();

            _conferenceController = new ConferenceController
            (
                _queryHandler.Object, _commandHandler.Object, _videoPlatformService.Object,
                _servicesConfiguration.Object, _logger.Object
            );
        }

        [Test]
        public async Task GetOpenConferencesByScheduledDate_Returns_Ok()
        {
            var scheduledDate = DateTime.Now;
            var conferences = new List<Conference>
            {
                new Conference(Guid.NewGuid(), "casetype", scheduledDate, "123", "casename", 60)
            };
            
            _queryHandler
                .Setup(x => 
                x.Handle<GetOpenConferencesByDateTimeQuery, List<Conference>>
                (
                    It.Is<GetOpenConferencesByDateTimeQuery>(query => query.ScheduledDateTime == scheduledDate))
                )
                .ReturnsAsync(conferences);
            
            var result = await _conferenceController.GetOpenConferencesByScheduledDate(scheduledDate);

            result.Should().NotBeNull();
            result.Should()
                .BeAssignableTo<OkObjectResult>().Subject.Value.Should()
                .NotBeNull().And
                .BeAssignableTo<IEnumerable<ConferenceSummaryResponse>>()
                .And.Subject.As<IEnumerable<ConferenceSummaryResponse>>().Should().HaveCount(conferences.Count);
            
            var resultValue = result.As<OkObjectResult>().Value.As<IEnumerable<ConferenceSummaryResponse>>();
            
            Assert.NotNull(resultValue);
            Assert.AreEqual(resultValue.ElementAt(0).Id, conferences.ElementAt(0).Id);
            Assert.AreEqual(resultValue.ElementAt(0).CaseType, conferences.ElementAt(0).CaseType);
            Assert.AreEqual(resultValue.ElementAt(0).CaseNumber, conferences.ElementAt(0).CaseNumber);
            Assert.AreEqual(resultValue.ElementAt(0).ScheduledDateTime, conferences.ElementAt(0).ScheduledDateTime);
            Assert.AreEqual(resultValue.ElementAt(0).ScheduledDuration, conferences.ElementAt(0).ScheduledDuration);
        }
    }
}