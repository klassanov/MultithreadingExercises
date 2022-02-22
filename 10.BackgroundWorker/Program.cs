using System;
using System.ComponentModel;
using System.Threading;

namespace BackgroundWorkerDemo
{
    class Program
    {
        private static BackgroundWorker worker;

        static void Main(string[] args)
        {
            //Uses the ThreadPool
            worker = new BackgroundWorker() {
                WorkerReportsProgress = true, 
                WorkerSupportsCancellation = true
            };

            worker.DoWork += OnDoWork;
            worker.RunWorkerCompleted += OnWorkCompleted;
            worker.ProgressChanged += OnProgressChanged;
            worker.RunWorkerAsync("Worker is starting");

            Console.WriteLine("Press Enter in the next 5 seconds to cancel");
            Console.ReadLine();

            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }

            Console.ReadLine();
        }

        private static void OnDoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine(e.Argument);

            for (int i = 0; i <= 100; i+=20)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                worker.ReportProgress(i);
                Thread.Sleep(1000);
            }
        }

        private static void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine($"Reached {e.ProgressPercentage} %");
        }

        private static void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("Work cancelled");
            }

            else if (e.Error != null)
            {
                Console.WriteLine($"Worker exception {e.Error}");
            }

            else
            {
                Console.WriteLine($"Completed {e.Result}");
            }
        }
    }
}
