using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetDistinctJudgeListByFirstNameQuery : IQuery {}

    public class GetDistinctJudgeListByFirstNameQueryHandler : IQueryHandler<GetDistinctJudgeListByFirstNameQuery, List<string>>
    {
        private readonly VideoApiDbContext _context;

        public GetDistinctJudgeListByFirstNameQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> Handle(GetDistinctJudgeListByFirstNameQuery query)
        {
            var participants = await _context.Participants
                .AsNoTracking()
                .Where(p => p.UserRole == UserRole.Judge)
                .Select(x => x.FirstName)
                .Distinct()
                .ToListAsync();

            return participants;
        }
    }
}
