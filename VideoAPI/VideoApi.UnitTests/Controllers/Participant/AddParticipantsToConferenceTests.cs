using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class AddParticipantsToConferenceTests : ParticipantsControllerTestBase
    {
        private AddParticipantsToConferenceRequest request;

        [SetUp]
        public void TestInitialize()
        {
            request = new AddParticipantsToConferenceRequest
            {
                Participants = new List<ParticipantRequest> { new ParticipantRequestBuilder(UserRole.Individual).Build() }
            };
        }

        [Test]
        public async Task Should_add_participants_to_conference()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);

            await _controller.AddParticipantsToConference(TestConference.Id, request);

            _mockQueryHandler.Verify(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            _mockCommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToConferenceCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((Conference)null);                        
            
            var result = await _controller.AddParticipantsToConference(Guid.NewGuid(), request);

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
