using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetIncompleteAlertsForConferenceQuery : IQuery
    {
        public GetIncompleteAlertsForConferenceQuery(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; }
    }

    public class
        GetIncompleteAlertsForConferenceQueryHandler : IQueryHandler<GetIncompleteAlertsForConferenceQuery, List<Alert>>
    {
        private readonly VideoApiDbContext _context;

        public GetIncompleteAlertsForConferenceQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alert>> Handle(GetIncompleteAlertsForConferenceQuery query)
        {
            var conference = await _context.Conferences.Include(x => x.Alerts)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(query.ConferenceId);
            }

            var alerts = conference.GetAlerts().Where(x => x.Status == AlertStatus.ToDo).ToList();
            return alerts;
        }
    }
}