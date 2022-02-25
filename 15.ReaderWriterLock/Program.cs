using System;
using System.Collections.Generic;
using System.Threading;

namespace _15.ReaderWriterLock
{
    class Program
    {
        //Many reads can occur in the same time, only 1 write can occur at the same time => optimization

        //The write lock is all exclusive – blocks every single thread
        //The read lock is does not block, if the write lock is free
        //Correct way to do it with try/finally because we ensure that the release occurs event if we have an exception in the try block
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        private static List<int> Items = new List<int>();
        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            new Thread(Read).Start();
            new Thread(Read).Start();
            new Thread(Read).Start();

            new Thread(Write).Start();
            new Thread(Write).Start();
        }


        private static void Read()
        {
            while (true)
            {
                try
                {
                    Locker.EnterReadLock();

                    foreach (var item in Items)
                    {
                        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} read {item}");
                        Thread.Sleep(100);
                    }
                }

                finally
                {
                    Locker.ExitReadLock();
                }
            }
        }

        private static void Write()
        {
            while (true)
            {
                int num = GetRandNum(100);

                try
                {
                    Locker.EnterWriteLock();

                    Items.Add(num);
                }

                finally
                {
                    Locker.ExitWriteLock();
                }
                
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} added {num}");
                Thread.Sleep(1000);
            }
        }

        private static int GetRandNum(int max)
        {
            lock (Random)
            {
                return Random.Next(max);
            }
        }
    }
}
