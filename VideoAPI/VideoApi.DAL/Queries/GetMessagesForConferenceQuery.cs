using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using Task = VideoApi.Domain.Task;

namespace VideoApi.DAL.Queries
{
    public class GetMessagesForConferenceQuery : IQuery
    {
        public GetMessagesForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; }
    }

    public class GetMessagesForConferenceQueryHandler : IQueryHandler<GetMessagesForConferenceQuery, List<Message>>
    {
        private readonly VideoApiDbContext _context;

        public GetMessagesForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> Handle(GetMessagesForConferenceQuery query)
        {
            var conference = await _context.Conferences.Include(x => x.Messages)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(query.ConferenceId);
            }

            return conference.Messages.OrderByDescending(x => x.TimeStamp).ToList();
        }
    }
}