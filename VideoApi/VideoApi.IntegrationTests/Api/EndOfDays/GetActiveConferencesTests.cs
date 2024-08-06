using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Client;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Api.Setup;
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
        var conferenceInSession = new ConferenceBuilder(ignoreId: true, supplier: Supplier.Kinly).WithParticipants(2)
            .WithConferenceStatus(ConferenceState.InSession).Build();
        var kinlyConference = conferenceInSession;
        var conferencePaused = new ConferenceBuilder(ignoreId: true, supplier: Supplier.Vodafone).WithParticipants(2)
            .WithConferenceStatus(ConferenceState.Paused).Build();
        var vodafoneConference = conferencePaused;
        
        var conferenceClosed = new ConferenceBuilder(ignoreId: true).WithParticipants(2)
            .WithConferenceStatus(ConferenceState.Closed).Build();
        
        var conferenceNotStartedWithActiveConsultation = new ConferenceBuilder(ignoreId: true)
            .WithConferenceStatus(ConferenceState.NotStarted).WithParticipants(2).Build();
        var consultationRoomPreStart = new ConsultationRoom(conferenceNotStartedWithActiveConsultation.Id,
            "Civilian_ConsultationRoom1", VirtualCourtRoomType.Civilian, false);
        
        conferenceNotStartedWithActiveConsultation.Participants[0].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceNotStartedWithActiveConsultation.Participants[0].UpdateCurrentConsultationRoom(consultationRoomPreStart);
        conferenceNotStartedWithActiveConsultation.Participants[1].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceNotStartedWithActiveConsultation.Participants[1].UpdateCurrentConsultationRoom(consultationRoomPreStart);
        
        var conferenceClosedWithActiveConsultation = new ConferenceBuilder(ignoreId: true)
            .WithConferenceStatus(ConferenceState.Closed).WithParticipants(2).Build();
        var consultationRoomPostClosed = new ConsultationRoom(conferenceClosedWithActiveConsultation.Id,
            "Civilian_ConsultationRoom1", VirtualCourtRoomType.Civilian, false);
        
        conferenceClosedWithActiveConsultation.Participants[0].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceClosedWithActiveConsultation.Participants[0].UpdateCurrentConsultationRoom(consultationRoomPostClosed);
        conferenceClosedWithActiveConsultation.Participants[1].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceClosedWithActiveConsultation.Participants[1].UpdateCurrentConsultationRoom(consultationRoomPostClosed);
        
        
        var conferenceSuspendedWithActiveConsultation = new ConferenceBuilder(ignoreId: true)
            .WithConferenceStatus(ConferenceState.Suspended).WithParticipants(2).Build();
        var consultationSuspendedClosed = new ConsultationRoom(conferenceSuspendedWithActiveConsultation.Id,
            "Civilian_ConsultationRoom1", VirtualCourtRoomType.Civilian, false);
        
        conferenceSuspendedWithActiveConsultation.Participants[0].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceSuspendedWithActiveConsultation.Participants[0].UpdateCurrentConsultationRoom(consultationSuspendedClosed);
        conferenceSuspendedWithActiveConsultation.Participants[1].UpdateParticipantStatus(ParticipantState.InConsultation);
        conferenceSuspendedWithActiveConsultation.Participants[1].UpdateCurrentConsultationRoom(consultationSuspendedClosed);
        
        await TestDataManager.SeedConference(conferenceInSession);
        await TestDataManager.SeedConference(conferencePaused);
        await TestDataManager.SeedConference(conferenceClosed);
        await TestDataManager.SeedConference(conferenceClosedWithActiveConsultation);
        await TestDataManager.SeedConference(conferenceNotStartedWithActiveConsultation);
        await TestDataManager.SeedConference(conferenceSuspendedWithActiveConsultation);
        
        
        using var client = Application.CreateClient();
        
        // Act
        var apiClient = VideoApiClient.GetClient(client);
        var conferenceResponse = await apiClient.GetActiveConferencesAsync();
        
        
        // Assert
        conferenceResponse.Should().NotBeNullOrEmpty();
        conferenceResponse.Should().NotContain(x=> x.Id == conferenceClosed.Id);
        
        conferenceResponse.Select(c => c.Id).Should().Contain(new List<Guid>
        {
            conferenceInSession.Id,
            conferencePaused.Id,
            conferenceClosedWithActiveConsultation.Id,
            conferenceNotStartedWithActiveConsultation.Id,
            conferenceSuspendedWithActiveConsultation.Id
        });
        
        var kinlyConferenceResponse = conferenceResponse.FirstOrDefault(x => x.Id == kinlyConference.Id);
        VerifyConferenceInResponse(kinlyConferenceResponse, kinlyConference);
        
        var vodafoneConferenceResponse = conferenceResponse.FirstOrDefault(x => x.Id == vodafoneConference.Id);
        VerifyConferenceInResponse(vodafoneConferenceResponse, vodafoneConference);
    }
}
