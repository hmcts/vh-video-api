using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetConferenceByHearingRefIdQuery : IQuery
    {
        public Guid HearingRefId { get; }

        public GetConferenceByHearingRefIdQuery(Guid hearingRefId)
        {
            HearingRefId = hearingRefId;
        }
    }

    public class GetConferenceByHearingRefIdQueryHandler :
        IQueryHandler<GetNonClosedConferenceByHearingRefIdQuery, Conference>
    {
        private readonly VideoApiDbContext _context;

        public GetConferenceByHearingRefIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<Conference> Handle(GetNonClosedConferenceByHearingRefIdQuery query)
        {
            return await _context.Conferences
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.HearingRefId == query.HearingRefId);
        }
    }
}
