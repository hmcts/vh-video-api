using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using VideoApi.Common.Helpers;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Clients.SupplierStub;
using VideoApi.Services.Clients.SupplierStub.Models;

namespace VideoApi.UnitTests.Services.Clients.SupplierStub;

public class SupplierStubApiClientTests
{
    private MockHttpMessageHandler _messageHandlerMock;
    private SupplierStubApiClient _client;
    private const string BaseAddress = "https://supplier-stub-api";
    
    [SetUp]
    public void SetUp()
    {
        _messageHandlerMock = new MockHttpMessageHandler();
        var httpClient = _messageHandlerMock.ToHttpClient();
        _client = new SupplierStubApiClient(httpClient)
        {
            BaseUrlAddress = BaseAddress
        };
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
        
        const string responseBody = """
                                    {
                                      "id": "a536efc7-c7ac-44da-ae3f-88ea53a729cf",
                                      "uris": {
                                        "pexip_node": "pexip_node",
                                        "participant": "participant",
                                        "hearing_room_uri": "hearing_room_uri",
                                        "telephone_conference_id": "telephone_conference_id"
                                      },
                                      "virtual_courtroom_id": "07a71e5e-05ae-4c26-9da3-702030366d31",
                                      "telephone_conference_id": "telephone_conference_id"
                                    }
                                    """;
        
        _messageHandlerMock.When(HttpMethod.Post, $"{BaseAddress}/hearings")
            .Respond("application/json", responseBody);

        // Act
        var response = await _client.CreateHearingAsync(requestBody);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(requestBody.ToSupplierStubHearingModel().ToBookHearingResponse());
    }

    [Test]
    public async Task CreateConsultationRoomAsync_should_return_expected_response_when_creating_room_with_existing_prefix()
    {
        // Arrange
        var requestBody = new ConsultationRoomRequest
        {
            RoomLabelPrefix = "ExistingPrefix"
        };

        var hearingId = Guid.Parse("a536efc7-c7ac-44da-ae3f-88ea53a729cf");
        
        const string getHearingResponseBody = """
                                    {
                                      "id": "a536efc7-c7ac-44da-ae3f-88ea53a729cf",
                                      "uris": {
                                        "pexip_node": "pexip_node",
                                        "participant": "participant",
                                        "hearing_room_uri": "hearing_room_uri",
                                        "telephone_conference_id": "telephone_conference_id"
                                      },
                                      "virtual_courtroom_id": "07a71e5e-05ae-4c26-9da3-702030366d31",
                                      "telephone_conference_id": "telephone_conference_id",
                                      "rooms": [
                                          {
                                            "label": "ExistingPrefix1"
                                          }
                                      ]
                                    }
                                    """;

        SetUpCreateConsultationRoomAsyncEndpoint(hearingId, getHearingResponseBody);
        
        // Act
        var response =  await _client.CreateConsultationRoomAsync(hearingId, requestBody);
        
        // Assert
        response.Should().NotBeNull();
        response.RoomLabel.Should().Be("ExistingPrefix2");
    }
    
    [Test]
    public async Task CreateConsultationRoomAsync_should_return_expected_response_when_creating_room_with_new_prefix()
    {
        // Arrange
        var requestBody = new ConsultationRoomRequest
        {
            RoomLabelPrefix = "NewPrefix"
        };

        var hearingId = Guid.Parse("a536efc7-c7ac-44da-ae3f-88ea53a729cf");
        
        const string getHearingResponseBody = """
                                    {
                                      "id": "a536efc7-c7ac-44da-ae3f-88ea53a729cf",
                                      "uris": {
                                        "pexip_node": "pexip_node",
                                        "participant": "participant",
                                        "hearing_room_uri": "hearing_room_uri",
                                        "telephone_conference_id": "telephone_conference_id"
                                      },
                                      "virtual_courtroom_id": "07a71e5e-05ae-4c26-9da3-702030366d31",
                                      "telephone_conference_id": "telephone_conference_id"
                                    }
                                    """;

        SetUpCreateConsultationRoomAsyncEndpoint(hearingId, getHearingResponseBody);
        
        // Act
        var response =  await _client.CreateConsultationRoomAsync(hearingId, requestBody);
        
        // Assert
        response.Should().NotBeNull();
        response.RoomLabel.Should().Be("NewPrefix1");
    }
    
    [Test]
    public async Task GetHearingAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.Parse("a536efc7-c7ac-44da-ae3f-88ea53a729cf");
        
        const string responseBody = """
                                      {
                                        "id": "a536efc7-c7ac-44da-ae3f-88ea53a729cf",
                                        "uris": {
                                          "pexip_node": "pexip_node",
                                          "participant": "participant",
                                          "hearing_room_uri": "hearing_room_uri",
                                          "telephone_conference_id": "telephone_conference_id"
                                        },
                                        "virtual_courtroom_id": "07a71e5e-05ae-4c26-9da3-702030366d31",
                                        "telephone_conference_id": "telephone_conference_id"
                                      }
                                      """;
        
        _messageHandlerMock.When(HttpMethod.Get, $"{BaseAddress}/hearings/{hearingId}")
            .Respond("application/json", responseBody);

        var deserialisedResponseBody = ApiRequestHelper.DeserialiseForSupplier<Hearing>(responseBody);
        
        // Act
        var response = await _client.GetHearingAsync(hearingId);
        
        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(deserialisedResponseBody.ToRetrieveHearingResponse());
    }
    
    [Test]
    public void DeleteHearingAsync_should_return_expected_response()
    {
        // Arrange
        var hearingId = Guid.NewGuid();
        
        _messageHandlerMock.When(HttpMethod.Delete, $"{BaseAddress}/hearings/{hearingId}")
            .Respond(HttpStatusCode.NoContent);

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
    public async Task GetHealth_should_return_expected_response_when_platform_is_healthy()
    {
        // Arrange
        const string responseBody = """
                                    [
                                        {
                                          "id": "a536efc7-c7ac-44da-ae3f-88ea53a729cf",
                                          "uris": {
                                            "pexip_node": "pexip_node",
                                            "participant": "participant",
                                            "hearing_room_uri": "hearing_room_uri",
                                            "telephone_conference_id": "telephone_conference_id"
                                          },
                                          "virtual_courtroom_id": "07a71e5e-05ae-4c26-9da3-702030366d31",
                                          "telephone_conference_id": "telephone_conference_id"
                                        }
                                    ]
                                    """;
        
        _messageHandlerMock.When(HttpMethod.Get, $"{BaseAddress}/hearings")
            .Respond("application/json", responseBody);
        
        // Act
        var response = await _client.GetHealth();

        // Assert
        response.Should().NotBeNull();
        response.HealthStatus.Should().Be(PlatformHealth.Healthy);
    }
    
    [Test]
    public async Task GetHealth_should_return_expected_response_when_platform_is_unhealthy()
    {
        // Arrange
        _messageHandlerMock.When(HttpMethod.Get, $"{BaseAddress}/hearings")
            .Respond(HttpStatusCode.InternalServerError);
        
        // Act
        var response = await _client.GetHealth();

        // Assert
        response.Should().NotBeNull();
        response.HealthStatus.Should().Be(PlatformHealth.Unhealthy);
    }

    private void SetUpCreateConsultationRoomAsyncEndpoint(Guid hearingId, string getHearingResponseBody)
    {
        _messageHandlerMock.When(HttpMethod.Get, $"{BaseAddress}/hearings/{hearingId}")
            .Respond("application/json", getHearingResponseBody);

        _messageHandlerMock.When(HttpMethod.Patch, $"{BaseAddress}/hearings/{hearingId}")
            .Respond(HttpStatusCode.NoContent);
    }
}
