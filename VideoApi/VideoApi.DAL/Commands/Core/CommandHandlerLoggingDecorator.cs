using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;

namespace VideoApi.DAL.Commands.Core
{
    public class CommandHandlerLoggingDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _underlyingHandler;

        private readonly ILogger<TCommand> _logger;

        private readonly ILoggingDataExtractor _loggingDataExtractor;

        public CommandHandlerLoggingDecorator(ICommandHandler<TCommand> underlyingHandler, ILogger<TCommand> logger, ILoggingDataExtractor loggingDataExtractor)
        {
            _logger = logger;
            _underlyingHandler = underlyingHandler;
            _loggingDataExtractor = loggingDataExtractor;
        }

        public async Task Handle(TCommand command)
        {
            var properties = _loggingDataExtractor.ConvertToDictionary(command);
            properties.Add(nameof(TCommand), typeof(TCommand).Name);
            using (_logger.BeginScope(properties))
            {
                _logger.LogDebug("Handling command");
                var sw = Stopwatch.StartNew();
                await _underlyingHandler.Handle(command);
                _logger.LogDebug("Handled command in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
            }
        }
    }
}
