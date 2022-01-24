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
            var results = from conf in _context.Conferences
                          join even in _context.Events on conf.Id equals even.ConferenceId
                          join room in _context.Rooms on conf.Id equals room.ConferenceId
                          join participant in _context.Participants on conf.Id equals participant.ConferenceId
                          where participant.Id == even.ParticipantId
                          && conf.AudioRecordingRequired
                          && participant.HearingRole == "Interpreter"
                          && even.EventType == Domain.Enums.EventType.RoomParticipantTransfer
                          && even.TransferredTo == Domain.Enums.RoomType.HearingRoom
                          && even.ExternalTimestamp.Date == query.DateStamp.Date
                          && room.Label.Contains(nameof(KinlyRoomType.Interpreter))
                          select new HearingAudioRoom{ 
                              HearingRefId = conf.HearingRefId, 
                              Label = room.Label + room.Id.ToString(), 
                              FileNamePrefix = "_interpreter_" + room.Id.ToString() + "_" + query.DateStamp.Date.ToString("yyyy-MM-dd") 
                          };

            return results.ToListAsync();
        }
    }
}
