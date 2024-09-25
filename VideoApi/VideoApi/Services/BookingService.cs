using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Extensions;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;

namespace VideoApi.Services;

public interface IBookingService
{
    public Task<bool> BookMeetingRoomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl,
        IEnumerable<EndpointDto> endpoints, Supplier supplier = Supplier.Kinly);
    
    public Task<Guid> CreateConferenceAsync(BookNewConferenceRequest request, string ingestUrl);
}

public class BookingService(
    ISupplierPlatformServiceFactory _supplierPlatformServiceFactory,
    ICommandHandler _commandHandler,
    IQueryHandler _queryHandler,
    ILogger<BookingService> _logger)
    : IBookingService
{
    public async Task<bool> BookMeetingRoomAsync(Guid conferenceId,
        bool audioRecordingRequired,
        string ingestUrl,
        IEnumerable<EndpointDto> endpoints,
        Supplier supplier = Supplier.Kinly)
    {
        MeetingRoom meetingRoom;
        var telephoneId = await CreateUniqueTelephoneId();
        var videoPlatformService = _supplierPlatformServiceFactory.Create((Domain.Enums.Supplier)supplier);
        try
        {
            meetingRoom = await videoPlatformService.BookVirtualCourtroomAsync(conferenceId,
                audioRecordingRequired,
                ingestUrl,
                endpoints,
                telephoneId);
        }
        catch (DoubleBookingException ex)
        {
            _logger.LogError(ex, "Room already booked for conference {conferenceId}", conferenceId);
            
            meetingRoom = await videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
        }
        
        if (meetingRoom == null) return false;
        
        var command = new UpdateMeetingRoomCommand
        (
            conferenceId, meetingRoom.AdminUri, meetingRoom.JudgeUri, meetingRoom.ParticipantUri,
            meetingRoom.PexipNode, meetingRoom.TelephoneConferenceId
        );
        
        await _commandHandler.Handle(command);
        
        return true;
    }
    
    public async Task<Guid> CreateConferenceAsync(BookNewConferenceRequest request, string ingestUrl)
    {
        var existingConference = await _queryHandler.Handle<CheckConferenceOpenQuery, Conference>(
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
            .Select(x => new Endpoint(x.DisplayName, x.SipAddress, x.Pin, x.DefenceAdvocate)).ToList();
        
        var linkedParticipants = request.Participants
            .SelectMany(x => x.LinkedParticipants)
            .Select(x => new LinkedParticipantDto()
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
            (Domain.Enums.Supplier)request.Supplier
        );
        
        await _commandHandler.Handle(createConferenceCommand);
        
        return createConferenceCommand.NewConferenceId;
    }
    
    private async Task<string> CreateUniqueTelephoneId()
    {
        var telephoneId = ConferenceHelper.GenerateGlobalRareNumber();
        var conferences = await _queryHandler.Handle<GetConferencesByTelephoneIdQuery, List<Conference>>(
            new GetConferencesByTelephoneIdQuery(telephoneId));
        
        if(conferences.Any())
            return await CreateUniqueTelephoneId();
        
        return telephoneId;
    }
}
