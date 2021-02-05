using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetHeartbeatsFromTimePointQuery : IQuery
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }
        public TimeSpan FromTimeSpan { get; }

        public GetHeartbeatsFromTimePointQuery(Guid conferenceId, Guid participantId, TimeSpan fromTimeSpan)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            FromTimeSpan = fromTimeSpan;
        }
    }
    
    public class GetHeartbeatsFromTimePointQueryHandler : IQueryHandler<GetHeartbeatsFromTimePointQuery, IList<Heartbeat>>
    {
        private readonly VideoApiDbContext _context;

        public GetHeartbeatsFromTimePointQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task<IList<Heartbeat>> Handle(GetHeartbeatsFromTimePointQuery query)
        {
            var fromDateTime = DateTime.UtcNow - query.FromTimeSpan;
            
            return await _context.Heartbeats
                .AsNoTracking()
                .Where
                (
                    x => x.ConferenceId == query.ConferenceId && 
                    x.ParticipantId == query.ParticipantId &&
                    x.Timestamp >= fromDateTime
                )
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
        }
    }
}
