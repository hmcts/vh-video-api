using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Queries.Core;
using Task = VideoApi.Domain.Task;

namespace VideoApi.DAL.Queries
{
    public class GetTasksForConferenceQuery : IQuery
    {
        public GetTasksForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; }
    }

    public class GetTasksForConferenceQueryHandler : IQueryHandler<GetTasksForConferenceQuery, List<Task>>
    {
        private readonly VideoApiDbContext _context;

        public GetTasksForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Task>> Handle(GetTasksForConferenceQuery query)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Where(x => x.ConferenceId == query.ConferenceId)
                .OrderByDescending(x => x.Created)
                .ToListAsync();
        }
    }
}
