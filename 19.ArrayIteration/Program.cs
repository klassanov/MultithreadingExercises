using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ArrayIterationDemo
{
    class Program
    {
        private static int[] Items = Enumerable.Range(0, 10000000).ToArray();
        private static Dictionary<string, TimeSpan> Results = new Dictionary<string, TimeSpan>();

        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            long regularSum = RegularArrayIteration();
            Results.Add("1. Regular Array Iteration", stopwatch.Elapsed);

            stopwatch = Stopwatch.StartNew();
            long threadPoolTotalsSum = ThreadPoolIteration();
            Results.Add("2. Thread Pool Iteration", stopwatch.Elapsed);

            stopwatch = Stopwatch.StartNew();
            long threadPoolLocalTotalsSum = ThreadPoolLocalTotalsIteration();
            Results.Add("3. Thread Pool Local Totals Iteration", stopwatch.Elapsed);

            stopwatch = Stopwatch.StartNew();
            long threadPoolWithInterlockedTotalsSum = ThreadPoolWithInterlockedIteration();
            Results.Add("4. Thread Pool Interlocked Iteration", stopwatch.Elapsed);

            stopwatch = Stopwatch.StartNew();
            long threadPoolLocalTotalsInterlockedSum = ThreadPoolLocalTotalsInterlockedIteration();
            Results.Add("5. Thread Pool Local Totals Interlocked Iteration", stopwatch.Elapsed);

            PrintResults();

            Debug.Assert(regularSum == threadPoolTotalsSum &&
                         threadPoolTotalsSum == threadPoolLocalTotalsSum &&
                         threadPoolLocalTotalsSum == threadPoolWithInterlockedTotalsSum &&
                         threadPoolWithInterlockedTotalsSum == threadPoolLocalTotalsInterlockedSum,
                        "Sums do not match");
        }

        static long RegularArrayIteration()
        {
            long total = 0;

            for (int i = 0; i < Items.Length; i++)
            {
                total += Items[i];
            }

            return total;
        }

        /// <summary>
        /// Thread pool and lock on each addition operation
        /// </summary>
        /// <returns></returns>
        static long ThreadPoolIteration()
        {
            var locker = new object();
            long total = 0;
            int threadNum = Environment.ProcessorCount;
            int chunkSize = Items.Length / threadNum;
            CountdownEvent countdownHandle = new CountdownEvent(threadNum);

            for (int i = 1; i <= threadNum; i++)
            {
                int startIndex = (i - 1) * chunkSize;
                int endIndex = Math.Min(startIndex + chunkSize - 1, Items.Length);

                if (i == threadNum && endIndex < Items.Length - 1)
                {
                    endIndex = Items.Length - 1;
                }

                var currentStartIndex = startIndex;
                var currentEndIndex = endIndex;

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (int j = currentStartIndex; j <= currentEndIndex; j++)
                    {
                        lock (locker)
                        {
                            total += Items[j];
                        }
                    }

                    countdownHandle.Signal();
                });
            }

            countdownHandle.Wait();
            return total;
        }


        /// <summary>
        /// ThreadPool, local totals and 1 lock per thread when adding to the global totals
        /// </summary>
        /// <returns></returns>
        static long ThreadPoolLocalTotalsIteration()
        {
            var locker = new object();
            long total = 0;
            int threadNum = Environment.ProcessorCount;
            int chunkSize = Items.Length / threadNum;
            CountdownEvent countdownHandle = new CountdownEvent(threadNum);

            for (int i = 1; i <= threadNum; i++)
            {
                int startIndex = (i - 1) * chunkSize;
                int endIndex = Math.Min(startIndex + chunkSize - 1, Items.Length);

                if (i == threadNum && endIndex < Items.Length - 1)
                {
                    endIndex = Items.Length - 1;
                }

                var currentStartIndex = startIndex;
                var currentEndIndex = endIndex;

                ThreadPool.QueueUserWorkItem(_ =>
                {

                    long localTotal = 0;

                    for (int j = currentStartIndex; j <= currentEndIndex; j++)
                    {
                        localTotal += Items[j];
                    }

                    lock (locker)
                    {
                        total += localTotal;
                    }

                    countdownHandle.Signal();
                });
            }

            countdownHandle.Wait();
            return total;
        }

        /// <summary>
        /// ThreadPool, lock with Interlocked on each addition
        /// </summary>
        /// <returns></returns>
        static long ThreadPoolWithInterlockedIteration()
        {
            var locker = new object();
            long total = 0;
            int threadNum = Environment.ProcessorCount;
            int chunkSize = Items.Length / threadNum;
            CountdownEvent countdownHandle = new CountdownEvent(threadNum);

            for (int i = 1; i <= threadNum; i++)
            {
                int startIndex = (i - 1) * chunkSize;
                int endIndex = Math.Min(startIndex + chunkSize - 1, Items.Length);

                if (i == threadNum && endIndex < Items.Length - 1)
                {
                    endIndex = Items.Length - 1;
                }

                var currentStartIndex = startIndex;
                var currentEndIndex = endIndex;

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (int j = currentStartIndex; j <= currentEndIndex; j++)
                    {
                        Interlocked.Add(ref total, Items[j]);                        
                    }

                    countdownHandle.Signal();
                });
            }

            countdownHandle.Wait();
            return total;
        }

        /// <summary>
        /// ThreadPool, 1 lock per thread with Interlocked
        /// </summary>
        /// <returns></returns>
        static long ThreadPoolLocalTotalsInterlockedIteration()
        {
            var locker = new object();
            long total = 0;
            int threadNum = Environment.ProcessorCount;
            int chunkSize = Items.Length / threadNum;
            CountdownEvent countdownHandle = new CountdownEvent(threadNum);

            for (int i = 1; i <= threadNum; i++)
            {
                int startIndex = (i - 1) * chunkSize;
                int endIndex = Math.Min(startIndex + chunkSize - 1, Items.Length);

                if (i == threadNum && endIndex < Items.Length - 1)
                {
                    endIndex = Items.Length - 1;
                }

                var currentStartIndex = startIndex;
                var currentEndIndex = endIndex;

                ThreadPool.QueueUserWorkItem(_ =>
                {

                    long localTotal = 0;

                    for (int j = currentStartIndex; j <= currentEndIndex; j++)
                    {
                        localTotal += Items[j];
                    }
                    
                    Interlocked.Add(ref total, localTotal);

                    countdownHandle.Signal();
                });
            }

            countdownHandle.Wait();
            return total;
        }

        static void PrintResults()
        {
            var orderedDictionary = Results.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (var item in orderedDictionary)
            {
                Console.WriteLine($"{ item.Key} {item.Value}");
            }
        }
    }
}
