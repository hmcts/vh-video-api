using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetInstantMessagesForConferenceQuery : IQuery
    {
        public GetInstantMessagesForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; }
    }

    public class
        GetInstantMessagesForConferenceQueryHandler : IQueryHandler<GetInstantMessagesForConferenceQuery,
            List<InstantMessage>>
    {
        private readonly VideoApiDbContext _context;

        public GetInstantMessagesForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<InstantMessage>> Handle(GetInstantMessagesForConferenceQuery query)
        {
            var conference = await _context.Conferences.Include(x => x.InstantMessageHistory)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(query.ConferenceId);
            }

            return conference.InstantMessageHistory.OrderByDescending(x => x.TimeStamp).ToList();
        }
    }
}
