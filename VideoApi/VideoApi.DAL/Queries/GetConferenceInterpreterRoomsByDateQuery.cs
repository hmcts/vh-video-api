using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services.Kinly;

namespace VideoApi.DAL.Queries
{
    public class GetConferenceInterpreterRoomsByDateQuery : IQuery
    {
        public DateTime DateStamp  { get; set; }

        public GetConferenceInterpreterRoomsByDateQuery(DateTime dateStamp)
        {
            DateStamp = dateStamp;
        }
    }
    
    public class GetConferenceInterpreterRoomsByDateQueryHandler : IQueryHandler<GetConferenceInterpreterRoomsByDateQuery, List<HearingAudioRoom>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceInterpreterRoomsByDateQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public Task<List<HearingAudioRoom>> Handle(GetConferenceInterpreterRoomsByDateQuery query)
        {
            var results = from conference in _context.Conferences
                          join @event in _context.Events on conference.Id equals @event.ConferenceId
                          join room in _context.Rooms on conference.Id equals room.ConferenceId
                          join participant in _context.Participants on conference.Id equals participant.ConferenceId
                          where participant.Id == @event.ParticipantId
                          && conference.AudioRecordingRequired
                          && participant.HearingRole == "Interpreter"
                          && @event.EventType == Domain.Enums.EventType.RoomParticipantTransfer
                          && @event.TransferredTo == Domain.Enums.RoomType.HearingRoom
                          && @event.ExternalTimestamp.Date == query.DateStamp.Date
                          && room.Label.Contains(nameof(KinlyRoomType.Interpreter))
                          select new HearingAudioRoom{ 
                              HearingRefId = conference.HearingRefId, 
                              Label = room.Label + room.Id.ToString(), 
                              FileNamePrefix = "_interpreter_" + room.Id.ToString() + "_" + query.DateStamp.Date.ToString("yyyy-MM-dd") 
                          };

            return results.ToListAsync();
        }
    }
}
