using System;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class SaveHeartbeatCommand : ICommand
    {
        public Heartbeat Heartbeat { get;private set; }

        public SaveHeartbeatCommand(Heartbeat heartbeat)
        {
            this.Heartbeat = heartbeat;    
        }
    }

    public class SaveHeartbeatCommandHandler : ICommandHandler<SaveHeartbeatCommand>
    {
        private readonly VideoApiDbContext _context;

        public SaveHeartbeatCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SaveHeartbeatCommand command)
        {

            await _context.Heartbeats.AddAsync(command.Heartbeat);

            await _context.SaveChangesAsync();
        }
    }
}
