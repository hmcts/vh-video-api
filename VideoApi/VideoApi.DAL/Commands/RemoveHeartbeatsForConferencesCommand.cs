using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using Z.EntityFramework.Plus;

namespace VideoApi.DAL.Commands
{
    public class RemoveHeartbeatsForConferencesCommand : ICommand
    {
        public RemoveHeartbeatsForConferencesCommand() { }
    }

    [ExcludeFromCodeCoverage]
    public class RemoveHeartbeatsForConferencesCommandHandler : ICommandHandler<RemoveHeartbeatsForConferencesCommand>
    {
        private readonly VideoApiDbContext _context;
        public RemoveHeartbeatsForConferencesCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        public async Task Handle(RemoveHeartbeatsForConferencesCommand command)
        {
            var expiredConferenceIdsQuery = 
                _context.Conferences
                    .Where(c => c.ScheduledDateTime <= DateTime.UtcNow.AddDays(-14))
                    .Select(c => c.Id);
            
            await _context.Heartbeats
                .Where(e => expiredConferenceIdsQuery.Contains(e.ConferenceId))
                .DeleteAsync();
        }
    }
}
