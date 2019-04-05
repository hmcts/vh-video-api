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
            return await _context.Conferences
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses").AsNoTracking()
                .Where(x => x.Participants.Any(p => p.Username == query.Username) 
                            && x.State != ConferenceState.Closed)
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}