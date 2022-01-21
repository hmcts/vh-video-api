using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetConferenceHearingRoomsByDateQuery : IQuery
    {
        public DateTime DateStamp  { get; set; }

        public GetConferenceHearingRoomsByDateQuery(DateTime dateStamp)
        {
            DateStamp = dateStamp;
        }
    }
    
    public class GetConferenceHearingRoomsByDateQueryHandler : IQueryHandler<GetConferenceHearingRoomsByDateQuery, List<HearingAudioRoom>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceHearingRoomsByDateQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public Task<List<HearingAudioRoom>> Handle(GetConferenceHearingRoomsByDateQuery query)
        {

            var results = from confStatuses in _context.ConferenceStatuses
                          join conf in _context.Conferences on confStatuses.ConferenceId equals conf.Id
                          where conf.AudioRecordingRequired
                          && confStatuses.ConferenceState == ConferenceState.InSession
                          && confStatuses.TimeStamp.Date == query.DateStamp.Date
                          select new HearingAudioRoom
                          {
                              HearingRefId = conf.HearingRefId,
                              Label = string.Empty,
                              FileNamePrefix = "_" + query.DateStamp.Date.ToString("yyyy-MM-dd")
                          };

            return results.ToListAsync();
        }
    }
}
