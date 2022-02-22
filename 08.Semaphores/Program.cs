using System;
using System.Threading;

namespace _08.Semaphores
{
    class Program
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(2, 2);

        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                Thread t = new Thread(Enter);
                t.Start();
            }
        }

        static void Enter()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} wants to enter");
            
            semaphoreSlim.Wait();

            Console.WriteLine($" Thread {Thread.CurrentThread.ManagedThreadId} is in");
            Thread.Sleep(Thread.CurrentThread.ManagedThreadId * 100);

            semaphoreSlim.Release();

            Console.WriteLine($" Thread {Thread.CurrentThread.ManagedThreadId} is out");
        }
    }
}
