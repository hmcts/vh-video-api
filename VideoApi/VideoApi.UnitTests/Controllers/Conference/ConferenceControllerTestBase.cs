using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FizzWare.NBuilder;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Kinly;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Factories;
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
        protected Mock<IOptions<KinlyConfiguration>> KinlyConfiguration;
        protected MeetingRoom MeetingRoom;
        protected VideoApi.Domain.Conference TestConference;
        protected VideoApi.Domain.Conference TestConference2;
        protected VideoApi.Domain.Conference TestConference3;
        protected List<VideoApi.Domain.Conference> TestConferences;
        protected Mock<IAudioPlatformService> AudioPlatformServiceMock;
        protected Mock<IAzureStorageServiceFactory> AzureStorageServiceFactoryMock;
        protected Mock<IAzureStorageService> AzureStorageServiceMock;
        protected Mock<IPollyRetryService> PollyRetryServiceMock;
        protected List<Endpoint> TestEndpoints;
        protected AutoMock Mocker;

        [SetUp]
        public void Setup()
        {
            Mocker = AutoMock.GetLoose();
            QueryHandlerMock = Mocker.Mock<IQueryHandler>();
            CommandHandlerMock = Mocker.Mock<ICommandHandler>();
            MockLogger = Mocker.Mock<ILogger<ConferenceController>>();
            VideoPlatformServiceMock = Mocker.Mock<IVideoPlatformService>();
            ServicesConfiguration = Mocker.Mock<IOptions<ServicesConfiguration>>();
            KinlyConfiguration = Mocker.Mock<IOptions<KinlyConfiguration>>();
            AudioPlatformServiceMock = Mocker.Mock<IAudioPlatformService>();
            AzureStorageServiceFactoryMock = Mocker.Mock<IAzureStorageServiceFactory>();
            AzureStorageServiceMock = Mocker.Mock<IAzureStorageService>();
            PollyRetryServiceMock = Mocker.Mock<IPollyRetryService>();
            TestEndpoints = new List<Endpoint>
            {
                new Endpoint("one", "44564", "1234", "Defence Sol"),
                new Endpoint("two", "867744", "5678", "Defence Sol")
            };

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithParticipant(UserRole.Individual, "Respondent")
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(2))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(2))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(3))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow)
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow)
                .WithEndpoints(TestEndpoints)
                .Build();
            
            TestConference2 = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Respondent")
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(2))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(2))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(3))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow)
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow)
                .WithEndpoints(TestEndpoints)
                .Build();
            
            TestConference3 = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(2))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(2))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.AddDays(3))
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow)
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow)
                .WithEndpoints(TestEndpoints)
                .Build();
            TestConferences = new List<VideoApi.Domain.Conference>();
            TestConferences.Add(TestConference);
            TestConferences.Add(TestConference2);
            TestConferences.Add(TestConference3);

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
                    x.Handle<GetConferencesTodayForAdminByHearingVenueNameQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetConferencesTodayForAdminByHearingVenueNameQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference });

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetExpiredAudiorecordingConferencesQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetExpiredAudiorecordingConferencesQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> {TestConference});

            CommandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));

            ServicesConfiguration.Setup(s => s.Value).Returns(new ServicesConfiguration());
            KinlyConfiguration.Setup(options => options.Value).Returns(Builder<KinlyConfiguration>.CreateNew().Build());

            MeetingRoom = new MeetingRoom($"http://adminuri", $"http://judgeuri", $"http://participanturi", "pexipnode",
                "12345678");
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferenceHearingRoomsByDateQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetConferenceHearingRoomsByDateQuery>()))
                .ReturnsAsync(TestConferences);
            
            Controller = Mocker.Create<ConferenceController>();
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
