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
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Controllers.QuickLink
{
    public class QuickLinkControllerTestsBase
    {
        public Mock<IQueryHandler> QueryHandler;
        public Mock<ICommandHandler> CommandHandler;
        public Mock<IQuickLinksJwtTokenProvider> QuickLinksJwtTokenProvider;
        public Mock<ILogger<QuickLinksController>> Logger;
        public QuickLinksController Controller;
        public Guid HearingId;
        public AddQuickLinkParticipantRequest AddQuickLinkParticipantRequest;
        public VideoApi.Domain.Conference Conference;
        public QuickLinksJwtDetails QuickLinksJwtDetails;

        [SetUp]
        public void SetUp()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            QuickLinksJwtTokenProvider = new Mock<IQuickLinksJwtTokenProvider>();
            Logger = new Mock<ILogger<QuickLinksController>>();

            Conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .Build();

            QueryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(Conference);

            QuickLinksJwtDetails = new QuickLinksJwtDetails("token", DateTime.Today.AddDays(1));
            QuickLinksJwtTokenProvider.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns(QuickLinksJwtDetails);

            HearingId = Guid.NewGuid();
            AddQuickLinkParticipantRequest = new AddQuickLinkParticipantRequest { Name = "Name", UserRole = Contract.Enums.UserRole.QuickLinkParticipant };

            Controller = new QuickLinksController(CommandHandler.Object, QueryHandler.Object, QuickLinksJwtTokenProvider.Object, Logger.Object);
        }
    }
}
