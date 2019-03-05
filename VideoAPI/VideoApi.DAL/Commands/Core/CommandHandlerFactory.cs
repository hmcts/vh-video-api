using System;

namespace VideoApi.DAL.Commands.Core
{
    public class CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommandHandler<TCommand> Create<TCommand>(TCommand command) where TCommand : ICommand
        {
            var genericType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            return (ICommandHandler<TCommand>)_serviceProvider.GetService(genericType); 
            // Service Locator is anti-pattern. Need to figure out better way of managing this
        }
    }
}