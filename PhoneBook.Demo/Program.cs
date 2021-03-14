using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PhoneBook.Test
{
    class Program
    {
        const int EntryCount = 10_000_000;

        static Random rnd = new Random();

        static string NextRnd()
        {
            return rnd.Next(10_000_000, 90_000_000).ToString();
        }

        static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            using (var store = new NaiveLsmTreePhoneBookStore("PhoneBook.txt", 64))
            {
                var rnd = new Random();
                for (int i = 0; i < EntryCount; i++)
                {
                    var entry = new PhoneBookEntry
                    {
                        Name = NextRnd() + NextRnd() + " " + NextRnd() + NextRnd(),
                        PhoneNumber = NextRnd()
                    };
                    store.Add(entry);
                }
                Console.WriteLine($"Elapsed time to insert {EntryCount} entries: {watch.Elapsed}");
                watch.Restart();

                using (var writer = new StreamWriter("PhoneBookCopy.txt", false, Encoding.UTF8))
                {
                    foreach (var entry in store.GetEntries())
                    {
                        writer.WriteLine(entry.Name);
                        writer.WriteLine(entry.PhoneNumber);
                    }
                }

                Console.WriteLine($"Elapsed time to copy the phone book: {watch.Elapsed}");
            }
        }
    }
}
