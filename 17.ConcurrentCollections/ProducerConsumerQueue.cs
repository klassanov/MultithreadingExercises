using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _17.ConcurrentCollections
{
    class ProducerConsumerQueue
    {
        private readonly BlockingCollection<Action> jobs = new BlockingCollection<Action>();


        public ProducerConsumerQueue()
        {
            //Create consumers according to the environment processor number
            for (int i = 0; i < Environment.ProcessorCount / 2; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => Work());
            }
        }

        public void EnqueueJob(Action job)
        {
            if (this.jobs.IsAddingCompleted)
            {
                this.jobs.Add(job);
            }
        }


        public void Dispose() => this.jobs.CompleteAdding();

        private void Work()
        {
            foreach (var job in jobs.GetConsumingEnumerable())
            {
                try
                {
                    job();
                }
                catch (Exception ex)
                {
                    //Exception handling logic
                    throw;
                }
            }
        }
    }
}
