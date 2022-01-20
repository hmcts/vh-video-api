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
using VideoApi.Contract.Responses;
using VideoApi.Contract.Consts;

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
            var result = await Controller.AddStaffMemberToConferenceAsync(TestConference.Id, _request) as OkObjectResult;
            MockCommandHandler.Verify(c => c.Handle(It.IsAny<AddParticipantsToConferenceCommand>()), Times.Once);

            result.Should().NotBeNull();
            var response = result?.Value.Should().BeAssignableTo<AddStaffMemberResponse>().Which;
            response.ConferenceId.Should().Be(TestConference.Id);
            response.ParticipantDetails.Should().NotBeNull();
            response.ParticipantDetails.HearingRole.Should().Be(HearingRoleName.StaffMember);
            response.ParticipantDetails.LastName.Should().Be(_request.LastName);
            response.ParticipantDetails.CurrentInterpreterRoom.Should().BeNull();
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
