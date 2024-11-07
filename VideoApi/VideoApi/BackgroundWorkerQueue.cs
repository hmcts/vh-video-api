using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;

namespace VideoApi
{
    public interface IBackgroundWorkerQueue
    {
        Task<ICommand> DequeueAsync(CancellationToken cancellationToken);
        Task QueueBackgroundWorkItem(ICommand workItem);
    }

    public class BackgroundWorkerQueue : IBackgroundWorkerQueue
    {
        private readonly ConcurrentQueue<ICommand> _workItems = new();
        private readonly SemaphoreSlim _signal = new(0);
        
        public async Task<ICommand> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public Task QueueBackgroundWorkItem(ICommand workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);

            _workItems.Enqueue(workItem);
            _signal.Release();
            return Task.FromResult(workItem);
        }
    }
}
