using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
//using VideoApi.DAL;

namespace VideoApi
{
    public class LongRunningService : BackgroundService
    {
        private readonly BackgroundWorkerQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
            
        public LongRunningService(BackgroundWorkerQueue queue, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    //await workItem(stoppingToken);
                    // Need to identify another way of executing the work item
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {

                        if (workItem.GetMethodInfo().Name.Contains("RemoveHeartbeats"))
                        {
                            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RemoveHeartbeatsForConferencesCommand>>();
                            await handler.Handle(new RemoveHeartbeatsForConferencesCommand());
                        }
                        else if (workItem.GetMethodInfo().Name.Contains("RemoveInstantMessages"))
                        {
                            //var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RemoveInstantMessagesForConferenceCommand>>();
                            //await handler.Handle(new RemoveInstantMessagesForConferenceCommand());
                        }

                    }

                    //var genericType = typeof(ICommandHandler<>).MakeGenericType((typeof(ICommandHandler<RemoveHeartbeatsForConferencesCommand>)));
                    //_serviceProvider.GetService(genericType);

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
