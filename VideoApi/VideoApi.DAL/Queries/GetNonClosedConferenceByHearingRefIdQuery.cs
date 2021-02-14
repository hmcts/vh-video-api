using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetNonClosedConferenceByHearingRefIdQuery : IQuery
    {
        public Guid HearingRefId { get; }
        public bool IncludeClosedConferences { get; }

        public GetNonClosedConferenceByHearingRefIdQuery(Guid hearingRefId, bool includeClosedConferences = false)
        {
            HearingRefId = hearingRefId;
            IncludeClosedConferences = includeClosedConferences;
        }
    }

    public class GetNonClosedConferenceByHearingRefIdQueryHandler :
        IQueryHandler<GetNonClosedConferenceByHearingRefIdQuery, Conference>
    {
        private readonly VideoApiDbContext _context;

        public GetNonClosedConferenceByHearingRefIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<Conference> Handle(GetNonClosedConferenceByHearingRefIdQuery query)
        {
            var efQuery = _context.Conferences
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints)
                .AsNoTracking();
            
            if (!query.IncludeClosedConferences)
            {
                efQuery = efQuery.Where(x => x.State != ConferenceState.Closed);
            }

            return await efQuery.SingleOrDefaultAsync(x => x.HearingRefId == query.HearingRefId);
        }
    }
}
