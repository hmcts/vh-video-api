using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Controllers
{
    public class MessageControllerTestBase
    {
        protected Mock<IQueryHandler> queryHandler;
        protected Mock<ICommandHandler> commandHandler;
        protected Mock<ILogger<MessageController>> logger;
        protected MessageController messageController;
        protected VideoApi.Domain.Conference TestConference;

        [SetUp]
        public void TestInitialize()
        {
            queryHandler = new Mock<IQueryHandler>();
            commandHandler = new Mock<ICommandHandler>();
            logger = new Mock<ILogger<Video.API.Controllers.MessageController>>();
            messageController = new MessageController(queryHandler.Object, commandHandler.Object, logger.Object);

            TestConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant", null, RoomType.ConsultationRoom1)
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithMessages(10)
                .Build();

            var closedConferences = new List<Conference>();
            closedConferences.Add(TestConference);
        }
    }
}
