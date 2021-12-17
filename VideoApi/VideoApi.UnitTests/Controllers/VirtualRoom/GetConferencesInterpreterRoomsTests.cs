using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Responses;
using VideoApi.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.VirtualRoom
{
    public class GetConferencesInterpreterRoomsTests
    {
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected VirtualRoomController Controller;
        protected AutoMock Mocker;

        [SetUp]
        public void TestInitialize()
        {
            Mocker = AutoMock.GetLoose();
            QueryHandlerMock = Mocker.Mock<IQueryHandler>();
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferenceInterpreterRoomsByDateQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetConferenceInterpreterRoomsByDateQuery>()))
                .ReturnsAsync(BuildDefaultConference());
            Controller = Mocker.Create<VirtualRoomController>();
        }

        [Test]
        public async Task Should_return_ok_result_with_hearing_rooms_response()
        {
            var result =
                (OkObjectResult)await Controller.GetConferencesInterpreterRoomsAsync(
                    DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();
            var results = result.Value as List<SharedParticipantRoomResponse>;
            results.Should().NotBeNull();
            results.Count().Should().Be(3);
        }

        private static List<VideoApi.Domain.Conference> BuildDefaultConference()
        {
            var testConference1 = new ConferenceBuilder()
                .WithInterpreterRoom()
                .Build();
            var testConference2 = new ConferenceBuilder()
                .WithInterpreterRoom()
                .Build();
            var testConference3 = new ConferenceBuilder()
                .WithInterpreterRoom()
                .Build();
            List<VideoApi.Domain.Conference> testConferences = new List<VideoApi.Domain.Conference>();
            testConferences.Add(testConference1);
            testConferences.Add(testConference2);
            testConferences.Add(testConference3);
            return testConferences;
        }
    }
}
