using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WordCountInBigFile
{
    class Program
    {
        public static async Task Main()
        {
            //Give high priority to the process, for all the threads
            var currentProcess = Process.GetCurrentProcess();
            currentProcess.PriorityClass = ProcessPriorityClass.High;

            //Calculate page length
            int numThreads = Environment.ProcessorCount;
            int fileLength = 19999;
            int pageSize = (int)Math.Ceiling((double)fileLength / numThreads);




            var hugePath = @"Files\MobyDick.txt";
            var splitParts = @"Files\SplitParts.txt";

            var syncSearcher = new SyncFileSearcher(hugePath);
            var threadSearcher = new ThreadFileSearcher(hugePath);
            var threadLinesSearcher = new ThreadFileLinesSearcher(hugePath);
            var asyncSearcher = new AsyncFileSearcher(hugePath);

            //Sync
            var timer = Stopwatch.StartNew();

            var result = syncSearcher.Search("Moby");

            Console.WriteLine($"Sync: {timer.Elapsed}");

            Console.WriteLine(result);


            //Thread, separating file by space
            timer = Stopwatch.StartNew();

            result = threadSearcher.Search("Moby");

            Console.WriteLine($"Threads, separating file by space: {timer.Elapsed}");

            Console.WriteLine(result);

            //Thread, separating file by lines
            timer = Stopwatch.StartNew();

            result = threadLinesSearcher.Search("Moby");

            Console.WriteLine($"Threads, separating file by line: {timer.Elapsed}");

            Console.WriteLine(result);

            //Async
            timer = Stopwatch.StartNew();

            result = await asyncSearcher.Search("Moby");

            Console.WriteLine($"Async: {timer.Elapsed}");

            Console.WriteLine(result);

            // Validate split words are correctly found.
            threadSearcher = new ThreadFileSearcher(splitParts);

            result = threadSearcher.Search("programming");

            if (result != 100)
            {
                throw new InvalidOperationException("Split parts are not correctly searched in the Threads solution!");
            }

            asyncSearcher = new AsyncFileSearcher(splitParts);

            result = await asyncSearcher.Search("programming");

            if (result != 100)
            {
                throw new InvalidOperationException("Split parts are not correctly searched in the Tasks solution!");
            }
        }
    }
}
