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
    public class GetConferencesByUsernameQuery : IQuery
    {
        public string Username { get; set; }
        public DateTime? Date { get; set; }

        public GetConferencesByUsernameQuery(string username)
        {
            Username = username;
        }
    }

    public class GetConferencesByUsernameQueryHandler : IQueryHandler<GetConferencesByUsernameQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetConferencesByUsernameQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetConferencesByUsernameQuery query)
        {
            query.Username = query.Username.ToLower().Trim();
            var efQuery = _context.Conferences
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses")
                .Include("Tasks").AsNoTracking();

            if (query.Date.HasValue)
            {
                efQuery = efQuery.Where(x =>
                    x.ScheduledDateTime > query.Date.Value.Date &&
                    x.ScheduledDateTime < query.Date.Value.Date.AddDays(1));
            }
            return await efQuery
                .Where(x => x.Participants.Any(p => p.Username == query.Username))
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}