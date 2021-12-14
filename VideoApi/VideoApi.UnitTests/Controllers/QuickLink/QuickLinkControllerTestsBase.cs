using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Common.Security;
using VideoApi.Contract.Requests;
using VideoApi.Controllers;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Controllers.QuickLink
{
    public class QuickLinkControllerTestsBase
    {
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<ICommandHandler> CommandHandler;
        protected Mock<IQuickLinksJwtTokenProvider> QuickLinksJwtTokenProvider;
        private Mock<ILogger<QuickLinksController>> _logger;
        protected QuickLinksController Controller;
        protected Guid HearingId;
        protected AddQuickLinkParticipantRequest AddQuickLinkParticipantRequest;
        protected VideoApi.Domain.Conference Conference;
        protected QuickLinksJwtDetails QuickLinksJwtDetails;
        protected QuickLinkParticipant QuickLinksParticipant;

        [SetUp]
        public void SetUp()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            QuickLinksJwtTokenProvider = new Mock<IQuickLinksJwtTokenProvider>();
            _logger = new Mock<ILogger<QuickLinksController>>();

            Conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .Build();

            QuickLinksParticipant = new QuickLinkParticipant("DisplayName", UserRole.QuickLinkParticipant);

            QueryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(Conference);
            
            QueryHandler.Setup(x => x.Handle<GetQuickLinkParticipantByIdQuery, ParticipantBase>(
                It.IsAny<GetQuickLinkParticipantByIdQuery>())).ReturnsAsync(QuickLinksParticipant);

            QuickLinksJwtDetails = new QuickLinksJwtDetails("token", DateTime.Today.AddDays(1));
            QuickLinksJwtTokenProvider.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns(QuickLinksJwtDetails);

            HearingId = Guid.NewGuid();
            AddQuickLinkParticipantRequest = new AddQuickLinkParticipantRequest { Name = "Name", UserRole = Contract.Enums.UserRole.QuickLinkParticipant };

            Controller = new QuickLinksController(CommandHandler.Object, QueryHandler.Object, QuickLinksJwtTokenProvider.Object, _logger.Object);
        }
    }
}
