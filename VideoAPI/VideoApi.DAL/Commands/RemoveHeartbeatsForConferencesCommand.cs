using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;

namespace VideoApi.DAL.Commands
{
    public class RemoveHeartbeatsForConferencesCommand : ICommand
    {
        public RemoveHeartbeatsForConferencesCommand() { }
    }

    public class RemoveHeartbeatsForConferencesCommandHandler : ICommandHandler<RemoveHeartbeatsForConferencesCommand>
    {
        private readonly VideoApiDbContext _context;
        public RemoveHeartbeatsForConferencesCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        public async Task Handle(RemoveHeartbeatsForConferencesCommand command)
        {
            var expiredConferences = _context.Conferences.Where(c => c.ScheduledDateTime <= DateTime.UtcNow.AddDays(-14)).Select(c => c.Id);
            var expiredHeartbeats = await _context.Heartbeats.Where(x => expiredConferences.Contains(x.ConferenceId)).ToListAsync();
            _context.RemoveRange(expiredHeartbeats);
            await _context.SaveChangesAsync();
        }
    }
}
