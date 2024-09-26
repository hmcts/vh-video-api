using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace VideoApi.DAL.Queries
{
    public class GetConferencesByTelephoneIdQuery(string telephoneConferenceId) : IQuery
    {
        public string TelephoneConferenceId { get; set; } = telephoneConferenceId;
    }

    public class GetConferencesByTelephoneIdQueryHandler : IQueryHandler<GetConferencesByTelephoneIdQuery, List<Conference>>
    {
        private readonly VideoApiDbContext _context;
        
        public GetConferencesByTelephoneIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Conference>> Handle(GetConferencesByTelephoneIdQuery query)
        {
            return await _context.Conferences
                .Include(x => x.MeetingRoom)
                .Where(x => x.MeetingRoom.TelephoneConferenceId == query.TelephoneConferenceId)
                .ToListAsync();
        }
    }

}
