using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class CheckConferenceOpenQuery : IQuery
    {
        public CheckConferenceOpenQuery(DateTime scheduledDateTime, string caseNumber, string caseName)
        {
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
            CaseName = caseName;
        }

        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
    }

    public class CheckConferenceOpenQueryHandler : IQueryHandler<CheckConferenceOpenQuery, Conference>
    {
        private readonly VideoApiDbContext _context;

        public CheckConferenceOpenQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<Conference> Handle(CheckConferenceOpenQuery query)
        {
            return await _context.Conferences
                .AsNoTracking()
                .SingleOrDefaultAsync(x =>
                x.ScheduledDateTime == query.ScheduledDateTime
                && x.CaseName == query.CaseName
                && x.CaseNumber == query.CaseNumber
                && x.State != ConferenceState.Closed).ConfigureAwait(false);
        }
    }
}