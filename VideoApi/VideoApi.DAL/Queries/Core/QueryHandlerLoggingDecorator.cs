using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Common.Helpers;

namespace VideoApi.DAL.Queries.Core
{
    [SuppressMessage("csharpsquid", "S6672:Generic logger injection should match enclosing type")]
    public class QueryHandlerLoggingDecorator<TQuery, TResult>(
        IQueryHandler<TQuery, TResult> underlyingHandler,
        ILogger<TQuery> logger,
        ILoggingDataExtractor loggingDataExtractor)
        : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery
        where TResult : class
    {
        public async Task<TResult> Handle(TQuery query)
        {
            var properties = loggingDataExtractor.ConvertToDictionary(query);
            properties.Add(nameof(TQuery), typeof(TQuery).Name);
            properties.Add(nameof(TResult), typeof(TResult).Name);
            using (logger.BeginScope(properties))
            {
                // Unfortunately this scope won't apply to the underlying handler as its already been resolved from the logger factory.
                logger.LogDebug("Handling query");
                var sw = Stopwatch.StartNew();
                var result = await underlyingHandler.Handle(query);
                logger.LogDebug("Handled query in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
                return result;
            }
        }
    }
}
