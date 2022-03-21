namespace WordCountInBigFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AsyncFileSearcher
    {
        private readonly string [] fileLines;

        public AsyncFileSearcher(string filePath)
            => this.fileLines = File.ReadAllLines(filePath);

        public async Task<int> Search(string searchTerm)
        {
            int numWords = fileLines.Length;
            int numThreads = Environment.ProcessorCount;
            int chunkSize = numWords / numThreads;
            var locker = new object();
            List<Task<int>> tasks = new List<Task<int>>();

            for (int i = 0; i < numThreads; i++)
            {
                var startIndex = i * chunkSize;
                var endIndex = Math.Min(numWords, (i + 1) * chunkSize);

                if (i == numThreads - 1)
                {
                    endIndex = numWords;
                }

                tasks.Add(SearchTerm(searchTerm, startIndex, endIndex));
            }

            var taskSums = await Task.WhenAll(tasks);
            var result = taskSums.Sum();
            return result;
        }

        private Task<int> SearchTerm(string searchTerm, int startIndex, int endIndex)
        {
            return Task.Run(() =>
                    {
                        int localTotal = 0;

                        for (int j = startIndex; j < endIndex; j++)
                        {
                            localTotal += SearchLocal(searchTerm, fileLines[j]);
                        }

                        return localTotal;
                    });
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
