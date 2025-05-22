using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using VideoApi.Common.Helpers;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.Models;

namespace VideoApi.UnitTests.Services.Clients;

public class SupplierClientTests
{
    [Test]
    public async Task should_map_conference_details_on_booking()
    {
        // arrange
        var conferenceId = Guid.Parse("b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d");
        var baseAddress = "http://supplier-api";
        
        var responseBody = """
                           {
                             "uris": {
                               "pexip_node": "test.unit.vh.co.uk",
                               "participant": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM",
                               "hearing_room_uri": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"
                             },
                             "virtual_courtroom_id": "b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d"
                           }
                           """;
        
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Post, $"{baseAddress}/hearing")
            .Respond(HttpStatusCode.OK, "application/json", responseBody);
        var httpClient = mockHttp.ToHttpClient();
        var supplierApiClient = new SupplierClient(httpClient)
        {
            BaseUrlAddress = baseAddress
        };

        // act
        var result = await supplierApiClient.CreateHearingAsync(new BookHearingRequest()
        {
            VirtualCourtroomId = conferenceId,
            CallbackUri = "https://vh-video-web.dev.platform.hmcts.net/callback",
            RecordingEnabled = false,
            RecordingUrl =
                "rtmps://vh-recorder:443/vh-recording-app/ABA4-AutomationTestHearingbac29d98326e4d118783bbdf9f1c24f6-77451c22-85cf-4b0f-90f7-146d09472843",
            StreamingEnabled = false,
            TelephoneConferenceId = "75926535",
            RoomType = "VMR",
            AudioPlaybackLanguage = "English",
            JvsEndpoint = []
        });
        
        Assert.That(result.VirtualCourtroomId, Is.EqualTo(conferenceId));
        Assert.That(result.Uris.PexipNode, Is.EqualTo("test.unit.vh.co.uk"));
        Assert.That(result.Uris.Participant, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM"));
        Assert.That(result.Uris.HearingRoomUri, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"));
    }

    [Test]
    public async Task should_map_conference_on_retrieve_hearing_details()
    {
        // arrange
        var conferenceId = Guid.Parse("b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d");
        var baseAddress = "http://supplier-api";
        
        var responseBody = """
                           {
                             "uris": {
                               "pexip_node": "test.unit.vh.co.uk",
                               "participant": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM",
                               "hearing_room_uri": "hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"
                             },
                             "virtual_courtroom_id": "b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d",
                             "consultation_rooms":[]
                           }
                           """;
        
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, $"{baseAddress}/hearing/{conferenceId}")
            .Respond(HttpStatusCode.OK, "application/json", responseBody);
        var httpClient = mockHttp.ToHttpClient();
        var supplierApiClient = new SupplierClient(httpClient)
        {
            BaseUrlAddress = baseAddress
        };

        // act
        var result = await supplierApiClient.GetHearingAsync(conferenceId);
        
        Assert.That(result.VirtualCourtroomId, Is.EqualTo(conferenceId));
        Assert.That(result.Uris.PexipNode, Is.EqualTo("test.unit.vh.co.uk"));
        Assert.That(result.Uris.Participant, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_WAITING_ROOM"));
        Assert.That(result.Uris.HearingRoomUri, Is.EqualTo("hmcts-unittesting-bd5e5ec9-b90c-4f64-b74d-5cfc2664fbdf_b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d_HEARING_ROOM"));
    }
    
    [TestCase(Layout.Automatic, "AUTOMATIC")]
    [TestCase(Layout.Single, "SINGLE")]
    [TestCase(Layout.FourEqual, "FOUR_EQUAL")]
    [TestCase(Layout.OnePlusSeven, "ONE_PLUS_SEVEN")]
    [TestCase(Layout.TwoPlusTwentyone, "TWO_PLUS_TWENTYONE")]
    [TestCase(Layout.NineEqual, "NINE_EQUAL")]
    [TestCase(Layout.SixteenEqual, "SIXTEEN_EQUAL")]
    [TestCase(Layout.TwentyFiveEqual, "TWENTY_FIVE_EQUAL")]
    public async Task should_serialise_start_request_as_expected(Layout layout, string expectedString)
    {
        // Arrange
        var conferenceId = Guid.Parse("b1c3d8f9-67cb-4f0a-8fd3-fa8f81b6893d");
        var baseAddress = "http://supplier-api";
        
        var hostId = "643fade8-bf6c-4cad-bd6f-a5484b097c34";
        var participantId = "3064a986-12ea-4385-9c70-8dd1ec46d61d";
        var startHearingRequest = new StartHearingRequest
        {
            HearingLayout = layout,
            MuteGuests = true,
            TriggeredByHostId = "host123",
            HostsForScreening = null,
            Hosts = [hostId],
            ForceTransferParticipantIds = [participantId]
        };

        var expectedRequestBody = $$"""
                                  {
                                    "hosts": [
                                      "643fade8-bf6c-4cad-bd6f-a5484b097c34"
                                    ],
                                    "hearing_layout": "{{expectedString}}",
                                    "mute_guests": true,
                                    "force_transfer_participant_ids": [
                                      "3064a986-12ea-4385-9c70-8dd1ec46d61d"
                                    ],
                                    "triggered_by_host_id": "host123"
                                  }
                                  """;

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Post, $"{baseAddress}/hearing/{conferenceId}/start")
            .WithContent(expectedRequestBody)
            .Respond(HttpStatusCode.OK, "application/json", "response");

        var httpClient = mockHttp.ToHttpClient();
        var supplierApiClient = new SupplierClient(httpClient)
        {
            BaseUrlAddress = baseAddress
        };

        // Act
        var result = await supplierApiClient.StartAsync(conferenceId, startHearingRequest);

        // Assert
        Assert.That(result, Is.EqualTo("response"));
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
