namespace WordCountInBigFile
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class ThreadFileSearcher
    {
        private readonly string fileContent;

        public ThreadFileSearcher(string filePath)
            => this.fileContent = File.ReadAllText(filePath);

        public int Search(string searchTerm)
        {
            int result = 0;
            string[] splitFileContent = this.fileContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int numWords = splitFileContent.Length;
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
                        if (splitFileContent[j].Contains(searchTerm))
                        {
                            localTotal++;
                        }
                    }

                    return localTotal;
                },

                localFinally: (localTotal) =>
                {
                    Interlocked.Add(ref result, localTotal);
                    //lock (locker)
                    //{
                    //    result += localTotal;
                    //}
                }
           );

            return result;
        }
    }
}
