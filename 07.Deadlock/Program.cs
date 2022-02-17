using System;
using System.Threading;

namespace _07.Deadlock
{
    class Program
    {
        private static object locker1 = new object();
        private static object locker2 = new object();

        static void Main(string[] args)
        {
            var th1 = new Thread(Method1);
            var th2 = new Thread(Method2);

            th1.Start();
            th2.Start();
        }


        private static void Method1()
        {
            lock (locker1)
            {
                Console.WriteLine($" Thread Id: {Thread.CurrentThread.ManagedThreadId} locker1 locked");

                Thread.Sleep(500);

                lock (locker2)
                {
                    Console.WriteLine($" Thread Id: {Thread.CurrentThread.ManagedThreadId} locker2 locked");
                }
            }
        }


        private static void Method2()
        {
            lock (locker2)
            {
                Console.WriteLine($" Thread Id: {Thread.CurrentThread.ManagedThreadId} locker2 locked");

                Thread.Sleep(500);

                lock (locker1)
                {
                    Console.WriteLine($" Thread Id: {Thread.CurrentThread.ManagedThreadId} locker1 locked");
                }
            }
        }
    }
}
