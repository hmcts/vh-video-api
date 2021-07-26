using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.Controllers;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.MagicLink
{
    public class AddMagicLinkParticipantTests
    {
        private Mock<IQueryHandler> _queryHandler;
        private Mock<ICommandHandler> _commandHandler;
        private Mock<ILogger<MagicLinksController>> _logger;

        private MagicLinksController _controller;

        private Guid _hearingId;
        private AddMagicLinkParticipantRequest _addMagicLinkParticipantRequest;
        private VideoApi.Domain.Conference _conference;
        
        [SetUp]
        public void SetUp()
        {
            _queryHandler = new Mock<IQueryHandler>();
            _commandHandler = new Mock<ICommandHandler>();
            _logger = new Mock<ILogger<MagicLinksController>>();


            _conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .Build();
            
            _queryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ReturnsAsync(_conference);

            _hearingId = Guid.NewGuid();
            _addMagicLinkParticipantRequest = new AddMagicLinkParticipantRequest { Name = "Name", UserRole = Contract.Enums.UserRole.MagicLinkParticipant };

            _controller = new MagicLinksController(_commandHandler.Object, _queryHandler.Object, _logger.Object);
        }

        [Test]
        public async Task Should_call_query_handler_to_get_conference()
        {
            //Arrange/Act
            await _controller.AddMagicLinkParticipant(_hearingId, _addMagicLinkParticipantRequest);

            //Assert
            _queryHandler.Verify(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_not_found_result_if_conference_is_not_found()
        {
            //Arrange
            _queryHandler.Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                It.IsAny<GetConferenceByHearingRefIdQuery>())).ThrowsAsync(new ConferenceNotFoundException(_hearingId));

            //Act
            var result = await _controller.AddMagicLinkParticipant(_hearingId, _addMagicLinkParticipantRequest) as NotFoundObjectResult;

            //Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.False((bool)result.Value);
        }

        [Test]
        public async Task Should_call_command_handler_to_add_participant_to_conference()
        {
            //Arrange/Act
            await _controller.AddMagicLinkParticipant(_hearingId, _addMagicLinkParticipantRequest);

            //Assert
            _commandHandler.Verify(x => x.Handle(It.Is<AddParticipantsToConferenceCommand>(x =>
                x.ConferenceId == _conference.Id &&
                x.Participants[0].Name == _addMagicLinkParticipantRequest.Name &&
                (int)x.Participants[0].UserRole == (int)_addMagicLinkParticipantRequest.UserRole &&
                x.LinkedParticipants.Count == 0
            )), Times.Once);
        }

        [Test]
        public async Task Should_return_ok_result()
        {
            //Arrange/Act
            var result = await _controller.AddMagicLinkParticipant(_hearingId, _addMagicLinkParticipantRequest) as OkResult;

            //Assert
            Assert.IsInstanceOf<OkResult>(result);
        }
    }
}
