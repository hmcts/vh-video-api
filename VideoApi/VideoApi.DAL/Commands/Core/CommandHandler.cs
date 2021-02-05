using System.Threading.Tasks;

namespace VideoApi.DAL.Commands.Core
{
    public class CommandHandler : ICommandHandler
    {
        private readonly ICommandHandlerFactory _queryHandlerFactory;

        public CommandHandler(ICommandHandlerFactory queryHandlerFactory)
        {
            _queryHandlerFactory = queryHandlerFactory;
        }

        public async Task Handle<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _queryHandlerFactory.Create(command);
            await handler.Handle(command);
        }
    }
}
