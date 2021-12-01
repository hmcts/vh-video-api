using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetQuickLinkParticipantByIdQuery : IQuery
    {
        public GetQuickLinkParticipantByIdQuery(Guid participantId)
        {
            ParticipantId = participantId;
        }
        public Guid ParticipantId { get; set; }

    }

    public class GetQuickLinkParticipantByIdQueryHandler : IQueryHandler<GetQuickLinkParticipantByIdQuery, ParticipantBase>
    {
        private readonly VideoApiDbContext _context;

        public GetQuickLinkParticipantByIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<ParticipantBase> Handle(GetQuickLinkParticipantByIdQuery query)
        {
            return await _context.Participants
                .AsNoTracking()
                .SingleOrDefaultAsync(x =>
                x.Id == query.ParticipantId);
        }
    }
}
