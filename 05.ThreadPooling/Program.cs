using System;
using System.Threading;
using System.Threading.Tasks;

namespace _05.ThreadPooling
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPoolWork();
        }


        static void ThreadPoolWork()
        {
            //The thread pool cuts these overheads by sharing and reusing threads

            //We do not wait for it and since it is a background thread, the main thread finishes before this
            //Difficult to get result by using this method, better use a Task
            ThreadPool.QueueUserWorkItem(Go, 123);
            
            //In order to see the result
            Thread.Sleep(3000);



            //This is exactly equivalent to
            //Task.Run(() => Go(123)).Wait(); //equivalence with or without wait, not sure
            
            //or to
            //Task.Factory.StartNew() //which has a lot of options
            
        }

        static void Go(object arg)
        {
            Console.WriteLine($"Hello from the thread pool: {arg} ");
            Console.WriteLine($"ManagedThreadId: { Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"IsBackgroundThread: { Thread.CurrentThread.IsBackground}");
            Console.WriteLine($"IsThreadPoolThread: { Thread.CurrentThread.IsThreadPoolThread}");
            Console.WriteLine();
        }
    }
}
