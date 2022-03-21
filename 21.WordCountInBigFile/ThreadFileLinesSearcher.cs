using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordCountInBigFile
{
    public class ThreadFileLinesSearcher
    {
        private readonly string[] fileContent;

        public ThreadFileLinesSearcher(string filePath) =>
            this.fileContent = File.ReadAllLines(filePath);


        public int Search(string searchTerm)
        {
            int result = 0;
            int numWords = fileContent.Length;
            int numThreads = Environment.ProcessorCount;
            int chunkSize = numWords / numThreads;
            var locker = new object();


            Parallel.For(0, numThreads,

                localInit: () => 0,

                body: (i, state, localTotal) =>
                {
                    var startIndex = i * chunkSize;
                    var endIndex = Math.Min(numWords, (i + 1) * chunkSize);

                    if (i == numThreads - 1)
                    {
                        endIndex = numWords;
                    }

                    for (int j = startIndex; j < endIndex; j++)
                    {
                        localTotal += SearchLocal(searchTerm, fileContent[j]);
                    }

                    return localTotal;
                },

                localFinally: (localTotal) =>
                {
                    Interlocked.Add(ref result, localTotal);
                });

            return result;
        }

        private int SearchLocal(string searchTerm, string content)
        {
            var currentIndex = -1;
            var count = 0;

            while (true)
            {
                currentIndex = content.IndexOf(
                    searchTerm,
                    currentIndex + 1,
                    StringComparison.InvariantCulture);

                if (currentIndex < 0)
                {
                    break;
                }

                count++;
            }

            return count;
        }
    }
}
