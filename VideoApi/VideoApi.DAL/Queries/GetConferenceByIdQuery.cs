using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferenceByIdQuery : IQuery
    {
        public Guid ConferenceId { get; set; }

        public GetConferenceByIdQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }
    
    public class GetConferenceByIdQueryHandler : IQueryHandler<GetConferenceByIdQuery, Conference>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceByIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task<Conference> Handle(GetConferenceByIdQuery query)
        {
            return _context.Conferences
                .Include(x => x.Participants).ThenInclude(x => x.CurrentVirtualRoom)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints).ThenInclude(x => x.CurrentVirtualRoom)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);
        }
    }
}
