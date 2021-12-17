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
    
    public class GetConferenceInterpreterRoomsByDateQueryHandler : IQueryHandler<GetConferenceInterpreterRoomsByDateQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceInterpreterRoomsByDateQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public Task<List<Conference>> Handle(GetConferenceInterpreterRoomsByDateQuery query)
        {
            return _context.Conferences
                .Include("Rooms")
                .Where(x=>x.ActualStartTime.Value.Date == query.DateStamp.Date)
                .Where(x=>x.Rooms.Any(s=>s.Label.Contains(nameof(KinlyRoomType.Interpreter))))
                .AsNoTracking().ToListAsync();
        }
    }
}
