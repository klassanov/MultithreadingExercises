using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace ParallelDemo
{
    class Program
    {
        static void Main(string[] args)
        {




            //The Parallel class

            //Invoke multiple jobs
            Parallel.Invoke(() => new WebClient().DownloadFile("https://google.com", "google.html"),
                            () => new WebClient().DownloadFile("https://abv.bg", "abv.html"));



            //Invoke multiple jobs and save the result
            var bag = new ConcurrentBag<string>();

            Parallel.Invoke(() => bag.Add(new WebClient().DownloadString("https://google.com")),
                            () => bag.Add(new WebClient().DownloadString("https://abv.bg")));


            //For loop
            Parallel.For(0, 100, i => Console.Write(i + " "));

            Console.WriteLine();


            //Foreach loop
            Parallel.ForEach("Parallel?", (c, state) =>
            {
                if (c != '?')
                {
                    Console.WriteLine(c);
                }
                else
                {
                    state.Break();
                    //state.Stop();//Slightly different from break
                }
            });



            // Calculations with local thread-safe values.

            // Bad example - too many locks.
            //var badLocker = new object();
            //var badTotal = 0.0;
            //Parallel.For(1, 10000000,
            //    i =>
            //    {
            //        lock (badLocker)
            //        {
            //            badTotal += Math.Sqrt(i);
            //        }
            //    });

            //Console.WriteLine(badTotal);



            //Good example one lock per thread only when adding back the thread-scope calculated total to the global total varaiable
            var goodLocker = new object();
            var total = 0.0; //global total

            Parallel.For(1, 100,

             localInit: () => 0.0, //Initialize localTotal

             body: (i, state, localTotal) => localTotal + Math.Sqrt(i),   //Body delegate

             localFinally: localTotal =>                   //Executed when the Body delegate has finished
              {
                  lock (goodLocker)
                  {
                      total += localTotal;    //Add the local value to the master value.
                  }
              }
            );

            Console.WriteLine(total);
        }
    }
}
