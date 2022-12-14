using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoApi
{
    public class BackgroundWorkerQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
        //private readonly ConcurrentQueue<Func<CancellationToken, ICommand>> _workItems2 = new ConcurrentQueue<Func<CancellationToken, ICommand>>();
        private readonly ConcurrentQueue<object> _workItems3 = new ConcurrentQueue<object>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
        
        public async Task<object> DequeueAsync2(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems3.TryDequeue(out var workItem);

            return workItem;
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }
        
        public void QueueBackgroundWorkItem(object workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems3.Enqueue(workItem);
            _signal.Release();
        }
    }
}
