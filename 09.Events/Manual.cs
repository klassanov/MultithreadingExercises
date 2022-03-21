using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _09.Events
{
    class Manual
    {
        //Gate: If open, many cars can pass through
        private static ManualResetEventSlim WaitHandle = new ManualResetEventSlim(false);//The gate is initially closed

        public static void Demo()
        {
            for (int i = 0; i < 10; i++)
            {
                var current = i;
                new Thread(Work).Start(current);
            }
            Thread.Sleep(3000);
            Console.WriteLine("Opening the gate");
            WaitHandle.Reset();//Unblocks many thread and it is called once per all threads
        }

        private static void Work(object id)
        {
            Console.WriteLine($"{id} is waiting in front of the gate");
            WaitHandle.Wait();//The thread stops and waits for the gate to open
            Console.WriteLine($"{id} is notified and unblocked");
        }
    }
}
