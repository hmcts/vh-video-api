using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetHeartbeatMaxPercentageLostRecentQuery : IQuery
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }

        public GetHeartbeatMaxPercentageLostRecentQuery(Guid conferenceId, Guid participantId)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
        }
    }
    
    public class GetHeartbeatMaxPercentageLostRecentQueryHandler : IQueryHandler<GetHeartbeatMaxPercentageLostRecentQuery, IList<Heartbeat>>
    {
        private readonly VideoApiDbContext _context;

        public GetHeartbeatMaxPercentageLostRecentQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task<IList<Heartbeat>> Handle(GetHeartbeatMaxPercentageLostRecentQuery query)
        {
            return await _context.Heartbeats
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && x.ParticipantId == query.ParticipantId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
        }
    }
}
