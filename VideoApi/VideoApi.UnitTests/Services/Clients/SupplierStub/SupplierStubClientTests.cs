using System;
using System.Threading.Tasks;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Clients.SupplierStub;

namespace VideoApi.UnitTests.Services.Clients.SupplierStub;

public class SupplierStubClientTests
{
    private SupplierStubClient _client;
    
    [SetUp]
    public void SetUp()
    {
        _client = new SupplierStubClient();
    }
    
    [Test]
    public async Task CreateHearingAsync_should_return_expected_response()
    {
        // Arrange
        var requestBody = new BookHearingRequest
        {
            VirtualCourtroomId = Guid.Parse("07a71e5e-05ae-4c26-9da3-702030366d31"),
            TelephoneConferenceId = "telephone_conference_id"
        };

        // Act
        var response = await _client.CreateHearingAsync(requestBody);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(new BookHearingResponse
        {
            Uris = new MeetingUris
            {
                PexipNode = "pexip_node",
                Participant = "participant",
                HearingRoomUri = "hearing_room_uri",
                TelephoneConferenceId = requestBody.TelephoneConferenceId
            },
            VirtualCourtroomId = requestBody.VirtualCourtroomId
        });
    }
    
    [Test]
    public async Task CreateConsultationRoomAsync_should_return_expected_response()
    {
        // Arrange
        var requestBody = new ConsultationRoomRequest
        {
            RoomLabelPrefix = "NewPrefix"
        };

        var hearingId = Guid.Parse("a536efc7-c7ac-44da-ae3f-88ea53a729cf");
        
        // Act
        var response =  await _client.CreateConsultationRoomAsync(hearingId, requestBody);
        
        // Assert
        response.Should().NotBeNull();
        response.RoomLabel.Should().StartWith(requestBody.RoomLabelPrefix);
    }
    
    [Test]
    public async Task GetHearingAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.Parse("a536efc7-c7ac-44da-ae3f-88ea53a729cf");
        
        // Act
        var response = await _client.GetHearingAsync(hearingId);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(new RetrieveHearingResponse
        {
            Uris = new MeetingUris
            {
                PexipNode = "pexip_node",
                Participant = "participant",
                HearingRoomUri = "hearing_room_uri",
                TelephoneConferenceId = hearingId.ToString()
            },
            VirtualCourtroomId = hearingId,
            TelephoneConferenceId = hearingId.ToString()
        });
    }
    
    [Test]
    public void DeleteHearingAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _client.DeleteHearingAsync(hearingId));
    }
    
    [Test]
    public void UpdateHearingAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();
        var requestBody = new UpdateHearingRequest
        {
            RecordingEnabled = false,
            StreamingEnabled = false
        };
        
        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _client.UpdateHearingAsync(hearingId, requestBody));
    }
    
    [Test]
    public async Task TransferParticipantAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();
        var requestBody = new TransferRequest
        {
            PartId = Guid.NewGuid().ToString(),
            Role = "Judge",
            From = "WaitingRoom",
            To = "ParticipantRoom1"
        };

        // Act
        var response = await _client.TransferParticipantAsync(hearingId, requestBody);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().Be(requestBody.To);
    }
    
    [Test]
    public async Task StartAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();
        var requestBody = new StartHearingRequest
        {
            HearingLayout = Layout.Automatic,
            MuteGuests = false
        };

        // Act
        var response = await _client.StartAsync(hearingId, requestBody);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().Be(hearingId.ToString());
    }
    
    [Test]
    public async Task PauseHearingAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();

        // Act
        var response = await _client.PauseHearingAsync(hearingId);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().Be(hearingId.ToString());
    }
    
    [Test]
    public async Task UpdateParticipantDisplayNameAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();
        var requestBody = new DisplayNameRequest
        {
            ParticipantId = Guid.NewGuid().ToString(),
            ParticipantName = "Judge"
        };

        // Act
        var response = await _client.UpdateParticipantDisplayNameAsync(hearingId, requestBody);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().Be(hearingId.ToString());
    }
    
    [Test]
    public async Task EndHearingAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();

        // Act
        var response = await _client.EndHearingAsync(hearingId);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(hearingId.ToString());
    }
    
    [Test]
    public async Task TechnicalAssistanceAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();

        // Act
        var response = await _client.TechnicalAssistanceAsync(hearingId);

        // Assert
        response.Should().NotBeNull();
        response.Should().Be(hearingId.ToString());
    }
    
    [Test]
    public async Task RetrieveParticipantSelfTestScore_should_return_expected_response()
    {
        // Arrange
        var participantId = Guid.NewGuid();

        // Act
        var response = await _client.RetrieveParticipantSelfTestScore(participantId);

        // Assert
        response.Should().NotBeNull();
        response.Passed.Should().BeTrue();
        response.Score.Should().Be(1);
        response.UserId.Should().Be(participantId);
    }
    
    [Test]
    public async Task GetHealth_should_return_expected_response()
    {
        // Arrange
        // Arrange & Act
        var response = await _client.GetHealth();

        // Assert
        response.Should().NotBeNull();
        response.HealthStatus.Should().Be(PlatformHealth.Healthy);
    }
}
