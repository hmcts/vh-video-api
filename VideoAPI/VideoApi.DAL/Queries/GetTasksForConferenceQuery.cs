using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
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
            var conference = await _context.Conferences.Include(x => x.Tasks)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(query.ConferenceId);
            }

            return conference.GetTasks().ToList();
        }
    }
}