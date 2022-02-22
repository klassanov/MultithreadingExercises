using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _12.ConsumerProducer
{
    class ConsumerProducerQueue : IDisposable
    {
        private readonly Queue<string> tasks = new Queue<string>();
        private readonly object locker = new object();
        private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);
        private readonly Thread worker;

        public ConsumerProducerQueue()
        {
            // 1 thread for dequeuing -> the consumer
            //Enquing is done from the main thread -> the producer
            this.worker = new Thread(Work);
            this.worker.Start();
        }

        public void Dispose()
        {
            this.EnqueueTask(null); // Used to say to the consumer to exit
            this.worker.Join();  // Wait for the consumer's thread to finish.
            this.waitHandle.Close(); //Release OS resources
        }

        public void EnqueueTask(string task)
        {
            lock (this.locker)
            {
                this.tasks.Enqueue(task);
            }

            //Unblock eventually blocked threads
            this.waitHandle.Set();
        }

        private void Work()
        {
            while (true)
            {
                string task = null;
                if (this.tasks.Count > 0)
                {
                    lock (this.locker)
                    {
                        task = this.tasks.Dequeue();
                        if(task == null)
                        {
                            return;
                        }
                    }

                    if(task != null)
                    {
                        Console.WriteLine($"Performing task {task}");
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    //The queue is empty, so I block the thread in order not to consume resources -> while true cycle. When a new task enters, it will be released by the set method
                    // No more tasks - wait for a signal
                    waitHandle.WaitOne();
                }
            }
        }
    }
}
