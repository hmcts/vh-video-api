using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;

namespace VideoApi
{
    public class LongRunningService : BackgroundService
    {
        private readonly BackgroundWorkerQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public LongRunningService(BackgroundWorkerQueue queue,
            IServiceScopeFactory serviceScopeFactory)
        {
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var command = await _queue.DequeueAsync(stoppingToken);
                await ExecuteCommand(command);
            }
        }

        protected async Task ExecuteCommand<T>(T command) where T: ICommand
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var genericType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
                    var handler = scope.ServiceProvider.GetRequiredService(genericType);
                    var methodInfo = handler.GetType().GetMethod("Handle");
                    var task = (Task) methodInfo.Invoke(handler, new object[] { command });
                    await task.ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }   
}
