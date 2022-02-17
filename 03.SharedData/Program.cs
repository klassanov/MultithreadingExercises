using System;
using System.Threading;

namespace _03.SharedData
{
    class Program
    {
        private static bool isDone;

        static void Main(string[] args)
        {
            //new Thread(Go).Start();
            //Go();

            CapturedVariables();
        }

        static void Go()
        {
            if(!isDone)
            {
                Console.WriteLine("Done!");
                isDone = true;
            }
        }

        static void CapturedVariables()
        {
            for (int i = 0;
                i < 10;
                i++)
            {
                //We take a picture of i at the moment of the thread creation and then print it, it work OK
                //var current = i;

                new Thread(() =>
                {
                    //Thread.Sleep(3000);

                    //i is shared and it will print the value it has in the moment of thread execution (can be 10, the last icrement
                    Console.Write(i);
                    
                    //Console.Write(current);
                }).Start();
            }
        }
            
    }
}
