using System;
using System.Threading;

namespace _04.JoinAndSleep
{
    class Program
    {
        static void Main(string[] args)
        {
            var thread = new Thread(Go);
            thread.Start();

            //Code will run here together with the Go method after the start and before the join

            //Here we stop and wait
            //Wait for the thread to execute
            //Works fine when we have only 1 thread, but not when we have many threads
            thread.Join();

            Console.WriteLine("Hello world");
        }

        static void Go()
        {
            Thread.Sleep(1000);
            Console.WriteLine("Slept!");
        }
    }
}
