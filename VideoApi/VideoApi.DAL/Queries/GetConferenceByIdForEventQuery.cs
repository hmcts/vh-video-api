using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferenceByIdForEventQuery : IQuery
    {
        public Guid ConferenceId { get; set; }

        public GetConferenceByIdForEventQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }

    public class GetConferenceByIdForEventQueryHandler : IQueryHandler<GetConferenceByIdForEventQuery, Conference>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceByIdForEventQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task<Conference> Handle(GetConferenceByIdForEventQuery query)
        {
            return _context.Conferences
                .Include(x => x.Participants).ThenInclude(x => x.CurrentConsultationRoom)
                .Include(x => x.Endpoints)
                .Include(x => x.Rooms)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);
        }
    }
}
