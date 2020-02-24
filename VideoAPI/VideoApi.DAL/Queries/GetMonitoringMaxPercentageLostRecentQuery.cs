using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetMonitoringMaxPercentageLostRecentQuery : IQuery
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }

        public GetMonitoringMaxPercentageLostRecentQuery(Guid conferenceId, Guid participantId)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
        }
    }
    
    public class GetMonitoringMaxPercentageLostRecentQueryHandler : IQueryHandler<GetMonitoringMaxPercentageLostRecentQuery, IList<Monitoring>>
    {
        private readonly VideoApiDbContext _context;

        public GetMonitoringMaxPercentageLostRecentQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task<IList<Monitoring>> Handle(GetMonitoringMaxPercentageLostRecentQuery query)
        {
            return await _context.Monitorings
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId && x.ParticipantId == query.ParticipantId)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
        }
    }
}
