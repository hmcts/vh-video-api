using System;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Models;

namespace VideoApi.Mappings
{
    public static class EventRequestMapper
    {
        public static SaveEventCommand MapEventRequestToEventCommand(Guid conferenceId, ConferenceEventRequest request)
        {
            GetRoomTypeEnums(request, out var transferTo, out var transferFrom);

            var command = new SaveEventCommand(conferenceId, request.EventId, request.EventType,
                request.TimeStampUtc, transferFrom, transferTo, request.Reason, request.Phone);
            if (Guid.TryParse(request.ParticipantId, out var participantId))
            {
                command.ParticipantId = participantId;
            }

            command.TransferredFromRoomLabel = request.TransferFrom;
            command.TransferredToRoomLabel = request.TransferTo;

            return command;
        }

        public static CallbackEvent MapEventRequestToEventHandlerDto(Guid conferenceId, Guid participantId,
            ConferenceEventRequest request)
        {
            GetRoomTypeEnums(request, out var transferTo, out var transferFrom);

            return new CallbackEvent
            {
                EventId = request.EventId,
                EventType = request.EventType,
                ConferenceId = conferenceId,
                Reason = request.Reason,
                TransferTo = transferTo,
                TransferFrom = transferFrom,
                TimeStampUtc = request.TimeStampUtc,
                ParticipantId = participantId,
                Phone = request.Phone,
                TransferredFromRoomLabel = request.TransferFrom,
                TransferredToRoomLabel = request.TransferTo
            };
        }

        private static void GetRoomTypeEnums(ConferenceEventRequest request, out RoomType? transferTo,
            out RoomType? transferFrom)
        {
            var isTransferFromEnum = Enum.TryParse(request.TransferFrom, out RoomType transferFromEnum);
            var isTransferToEnum = Enum.TryParse(request.TransferTo, out RoomType transferToEnum);

            transferFrom = isTransferFromEnum ? transferFromEnum : (RoomType?) null;
            transferTo = isTransferToEnum ? transferToEnum : (RoomType?) null;
        }
    }
}
