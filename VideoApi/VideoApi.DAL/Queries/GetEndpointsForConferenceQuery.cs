using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetEndpointsForConferenceQuery(Guid conferenceId) : IQuery
    {
        public Guid ConferenceId { get; } = conferenceId;
    }

    public class GetEndpointsForConferenceQueryHandler(VideoApiDbContext context)
        : IQueryHandler<GetEndpointsForConferenceQuery, IList<Endpoint>>
    {
        public async Task<IList<Endpoint>> Handle(GetEndpointsForConferenceQuery query)
        {
            var conference = await context.Conferences
                .Include(x => x.Endpoints).ThenInclude(e => e.CurrentConsultationRoom)
                .Include(x => x.Endpoints).ThenInclude(e => e.ParticipantsLinked)
                .AsNoTracking().SingleOrDefaultAsync(x => x.Id == query.ConferenceId);

            return conference == null ? new List<Endpoint>() : conference.GetEndpoints();
        }
    }
}
