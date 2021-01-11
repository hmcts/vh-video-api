using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Extensions;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("events")]
    [ApiController]
    public class VideoEventsController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly ILogger<VideoEventsController> _logger;

        public VideoEventsController(ICommandHandler commandHandler, IEventHandlerFactory eventHandlerFactory,
            ILogger<VideoEventsController> logger)
        {
            _commandHandler = commandHandler;
            _eventHandlerFactory = eventHandlerFactory;
            _logger = logger;
        }

        /// <summary>
        /// Accepts video conference events and publishes to internal clients/services
        /// </summary>
        /// <param name="request">Details of the event</param>
        /// <returns>NoContent if event is handled as expected</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "RaiseVideoEvent")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostEventAsync(ConferenceEventRequest request)
        {
            Guid.TryParse(request.ConferenceId, out var conferenceId);
            Guid.TryParse(request.ParticipantId, out var participantId);

            var command = MapEventRequestToEventCommand(conferenceId, request);

            _logger.LogWarning("Handling {ConferenceEventRequest}", nameof(ConferenceEventRequest));
            
            await _commandHandler.Handle(command);

            if (request.ShouldSkipEventHandler())
            {
                _logger.LogDebug("Handling CallbackEvent skipped due to result of ShouldHandleEvent.");
                return NoContent();
            }

            var callbackEvent = MapEventRequestToEventHandlerDto(conferenceId, participantId, request);
            await _eventHandlerFactory.Get(request.EventType).HandleAsync(callbackEvent);
            return NoContent();
        }

        private SaveEventCommand MapEventRequestToEventCommand(Guid conferenceId, ConferenceEventRequest request)
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

        private CallbackEvent MapEventRequestToEventHandlerDto(Guid conferenceId, Guid participantId,
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
