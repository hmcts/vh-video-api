using System;
using System.Collections.Generic;
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
        public List<Guid> HearingRefIds { get; }
        public bool IncludeClosedConferences { get; }

        public GetNonClosedConferenceByHearingRefIdQuery(IEnumerable<Guid> hearingRefIds, bool includeClosedConferences = false)
        {
            HearingRefIds = hearingRefIds.ToList();
            IncludeClosedConferences = includeClosedConferences;
        }

        public GetNonClosedConferenceByHearingRefIdQuery(Guid hearingRefId, bool includeClosedConferences = false) :
            this(new List<Guid> { hearingRefId }, includeClosedConferences) {}
    }

    public class GetNonClosedConferenceByHearingRefIdQueryHandler :
        IQueryHandler<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;

        public GetNonClosedConferenceByHearingRefIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conference>> Handle(GetNonClosedConferenceByHearingRefIdQuery query)
        {
            var efQuery = _context.Conferences
                .Include(x => x.MeetingRoom)
                .Include(x => x.Participants).ThenInclude(x => x.CurrentConsultationRoom)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints).ThenInclude(x => x.CurrentConsultationRoom)
                .AsNoTracking();
            
            if (!query.IncludeClosedConferences)
            {
                efQuery = efQuery.Where(x => x.State != ConferenceState.Closed);
            }

            return await efQuery.Where(x => query.HearingRefIds.Contains(x.HearingRefId)).ToListAsync();
        }
    }
}
