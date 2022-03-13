using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _16.PLINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            //Generate some speach, taking words from dictionary
            var random = new Random();
            var dictionaryWords = new HashSet<string>(File.ReadAllLines("WordLookup.txt"), StringComparer.OrdinalIgnoreCase);
            var speach = Enumerable.Range(0, 10000).Select(i => dictionaryWords.ElementAt(random.Next(0, dictionaryWords.Count - 1))).ToArray();

            //Introduce some incorrect words in the speach
            speach[59] = "incorrect word 1";
            speach[60] = "incorrect word 2";

            //Detect the incorrect words using PLINQ => benefit for very big collections
            //I am interested in the speach words' index, so first, I do the select and after that I filter
            //If I am not interested in the index, I can first filter
            var incorrectWords = speach.AsParallel()
                                       .Select((word, index) => new { Text = word, Index = index })
                                       .Where(word => !dictionaryWords.Contains(word.Text))
                                       .ToList();

            foreach (var incorrectWord in incorrectWords)
            {
                Console.WriteLine($"{incorrectWord.Index}:{incorrectWord.Text}");
            }

        }
    }
}
