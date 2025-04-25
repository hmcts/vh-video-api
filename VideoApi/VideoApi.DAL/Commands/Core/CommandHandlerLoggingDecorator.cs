using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;
using VideoApi.Common.Logging;

namespace VideoApi.DAL.Commands.Core
{
    [SuppressMessage("csharpsquid", "S6672:Generic logger injection should match enclosing type")]
    public class CommandHandlerLoggingDecorator<TCommand>(
        ICommandHandler<TCommand> underlyingHandler,
        ILogger<TCommand> logger,
        ILoggingDataExtractor loggingDataExtractor)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task Handle(TCommand command)
        {
            var properties = loggingDataExtractor.ConvertToDictionary(command);
            properties.Add(nameof(TCommand), typeof(TCommand).Name);
            using (logger.BeginScope(properties))
            {
                logger.LogHandlingCommand(typeof(TCommand).Name);
                var sw = Stopwatch.StartNew();
                await underlyingHandler.Handle(command);
                logger.LogHandledCommand(typeof(TCommand).Name, sw.ElapsedMilliseconds);
            }
        }
    }
}
