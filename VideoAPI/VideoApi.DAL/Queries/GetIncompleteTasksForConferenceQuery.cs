using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = VideoApi.Domain.Task;
using TaskStatus = VideoApi.Domain.Enums.TaskStatus;

namespace VideoApi.DAL.Queries
{
    public class GetIncompleteTasksForConferenceQuery : IQuery
    {
        public GetIncompleteTasksForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; }
    }

    public class
        GetIncompleteTasksForConferenceQueryHandler : IQueryHandler<GetIncompleteTasksForConferenceQuery, List<Task>>
    {
        private readonly VideoApiDbContext _context;

        public GetIncompleteTasksForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Task>> Handle(GetIncompleteTasksForConferenceQuery query)
        {
            var conference = await _context.Conferences.Include(x => x.Tasks)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(query.ConferenceId);
            }

            var alerts = conference.GetTasks().Where(x => x.Status == TaskStatus.ToDo).ToList();
            return alerts;
        }
    }
}