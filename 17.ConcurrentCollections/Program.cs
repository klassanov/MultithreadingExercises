using System;
using System.Collections.Concurrent;
using System.Threading;

namespace _17.ConcurrentCollections
{
    class Program
    {
        private static readonly Random Random = new Random();
        private static readonly CountdownEvent Countdown = new CountdownEvent(100);
        private static readonly ConcurrentBag<int> Bag = new ConcurrentBag<int>();
        private static readonly ConcurrentDictionary<string, int> Dictionary = new ConcurrentDictionary<string, int>();


        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                int current = i;
                Thread t = new Thread(() => Work("Key", current));
                t.Start();
            }

            Countdown.Wait();
            Console.WriteLine(Bag.Count); //Successfully removed
        }

        private static void Work(string key, int value)
        {
            Thread.Sleep(Random.Next(100, 2000));

            var result = Dictionary.GetOrAdd(key, _ => value);

            Thread.Sleep(Random.Next(100, 2000));

            if (Dictionary.TryRemove(key, out var newValue))
            {
                if (Dictionary.ContainsKey(key))
                {
                    Console.WriteLine("Still inside!");
                }

                Bag.Add(newValue);
            }

            Countdown.Signal();
        }
    }
}
