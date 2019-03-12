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

        public async Task<Conference> Handle(GetConferenceByIdQuery query)
        {
            return await _context.Conferences
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses").AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);
        }
    }
}