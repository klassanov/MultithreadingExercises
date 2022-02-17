using System;
using System.Threading;

namespace _02.Threads
{
    class Program
    {
        static void Main(string[] args)
        {
            MultipleThreads();
        }

        public static void MultipleThreads()
        {
            var threadA = new Thread(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    Console.Write("A");
                }
            });

            threadA.Start();


            for (int i = 0; i < 1000; i++)
            {
                Console.Write("B");
            }

        }
    }
}
