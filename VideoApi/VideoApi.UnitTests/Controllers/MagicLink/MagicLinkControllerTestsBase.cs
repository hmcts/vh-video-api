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

namespace VideoApi.UnitTests.Controllers.MagicLink
{
    public class MagicLinkControllerTestsBase
    {
        public Mock<IQueryHandler> QueryHandler;
        public Mock<ICommandHandler> CommandHandler;
        public Mock<IMagicLinksJwtTokenProvider> MagicLinksJwtTokenProvider;
        public Mock<ILogger<MagicLinksController>> Logger;
        public MagicLinksController Controller;
        public Guid HearingId;
        public AddMagicLinkParticipantRequest AddMagicLinkParticipantRequest;
        public VideoApi.Domain.Conference Conference;
        public MagicLinksJwtDetails MagicLinksJwtDetails;

        [SetUp]
        public void SetUp()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            MagicLinksJwtTokenProvider = new Mock<IMagicLinksJwtTokenProvider>();
            Logger = new Mock<ILogger<MagicLinksController>>();

            Conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .Build();

            QueryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(Conference);

            MagicLinksJwtDetails = new MagicLinksJwtDetails("token", DateTime.Today.AddDays(1));
            MagicLinksJwtTokenProvider.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns(MagicLinksJwtDetails);

            HearingId = Guid.NewGuid();
            AddMagicLinkParticipantRequest = new AddMagicLinkParticipantRequest { Name = "Name", UserRole = Contract.Enums.UserRole.MagicLinkParticipant };

            Controller = new MagicLinksController(CommandHandler.Object, QueryHandler.Object, MagicLinksJwtTokenProvider.Object, Logger.Object);
        }
    }
}
