using System;
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

    public class RemoveHeartbeatsForConferencesCommandHandler : ICommandHandler<RemoveHeartbeatsForConferencesCommand>
    {
        private readonly VideoApiDbContext _context;
        public RemoveHeartbeatsForConferencesCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        public async Task Handle(RemoveHeartbeatsForConferencesCommand command)
        {
            _context.Database.SetCommandTimeout(3600); //1 hour
            
            var heartBeatsToDeleteQuery = _context.Heartbeats
                .Where(hb => hb.Timestamp <= DateTime.UtcNow.AddDays(-14))
                .AsQueryable();

            await heartBeatsToDeleteQuery.DeleteAsync();
        }
    }
}
