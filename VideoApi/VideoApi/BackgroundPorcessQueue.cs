using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;

namespace VideoApi
{
    public class BackgroundWorkerQueue
    {
        private readonly ConcurrentQueue<ICommand> _workItems = new ConcurrentQueue<ICommand>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        
        public async Task<ICommand> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public Task QueueBackgroundWorkItem(ICommand workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
            return Task.FromResult(workItem);
        }
    }
}
