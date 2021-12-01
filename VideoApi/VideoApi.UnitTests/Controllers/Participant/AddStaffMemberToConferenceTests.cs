using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Exceptions;
using VideoApi.Contract.Enums;
using VideoApi.DAL.Commands;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class AddStaffMemberToConferenceTests : ParticipantsControllerTestBase
    {
        private AddStaffMemberRequest _request;

        [SetUp]
        public void TestInitialize()
        {
            _request = new AddStaffMemberRequestBuilder(UserRole.Individual).Build();
        }

        [Test]
        public async Task Should_add_participants_to_conference()
        {
            var result = await Controller.AddStaffMemberToConferenceAsync(TestConference.Id, _request);

            MockCommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToConferenceCommand>()), Times.Once);
            var typedResult = (NoContentResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            MockCommandHandler
                .Setup(
                    x => x.Handle(It.IsAny<AddParticipantsToConferenceCommand>()))
                .ThrowsAsync(new ConferenceNotFoundException(TestConference.Id));                     
            
            var result = await Controller.AddStaffMemberToConferenceAsync(Guid.NewGuid(), _request);

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}