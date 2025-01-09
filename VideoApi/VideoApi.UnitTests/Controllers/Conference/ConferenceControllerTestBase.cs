using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Enums;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Factories;
using ConferenceState = VideoApi.Domain.Enums.ConferenceState;
using RoomType = VideoApi.Domain.Enums.RoomType;
using Task = System.Threading.Tasks.Task;
using UserRole = VideoApi.Domain.Enums.UserRole;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class ConferenceControllerTestBase
    {
        protected const string AppName = "vh-recording-app";
        protected Mock<IAudioPlatformService> AudioPlatformServiceMock;
        protected Mock<IAzureStorageServiceFactory> AzureStorageServiceFactoryMock;
        protected Mock<IAzureStorageService> AzureStorageServiceMock;
        protected Mock<IBookingService> BookingServiceMock;
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected ConferenceController Controller;
        
        protected KinlyConfiguration KinlyConfig = new()
        {
            PexipSelfTestNode = "KinlyPexipSelfTestNode"
        };
        
        protected MeetingRoom MeetingRoom;
        protected AutoMock Mocker;
        protected Mock<ILogger<ConferenceController>> MockLogger;
        protected Mock<IPollyRetryService> PollyRetryServiceMock;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<IOptions<ServicesConfiguration>> ServicesConfiguration;
        protected Mock<SupplierConfiguration> SupplierConfiguration;
        protected Mock<ISupplierPlatformServiceFactory> SupplierPlatformServiceFactoryMock;
        protected VideoApi.Domain.Conference TestConference;
        protected VideoApi.Domain.Conference TestConference2;
        protected VideoApi.Domain.Conference TestConference3;
        protected List<VideoApi.Domain.Conference> TestConferences;
        protected List<Endpoint> TestEndpoints;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        
        protected VodafoneConfiguration VodafoneConfig = new()
        {
            PexipSelfTestNode = "VodafonePexipSelfTestNode"
        };
        
        [SetUp]
        public void Setup()
        {
            Mocker = AutoMock.GetLoose();
            QueryHandlerMock = Mocker.Mock<IQueryHandler>();
            CommandHandlerMock = Mocker.Mock<ICommandHandler>();
            MockLogger = Mocker.Mock<ILogger<ConferenceController>>();
            VideoPlatformServiceMock = Mocker.Mock<IVideoPlatformService>();
            SupplierPlatformServiceFactoryMock = Mocker.Mock<ISupplierPlatformServiceFactory>();
            SupplierPlatformServiceFactoryMock.Setup(x => x.Create(It.IsAny<VideoApi.Domain.Enums.Supplier>()))
                .Returns(VideoPlatformServiceMock.Object);
            ServicesConfiguration = Mocker.Mock<IOptions<ServicesConfiguration>>();
            SupplierConfiguration = Mocker.Mock<SupplierConfiguration>();
            AudioPlatformServiceMock = Mocker.Mock<IAudioPlatformService>();
            AzureStorageServiceFactoryMock = Mocker.Mock<IAzureStorageServiceFactory>();
            AzureStorageServiceMock = Mocker.Mock<IAzureStorageService>();
            PollyRetryServiceMock = Mocker.Mock<IPollyRetryService>();
            BookingServiceMock = Mocker.Mock<IBookingService>();
            
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
            
            HearingAudioRoom hearingAudioRoom1 = new HearingAudioRoom()
                { HearingRefId = Guid.NewGuid(), FileNamePrefix = string.Empty, Label = string.Empty };
            HearingAudioRoom hearingAudioRoom2 = new HearingAudioRoom()
                { HearingRefId = Guid.NewGuid(), FileNamePrefix = string.Empty, Label = string.Empty };
            
            List<HearingAudioRoom> hearingAudioRooms = new List<HearingAudioRoom>();
            hearingAudioRooms.Add(hearingAudioRoom1);
            hearingAudioRooms.Add(hearingAudioRoom2);
            
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
                .Setup(x => x.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<VideoApi.Domain.Conference>>(
                    It.IsAny<GetNonClosedConferenceByHearingRefIdQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference });
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferencesTodayQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetConferencesTodayQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference });
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetExpiredAudiorecordingConferencesQuery, List<VideoApi.Domain.Conference>>(
                        It.IsAny<GetExpiredAudiorecordingConferencesQuery>()))
                .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference });
            
            
            CommandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));
            
            ServicesConfiguration.Setup(s => s.Value).Returns(new ServicesConfiguration());
            VideoPlatformServiceMock.Setup(x => x.GetSupplierConfiguration()).Returns(SupplierConfiguration.Object);
            
            MeetingRoom = new MeetingRoom($"http://adminuri", $"http://judgeuri", $"http://participanturi", "pexipnode",
                "12345678");
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferenceHearingRoomsByDateQuery, List<VideoApi.Domain.HearingAudioRoom>>(
                        It.IsAny<GetConferenceHearingRoomsByDateQuery>()))
                .ReturnsAsync(hearingAudioRooms);
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetConferenceInterpreterRoomsByDateQuery, List<VideoApi.Domain.HearingAudioRoom>>(
                        It.IsAny<GetConferenceInterpreterRoomsByDateQuery>()))
                .ReturnsAsync(hearingAudioRooms);
            
            Controller = Mocker.Create<ConferenceController>();
        }
        
        protected void SetupCallToMockRetryService<T>(T expectedReturn)
        {
            PollyRetryServiceMock.Setup(x => x.WaitAndRetryAsync<Exception, T>
                (
                    It.IsAny<int>(), It.IsAny<Func<int, TimeSpan>>(), It.IsAny<Action<int>>(),
                    It.IsAny<Func<T, bool>>(), It.IsAny<Func<Task<T>>>()
                ))
                .Callback(async (int retries, Func<int, TimeSpan> sleepDuration, Action<int> retryAction,
                    Func<T, bool> handleResultCondition, Func<Task<T>> executeFunction) =>
                {
                    sleepDuration(1);
                    retryAction(1);
                    handleResultCondition(expectedReturn);
                    await executeFunction();
                })
                .ReturnsAsync(expectedReturn);
        }
        
        protected void UseSupplierPlatformServiceStub()
        {
            SupplierPlatformServiceFactoryMock
                .Setup(x => x.Create(VideoApi.Domain.Enums.Supplier.Vodafone))
                .Returns(new SupplierPlatformServiceStub(VodafoneConfig));
        }
        
        protected void VerifySupplierUsed(Supplier supplier, Times times)
        {
            VerifySupplierUsed((VideoApi.Domain.Enums.Supplier)supplier, times);
        }
        
        protected void VerifySupplierUsed(VideoApi.Domain.Enums.Supplier supplier, Times times)
        {
            SupplierPlatformServiceFactoryMock.Verify(x => x.Create(supplier), times);
        }
    }
}
