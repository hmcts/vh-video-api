using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferenceForParticipantQuery(Guid participantId) : IQuery
    {
        public Guid ParticipantId { get; } = participantId;

    }

    public class GetConferenceForParticipantQueryHandler(VideoApiDbContext context) :
        IQueryHandler<GetConferenceForParticipantQuery, Conference>
    {

        public async Task<Conference> Handle(GetConferenceForParticipantQuery query)
        {
            var participant = await context.Participants.SingleOrDefaultAsync(p => p.Id == query.ParticipantId);

            if (participant == null)
                return null;
            
            return await context.Conferences
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == participant.ConferenceId);
        }
    }
}
