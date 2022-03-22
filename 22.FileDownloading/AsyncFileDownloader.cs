using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _22.FileDownloading
{
    public class AsyncFileDownloader
    {
        private readonly string[] urls;
        private readonly CountdownEvent countdown;
        private readonly SemaphoreSlim semaphore;
        private ConcurrentDictionary<int, int> percentageProgress;
        private ConcurrentDictionary<int, long> downloadProgress;
        private readonly int maxNumThreads = 3;
        private long totalFilesSizeBytes;
        private Timer timer;

        public AsyncFileDownloader(string[] urls)
        {
            this.urls = urls;
            this.countdown = new CountdownEvent(urls.Length);
            this.semaphore = new SemaphoreSlim(this.maxNumThreads);
            this.percentageProgress = new ConcurrentDictionary<int, int>();
            this.downloadProgress = new ConcurrentDictionary<int, long>();
        }

        public void Download()
        {
            Console.WriteLine("Starting download...");

            CalculateTotalFileSize();


            for (int i = 0; i < urls.Length; i++)
            {
                int fileOrder = i;
                string currentUrl = urls[i];

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    DownloadFile(currentUrl, fileOrder);
                });
            }


            this.StartReporting();

            countdown.Wait();

            this.timer.Dispose();

            this.ReportResults();

        }

        private void CalculateTotalFileSize()
        {
            Parallel.For(0, urls.Length, new ParallelOptions { MaxDegreeOfParallelism = this.maxNumThreads },
               localInit: () => 0L,
               body: (i, state, localTotal) =>
               {
                   localTotal += GetFileSize(this.urls[i]);
                   return localTotal;
               },
               localFinally: (localTotal) => { Interlocked.Add(ref this.totalFilesSizeBytes, localTotal); });
        }

        private void DownloadFile(string url, int fileOrder)
        {
            this.semaphore.Wait();

            try
            {
                var webClient = new WebClient();

                webClient.DownloadProgressChanged += (obj, progress) =>
                {
                    this.percentageProgress[fileOrder] = progress.ProgressPercentage;
                    this.downloadProgress[fileOrder] = progress.BytesReceived;
                };

                webClient.DownloadFileCompleted += (obj, data) =>
                {
                    this.countdown.Signal();
                    this.semaphore.Release();
                };

                webClient.DownloadFileAsync(new Uri(url), fileOrder.ToString());
            }
            catch
            {
                this.countdown.Signal();
                this.semaphore.Release();
            }
        }

        private long GetFileSize(string url)
        {
            var webClient = new WebClient();

            using var readStream = webClient.OpenRead(url);

            return long.Parse(webClient.ResponseHeaders["Content-Length"]);
        }

        private void StartReporting()
        {
            this.timer = new Timer(_ =>
                {
                    this.ReportResults();
                },
                null,
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(1));
        }

        private void ReportResults()
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            var totalDownloadedBytes = this.downloadProgress.Values.Sum();
            var totalDownloadedPercentage = ((double)totalDownloadedBytes / this.totalFilesSizeBytes) * 100;

            var totalDownloadedMb = totalDownloadedBytes / 2 ^ 20;
            var totalFileSizeMb = totalFilesSizeBytes / 2 ^ 20;

            Console.WriteLine($"Progress: {totalDownloadedMb}/{totalFileSizeMb} MB - Percentage: {totalDownloadedPercentage:F2}%");

            foreach (var (key, value) in this.percentageProgress)
            {
                Console.SetCursorPosition(0, key + 1);
                Console.Write($"{key}: {value}%");
            }
        }
    }
}
