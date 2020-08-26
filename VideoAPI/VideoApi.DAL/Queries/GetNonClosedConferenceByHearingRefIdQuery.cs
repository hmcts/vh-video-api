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

        public GetNonClosedConferenceByHearingRefIdQuery(Guid hearingRefId)
        {
            HearingRefId = hearingRefId;
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
            return await _context.Conferences
                .Include(x => x.Participants)
                .Where(x => x.State != ConferenceState.Closed)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.HearingRefId == query.HearingRefId);
        }
    }
}
