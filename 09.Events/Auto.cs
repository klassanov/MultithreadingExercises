using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace _09.Events
{
    class Auto
    {
        //Parking bareer: Only 1 car can pass through - as parking

        private static EventWaitHandle autoResetEvent = new AutoResetEvent(false); //false=Initially closed

        public static void Demo()
        {
            Console.WriteLine("Starting Demo");
            Console.WriteLine("Starting second thread");
            new Thread(Work).Start();
            Thread.Sleep(3000);
            Console.WriteLine("Unblocking the second thread");
            autoResetEvent.Set(); //Set unblocks 1 thread and we should call it once per each different thread
        }

        private static void Work()
        {
            Console.WriteLine("Waiting for a signal - blocked");
            autoResetEvent.WaitOne();//Stop and wait for a signal
            Console.WriteLine("Notified and unblocked");
        }
    }
}
