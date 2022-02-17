using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace _06.Locking
{
    class Program
    {
        private static List<int> Data = Enumerable.Range(0, 100).ToList();
        private static object locker = new object();

        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                var thread = new Thread(Work);
                thread.Start();
            }

            Console.WriteLine("Hello World!");
        }

        static void Work()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(500);
                lock (locker)
                {
                    Data.RemoveAt(Data.Count - 1);
                }
            }
        }
    }
}
