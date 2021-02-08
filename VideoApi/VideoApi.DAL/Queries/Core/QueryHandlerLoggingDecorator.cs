using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;

namespace VideoApi.DAL.Queries.Core
{
    public class QueryHandlerLoggingDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery where TResult : class
    {
        private readonly IQueryHandler<TQuery, TResult> _underlyingHandler;

        private readonly ILogger<TQuery> _logger;

        private readonly ILoggingDataExtractor _loggingDataExtractor;

        public QueryHandlerLoggingDecorator(IQueryHandler<TQuery, TResult> underlyingHandler, ILogger<TQuery> logger, ILoggingDataExtractor loggingDataExtractor)
        {
            _logger = logger;
            _underlyingHandler = underlyingHandler;
            _loggingDataExtractor = loggingDataExtractor;
        }

        public async Task<TResult> Handle(TQuery query)
        {
            var properties = _loggingDataExtractor.ConvertToDictionary(query);
            properties.Add(nameof(TQuery), typeof(TQuery).Name);
            properties.Add(nameof(TResult), typeof(TResult).Name);
            using (_logger.BeginScope(properties))
            {
                // Unfortunetely this scope wont apply to the underlying handler as its already been resolved from the logger factory.
                _logger.LogDebug("Handling query");
                var sw = Stopwatch.StartNew();
                var result = await _underlyingHandler.Handle(query);
                _logger.LogDebug("Handled query in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
                return result;
            }
        }
    }
}
