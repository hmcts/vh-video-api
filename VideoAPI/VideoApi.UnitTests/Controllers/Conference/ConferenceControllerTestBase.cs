using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using Video.API.Factories;
using VideoApi.Common.Configuration;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class ConferenceControllerTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected ConferenceController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ILogger<ConferenceController>> MockLogger;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        protected Mock<IOptions<ServicesConfiguration>> ServicesConfiguration;
        protected MeetingRoom MeetingRoom;
        protected VideoApi.Domain.Conference TestConference;
        protected Mock<IAudioPlatformService> AudioPlatformServiceMock;
        protected Mock<IAzureStorageServiceFactory> AzureStorageServiceFactoryMock;
        protected Mock<IAzureStorageService> AzureStorageServiceMock;
        protected Mock<IPollyRetryService> PollyRetryServiceMock;
        protected List<Endpoint> TestEndpoints;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            MockLogger = new Mock<ILogger<ConferenceController>>();
            VideoPlatformServiceMock = new Mock<IVideoPlatformService>();
            ServicesConfiguration = new Mock<IOptions<ServicesConfiguration>>();
            AudioPlatformServiceMock = new Mock<IAudioPlatformService>();
            AzureStorageServiceFactoryMock = new Mock<IAzureStorageServiceFactory>();
            AzureStorageServiceMock = new Mock<IAzureStorageService>();
            PollyRetryServiceMock = new Mock<IPollyRetryService>();
            TestEndpoints = new List<Endpoint>
            {
                new Endpoint("one", "44564", "1234", "Defence Sol"),
                new Endpoint("two", "867744", "5678", "Defence Sol")
            };

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant", null, null, RoomType.ConsultationRoom1)
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithEndpoints(TestEndpoints)
                .Build();


            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(
                        It.IsAny<GetEndpointsForConferenceQuery>()))
                .ReturnsAsync(TestEndpoints);

            QueryHandlerMock
                .Setup(x => x.Handle<GetNonClosedConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                    It.IsAny<GetNonClosedConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(TestConference);

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetExpiredAudiorecordingConferencesQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetExpiredAudiorecordingConferencesQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> {TestConference});

            CommandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));

            ServicesConfiguration.Setup(s => s.Value).Returns(new ServicesConfiguration());

            MeetingRoom = new MeetingRoom($"http://adminuri", $"http://judgeuri", $"http://participanturi", "pexipnode",
                "12345678");

            Controller = new ConferenceController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                VideoPlatformServiceMock.Object, ServicesConfiguration.Object, MockLogger.Object,
                AudioPlatformServiceMock.Object, AzureStorageServiceFactoryMock.Object, PollyRetryServiceMock.Object);
        }
        
        protected void SetupCallToMockRetryService<T>(T expectedReturn)
        {
            PollyRetryServiceMock.Setup(x => x.WaitAndRetryAsync<Exception, T>
                (
                    It.IsAny<int>(), It.IsAny<Func<int, TimeSpan>>(), It.IsAny<Action<int>>(), It.IsAny<Func<T, bool>>(), It.IsAny<Func<Task<T>>>()
                ))
                .Callback(async (int retries, Func<int, TimeSpan> sleepDuration, Action<int> retryAction, Func<T, bool> handleResultCondition, Func<Task<T>> executeFunction) =>
                {
                    sleepDuration(1);
                    retryAction(1);
                    handleResultCondition(expectedReturn);
                    await executeFunction();
                })
                .ReturnsAsync(expectedReturn);
        }
    }
}
