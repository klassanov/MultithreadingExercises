using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _09.Events
{
    class Countdown
    {
        //Wait for N threads to finish and only then proceed
        private static CountdownEvent WaitHandle = new CountdownEvent(3); //How many threads should be waited for
        private static Random random = new Random();

        public static void Demo()
        {
            new Thread(SaySomething).Start("I am thread 1");
            new Thread(SaySomething).Start("I am thread 2");
            new Thread(SaySomething).Start("I am thread 3");

            WaitHandle.Wait();   // Blocks until Signal has been called 3 times, it counts down

            Console.WriteLine("All threads have finished speaking!");
        }

        private static void SaySomething(object s)
        {
            Console.WriteLine($"{s} and I am working");
            Thread.Sleep(random.Next(1000, 3000));

            WaitHandle.Signal();//Signal that the thread has finished
        }
    }
}
