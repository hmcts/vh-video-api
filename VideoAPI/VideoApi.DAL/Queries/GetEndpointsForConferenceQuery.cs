using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetEndpointsForConferenceQuery : IQuery
    {
        public Guid ConferenceId { get; }
        
        public GetEndpointsForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }

    public class GetEndpointsForConferenceQueryHandler : IQueryHandler<GetEndpointsForConferenceQuery, IList<Endpoint>>
    {
        private readonly VideoApiDbContext _context;

        public GetEndpointsForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Endpoint>> Handle(GetEndpointsForConferenceQuery query)
        {
            var conference = await _context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);

            return conference == null ? new List<Endpoint>() : conference.GetEndpoints();
        }
    }
}
