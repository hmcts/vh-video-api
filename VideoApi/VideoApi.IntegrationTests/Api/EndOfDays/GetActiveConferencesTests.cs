using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.EndOfDays;

public class GetActiveConferencesTests : ApiTest
{
    [TearDown]
    public async Task TearDown()
    {
        await TestDataManager.CleanUpSeededData();
    }
    
    [Test]
    public async Task should_return_active_conferences()
    {
        // Arrange
        var conferenceInSession = new ConferenceBuilder(ignoreId: true).WithParticipants(2)
            .WithConferenceStatus(ConferenceState.InSession).Build();
        var conferencePaused = new ConferenceBuilder(ignoreId: true).WithParticipants(2)
            .WithConferenceStatus(ConferenceState.Paused).Build();
        
        var conferenceClosed = new ConferenceBuilder(ignoreId: true).WithParticipants(2)
            .WithConferenceStatus(ConferenceState.Closed).Build();
        
        var conferenceClosedWithActiveConsultation = new ConferenceBuilder(ignoreId: true)
            .WithConferenceStatus(ConferenceState.Closed).WithParticipants(2).Build();
        var consultationRoom = new ConsultationRoom(conferenceClosedWithActiveConsultation.Id,
            "Civilian_ConsultationRoom1", VirtualCourtRoomType.Civilian, false);
        
        conferenceClosedWithActiveConsultation.Participants[0].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceClosedWithActiveConsultation.Participants[0].UpdateCurrentConsultationRoom(consultationRoom);
        conferenceClosedWithActiveConsultation.Participants[1].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceClosedWithActiveConsultation.Participants[1].UpdateCurrentConsultationRoom(consultationRoom);
        
        await TestDataManager.SeedConference(conferenceInSession);
        await TestDataManager.SeedConference(conferencePaused);
        await TestDataManager.SeedConference(conferenceClosed);
        await TestDataManager.SeedConference(conferenceClosedWithActiveConsultation);
        
        
        using var client = Application.CreateClient();
        
        // Act
        var result = await client.GetAsync(ApiUriFactory.EndOfDayEndpoints.GetActiveConferences);
        
        
        // Assert
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        var conferenceResponse = await ApiClientResponse.GetResponses<List<ConferenceForAdminResponse>>(result.Content);
        conferenceResponse.Should().NotBeNullOrEmpty();
        conferenceResponse.Should().NotContain(x=> x.Id == conferenceClosed.Id);
        
        conferenceResponse.Select(c => c.Id).Should().Contain(new List<Guid>
        {
            conferenceInSession.Id,
            conferencePaused.Id,
            conferenceClosedWithActiveConsultation.Id
        });
    }
}
