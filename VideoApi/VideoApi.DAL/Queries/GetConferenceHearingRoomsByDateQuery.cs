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
    
    public class GetConferenceHearingRoomsByDateQueryHandler : IQueryHandler<GetConferenceHearingRoomsByDateQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceHearingRoomsByDateQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public Task<List<Conference>> Handle(GetConferenceHearingRoomsByDateQuery query)
        {
            return _context.Conferences
                .Include("ConferenceStatuses")
                .Where(x=>x.ConferenceStatuses.Any(d=>d.TimeStamp.Date == query.DateStamp.Date))
                .Where(x=>x.ConferenceStatuses.Any(s=>s.ConferenceState == ConferenceState.InSession))
                .AsNoTracking().ToListAsync();
        }
    }
}
