using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class RemoveParticipantFromConferenceTests : ParticipantsControllerTestBase
    {
        [SetUp]
        public void TestInitialize()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
        }

        [Test]
        public async Task Should_remove_given_participants_from_conference()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[1];


            await _controller.RemoveParticipantFromConferenceAsync(conferenceId, participant.Id);

            _mockQueryHandler.Verify(m => m.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            _mockCommandHandler.Verify(c => c.Handle(It.IsAny<RemoveParticipantsFromConferenceCommand>()), Times.Once);
        }


        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((Conference)null);

            var result = await _controller.RemoveParticipantFromConferenceAsync(Guid.NewGuid(), Guid.NewGuid());

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_participant()
        {
            var result = await _controller.RemoveParticipantFromConferenceAsync(TestConference.Id, Guid.NewGuid());

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
