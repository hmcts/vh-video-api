using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using Task = System.Threading.Tasks.Task;


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
               .WithMessages(2)
               .Build();
        }
    }
}
