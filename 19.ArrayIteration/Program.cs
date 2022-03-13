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
            long threadPoolSum = ThreadPoolLocalTotalsIteration();
            Results.Add("3. Thread Pool Local Totals Iteration", stopwatch.Elapsed);



            PrintResults();
            Debug.Assert(regularSum == threadPoolSum, "Sums do not match");
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

                ThreadPool.QueueUserWorkItem(_ => {

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
