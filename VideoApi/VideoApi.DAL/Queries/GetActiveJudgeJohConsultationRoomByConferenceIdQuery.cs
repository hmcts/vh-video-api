using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using System.Collections.Generic;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Queries
{
    public class GetActiveJudgeJohConsultationRoomByConferenceIdQuery : IQuery
    {
        public Guid ConferenceId;
    }
 
    public class GetActiveJudgeJohConsultationRoomByConferenceIdQueryHandler : IQueryHandler<GetActiveJudgeJohConsultationRoomByConferenceIdQuery, List<Room>>
    {
        private readonly VideoApiDbContext _context;
        public GetActiveJudgeJohConsultationRoomByConferenceIdQueryHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        public async Task<List<Room>> Handle(GetActiveJudgeJohConsultationRoomByConferenceIdQuery query)
        {
           return await _context.Rooms.Where(x => x.ConferenceId == query.ConferenceId&& x.Status == Domain.Enums.RoomStatus.Live && x.Type==VirtualCourtRoomType.JudgeJOH).ToListAsync();
        }
    }
}
