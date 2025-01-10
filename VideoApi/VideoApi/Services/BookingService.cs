using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;

namespace VideoApi.Services;

public interface IBookingService
{
    public Task<bool> BookMeetingRoomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl,
        IEnumerable<EndpointDto> endpoints, ConferenceRoomType roomType,
        AudioPlaybackLanguage audioPlaybackLanguage, Supplier supplier = Supplier.Vodafone);
    
    public Task<Guid> CreateConferenceAsync(BookNewConferenceRequest request, string ingestUrl);
}

public class BookingService(
    ISupplierPlatformServiceFactory supplierPlatformServiceFactory,
    ICommandHandler commandHandler,
    IQueryHandler queryHandler,
    ILogger<BookingService> logger)
    : IBookingService
{
    public async Task<bool> BookMeetingRoomAsync(Guid conferenceId,
        bool audioRecordingRequired,
        string ingestUrl,
        IEnumerable<EndpointDto> endpoints,
        ConferenceRoomType roomType,
        AudioPlaybackLanguage audioPlaybackLanguage,
        Supplier supplier = Supplier.Vodafone)
    {
        MeetingRoom meetingRoom;
        var telephoneId = await CreateUniqueTelephoneId();
        var videoPlatformService = supplierPlatformServiceFactory.Create(supplier);
        var endpointDtos = endpoints.ToList();
        try
        {
            meetingRoom = await videoPlatformService.BookVirtualCourtroomAsync(conferenceId,
                audioRecordingRequired,
                ingestUrl,
                endpointDtos,
                telephoneId,
                roomType,
                audioPlaybackLanguage
                );
        }
        catch (DoubleBookingException ex)
        {
            logger.LogError(ex, "Room already booked for conference {conferenceId}", conferenceId);
            
            meetingRoom = await videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
        }
        
        if (meetingRoom == null) return false;
        
        var command = new UpdateMeetingRoomCommand
        (
            conferenceId, meetingRoom.AdminUri, meetingRoom.JudgeUri, meetingRoom.ParticipantUri,
            meetingRoom.PexipNode, meetingRoom.TelephoneConferenceId
        );
        
        await commandHandler.Handle(command);
        
        return true;
    }
    
    public async Task<Guid> CreateConferenceAsync(BookNewConferenceRequest request, string ingestUrl)
    {
        var existingConference = await queryHandler.Handle<CheckConferenceOpenQuery, Conference>(
            new CheckConferenceOpenQuery(request.ScheduledDateTime, request.CaseNumber, request.CaseName,
                request.HearingRefId));
        
        if (existingConference != null) return existingConference.Id;
        
        var participants = request.Participants.Select(x =>
                new Participant(x.ParticipantRefId, x.Name, x.FirstName, x.LastName, x.DisplayName, x.Username,
                    x.UserRole.MapToDomainEnum(), x.HearingRole, x.CaseTypeGroup, x.ContactEmail, x.ContactTelephone)
                {
                    Representee = x.Representee
                })
            .ToList();
        
        var endpoints = request.Endpoints
            .Select(x => new Endpoint(x.DisplayName, x.SipAddress, x.Pin, x.DefenceAdvocate, 
                (ConferenceRole)x.ConferenceRole))
            .ToList();
        
        var linkedParticipants = request.Participants
            .SelectMany(x => x.LinkedParticipants)
            .Select(x => new LinkedParticipantDto
            {
                ParticipantRefId = x.ParticipantRefId,
                LinkedRefId = x.LinkedRefId,
                Type = x.Type.MapToDomainEnum()
            }).ToList();

        var createConferenceCommand = new CreateConferenceCommand
        (
            request.HearingRefId, request.CaseType, request.ScheduledDateTime, request.CaseNumber,
            request.CaseName, request.ScheduledDuration, participants, request.HearingVenueName,
            request.AudioRecordingRequired, ingestUrl, endpoints, linkedParticipants,
            (Supplier)request.Supplier,
            (ConferenceRoomType)request.ConferenceRoomType,
            (AudioPlaybackLanguage)request.AudioPlaybackLanguage
        );
        
        await commandHandler.Handle(createConferenceCommand);
        
        return createConferenceCommand.NewConferenceId;
    }
    
    private async Task<string> CreateUniqueTelephoneId()
    {
        var telephoneId = ConferenceHelper.GenerateGlobalRareNumber();
        var conferences = await queryHandler.Handle<GetConferencesByTelephoneIdQuery, List<Conference>>(
            new GetConferencesByTelephoneIdQuery(telephoneId));
        
        if(conferences.Count != 0)
            return await CreateUniqueTelephoneId();
        
        return telephoneId;
    }
}
