using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Helpers;

namespace VideoApi.Extensions
{
    public class LoggingMiddleware : IAsyncActionFilter
    {
        private readonly ILogger<LoggingMiddleware> _logger;

        private readonly ILoggingDataExtractor _loggingDataExtractor;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger, ILoggingDataExtractor loggingDataExtractor)
        {
            _logger = logger;
            _loggingDataExtractor = loggingDataExtractor;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var properties = context.ActionDescriptor.Parameters.Select(p => context.ActionArguments.SingleOrDefault(x => x.Key == p.Name)).SelectMany(pv => _loggingDataExtractor.ConvertToDictionary(pv.Value, pv.Key)).ToDictionary(x => x.Key, x => x.Value);

            var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                properties.Add(nameof(actionDescriptor.ControllerName), actionDescriptor.ControllerName);
                properties.Add(nameof(actionDescriptor.ActionName), actionDescriptor.ActionName);
                properties.Add(nameof(actionDescriptor.DisplayName), actionDescriptor.DisplayName);
            }
            
            using (_logger.BeginScope(properties))
            {
                await next();
            }
        }
    }
}
