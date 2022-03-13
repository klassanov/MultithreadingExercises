using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    public class TaskBasedConsumerProducerQueue
    {
        private BlockingCollection<WorkItem> jobs = new BlockingCollection<WorkItem>();

        public TaskBasedConsumerProducerQueue()
        {
            for (int i = 0; i < Environment.ProcessorCount / 2; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => this.Work());
            }
        }

        public Task EnqueueWorkItem(Action action) => EnqueueWorkItem(action, null);

        public Task EnqueueWorkItem(Action action, CancellationToken? cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            if (!this.jobs.IsAddingCompleted)
            {
                jobs.Add(new WorkItem(tcs, action, cancellationToken));
            }
            else
            {
                return Task.FromException(new InvalidOperationException("Job queue is no longer operational"));
            }

            return tcs.Task;
        }

        public void Dispose() => this.jobs.CompleteAdding();


        public void Work()
        {
            foreach (var workItem in jobs.GetConsumingEnumerable()) //Thread is blocked if empty
            {
                if (workItem.CancellationToken.HasValue &&
                    workItem.CancellationToken.Value.IsCancellationRequested)
                {
                    workItem.TaskSource.SetCanceled();
                }

                else
                {
                    try
                    {
                        workItem.Action();
                        workItem.TaskSource.SetResult(null); // Indicate completion.
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken == workItem.CancellationToken)
                        {
                            workItem.TaskSource.SetCanceled();
                        }
                        else
                        {
                            workItem.TaskSource.SetException(ex);
                        }
                    }

                    catch (Exception ex)
                    {
                        workItem.TaskSource.SetException(ex);
                    }
                }
            }
        }
    }


    public class WorkItem
    {
        public TaskCompletionSource<object> TaskSource { get; }

        public Action Action { get; }

        public CancellationToken? CancellationToken { get; }

        public WorkItem(TaskCompletionSource<object> taskSource, Action action, CancellationToken? cancellationToken)
        {
            this.TaskSource = taskSource;
            this.Action = action;
            this.CancellationToken = cancellationToken;
        }
    }
}
