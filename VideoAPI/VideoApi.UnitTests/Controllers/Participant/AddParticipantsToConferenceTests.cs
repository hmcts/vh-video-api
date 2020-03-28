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
            await _controller.AddParticipantsToConferenceAsync(TestConference.Id, request);

            _mockCommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToConferenceCommand>()), Times.Once);
        }
    }
}
