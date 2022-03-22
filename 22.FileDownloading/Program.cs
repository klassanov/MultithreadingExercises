using System;
using System.IO;

namespace _22.FileDownloading
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(@"Files/Files.txt");

            var fileDownloader = new AsyncFileDownloader(lines);

            fileDownloader.Download();
        }
    }
}
