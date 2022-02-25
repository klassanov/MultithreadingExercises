using System;
using System.Threading;

namespace BarrierDemo
{
    class Program
    {
        //Barrier: Signals that a participant has reached the barrier and waits for all other participants to reach the barrier as well
        private static readonly Barrier Barrier = new Barrier(3, (b)=>
        {
            //Called once on one of the threads but all are unblocked. Not really needed
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} unblocked");
        });

        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            new Thread(Speak).Start();
            new Thread(Speak).Start();
            new Thread(Speak).Start();
        }


        private static void Speak()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i + " ");
                Thread.Sleep(Random.Next(1000, 3000));
                Barrier.SignalAndWait();
            }
        }
    }
}
