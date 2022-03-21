using System;
using System.Threading;

namespace _20.Timeouts
{
    class Program
    {

        static void Main(string[] args)
        {
            //DialogTimeout.Demo();

            var operationExecutor = new OperationExecutor(new Operation());
            Console.WriteLine("Starting with timeout of 5s");
            operationExecutor.StartWithTimeout(TimeSpan.FromSeconds(5));
            Thread.Sleep(8000);

            Console.WriteLine("Starting with timeout with timeout of 5s, but aborting after 2s");
            operationExecutor.StartWithTimeout(TimeSpan.FromSeconds(5));
            Thread.Sleep(2000);
            operationExecutor.AbortOperationIfNotStarted();
            Thread.Sleep(8000);

            Console.WriteLine("Starting with timeout of 5s again");
            operationExecutor.StartWithTimeout(TimeSpan.FromSeconds(5));
            Thread.Sleep(8000);
        }
    }
}
