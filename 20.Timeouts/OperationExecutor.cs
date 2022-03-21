using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _20.Timeouts
{
    class OperationExecutor
    {
        private readonly ManualResetEvent manualResetEvent;
        private readonly IOperation operation;

        public OperationExecutor(IOperation operation)
        {
            this.operation = operation;
            this.manualResetEvent = new ManualResetEvent(false);
        }

        public void StartWithTimeout(TimeSpan timeout)
        {
            manualResetEvent.Reset();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool wasSignaled = manualResetEvent.WaitOne(timeout);
                if (!wasSignaled)
                {
                    Console.WriteLine("Operation timeout expired, it will execute");
                    this.operation.Execute();
                }
                else
                {
                    Console.WriteLine("Operation aborted");
                }

            });
        }

        public void AbortOperationIfNotStarted()
        {
            Console.WriteLine("Aborting operation");
            manualResetEvent.Set();
        }

        /// <summary>
        /// Abort multiple operations (StartWIthTimeout)
        /// </summary>
        public void AbortOperationIfNotStartedMultipleOperations()
        {
            //Don't want to block the caller thread => I use the thread pool
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine("Aborting operation");
                manualResetEvent.Set(); //Signal
                Thread.Sleep(100); // Give time to all the threads to go to the else block to "Operation aborted", before resetting
                manualResetEvent.Reset(); //Not sure if needed
            });

            //Since Thread.Sleep(100) is needed, if I execute it on the caller thread, I would block it for 100 ms, which I do not want to
        }
    }
}
