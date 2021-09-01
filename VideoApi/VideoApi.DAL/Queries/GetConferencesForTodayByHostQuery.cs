using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesForTodayByHostQuery : IQuery
    {
        public string Username { get; set; }

        public GetConferencesForTodayByHostQuery(string username)
        {
            Username = username;
        }
    }

    public class GetConferencesForTodayByHostQueryHandler : IQueryHandler<GetConferencesForTodayByHostQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferencesForTodayByHostQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetConferencesForTodayByHostQuery query)
        {
            query.Username = query.Username.ToLower().Trim();
            var today = DateTime.Today;
            var tomorrow = DateTime.Today.AddDays(1);

            return await _context.Conferences
                .Include(x => x.Participants)
                .Include(x => x.Endpoints)
                .Include(x => x.MeetingRoom)
                .AsNoTracking()
                .Where(x => x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow)
                .Where(x => x.Participants.Any(p => p.Username == query.Username))
                .Where(x => x.MeetingRoom != null && x.MeetingRoom.AdminUri != null && x.MeetingRoom.JudgeUri != null 
                            && x.MeetingRoom.ParticipantUri != null && x.MeetingRoom.PexipNode != null)
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
                
        }
    }
}
