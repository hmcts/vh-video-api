using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services
{
    public class ConsultationServiceTests
    {
        private ConsultationService _consultationService;
        private Mock<IKinlyApiClient> _kinlyApiClientMock;
        private Mock<ILogger<ConsultationService>> _loggerMock;
        private Conference _testConference;

        [SetUp]
        public void Setup()
        {
            _kinlyApiClientMock = new Mock<IKinlyApiClient>();
            _loggerMock = new Mock<ILogger<ConsultationService>>();

            _consultationService = new ConsultationService(_kinlyApiClientMock.Object, _loggerMock.Object);

            _testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant", "rep1@dA.com")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .WithEndpoint("Endpoint With DA", $"{Guid.NewGuid():N}@test.hearings.com", "rep1@dA.com")
                .WithEndpoint("Endpoint Without DA", $"{Guid.NewGuid():N}@test.hearings.com")
                .Build();
        }

        [Test]
        public async Task should_remove_all_participants_in_room()
        {
            var participantId = _testConference.Participants[0].Id;
            var _consultationRoom = VirtualCourtRoomType.JudgeJOH;
            await _consultationService.LeaveConsultationAsync(_testConference.Id, participantId, _consultationRoom);

            _kinlyApiClientMock.Verify(x =>
                    x.TransferParticipantAsync(_testConference.Id.ToString(),
                        It.Is<TransferParticipantParams>(r =>
                            r.From == VirtualCourtRoomType.JudgeJOH.ToString() &&
                            r.To == VirtualCourtRoomType.WaitingRoom.ToString()
                        )
                    )
                , Times.Exactly(1));
        }
    }
}
