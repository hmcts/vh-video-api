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
    public class GetNonClosedConferenceByHearingRefIdQuery(
        IEnumerable<Guid> hearingRefIds,
        bool includeClosedConferences = false)
        : IQuery
    {
        public GetNonClosedConferenceByHearingRefIdQuery(Guid hearingRefId, bool includeClosedConferences = false) :
            this(new List<Guid> { hearingRefId }, includeClosedConferences) {}
        
        public List<Guid> HearingRefIds { get; } = hearingRefIds.ToList();
        public bool IncludeClosedConferences { get; } = includeClosedConferences;
    }

    public class GetNonClosedConferenceByHearingRefIdQueryHandler(VideoApiDbContext context) : IQueryHandler<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>
    {
        public async Task<List<Conference>> Handle(GetNonClosedConferenceByHearingRefIdQuery query)
        {
            var efQuery = context.Conferences
                .Include(x => x.MeetingRoom)
                .Include(x => x.Participants).ThenInclude(x => x.CurrentConsultationRoom)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints).ThenInclude(x => x.CurrentConsultationRoom)
                .Include(x => x.Endpoints).ThenInclude(x => x.ParticipantsLinked)
                .AsNoTracking();
            
            if (!query.IncludeClosedConferences)
            {
                efQuery = efQuery.Where(x => x.State != ConferenceState.Closed);
            }

            return await efQuery.Where(x => query.HearingRefIds.Contains(x.HearingRefId)).ToListAsync();
        }
    }
}
