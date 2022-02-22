using System;

namespace _12.ConsumerProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumerProducerQueue queue = new ConsumerProducerQueue();
            queue.EnqueueTask("Start");
            for (int i = 0; i < 10; i++)
            {
                queue.EnqueueTask("Say " + i);
            }
            queue.EnqueueTask("Finish");

            Console.ReadLine();
            queue.Dispose();
        }
    }
}
