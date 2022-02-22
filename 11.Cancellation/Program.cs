using System;
using System.Threading;

namespace _11.Cancellation
{
    class Program
    {
        //The correct way to cancel a thread

        static void Main(string[] args)
        {
            var cancellationToken = new CancellationTokenSource();
            //A way to set Thread timeout
            //var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)); 
            var t = new Thread(()=> {
                Work(cancellationToken.Token);
            });

            t.Start();


            Console.WriteLine("Enter 'stop' to cancel...");

            while (true)
            {
                var line = Console.ReadLine();
                if (line.ToLower() == "stop")
                {
                    cancellationToken.Cancel();
                    break;
                }
            }


        }

        static void Work(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Working...");
                Thread.Sleep(1000);

                if(cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Cancelling");
                    return;
                }
            }

            Console.WriteLine("Finished");
        }
    }
}
