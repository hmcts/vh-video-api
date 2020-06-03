using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetInstantMessagesForConferenceQuery : IQuery
    {
        public GetInstantMessagesForConferenceQuery(Guid conferenceId, string participantName)
        {
            ConferenceId = conferenceId;
            ParticipantName = participantName;
        }

        public Guid ConferenceId { get; }
        public string ParticipantName { get; set; }
    }

    public class GetInstantMessagesForConferenceQueryHandler : IQueryHandler<GetInstantMessagesForConferenceQuery, List<InstantMessage>>
    {
        private readonly VideoApiDbContext _context;

        public GetInstantMessagesForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<InstantMessage>> Handle(GetInstantMessagesForConferenceQuery query)
        {
            var instantMessages = await _context.InstantMessages
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId)
                .OrderByDescending(x => x.TimeStamp)
                .ToListAsync();

            if(query.ParticipantName != null)
            {
                instantMessages = instantMessages
                    .Where(x => x.From.ToUpper() == query.ParticipantName.ToUpper() ||
                    x.To.ToUpper() == query.ParticipantName.ToUpper()).ToList();
            }

            return instantMessages;
        }
    }
}
