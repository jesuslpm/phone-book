using PhoneBook;
using System;
using System.Linq;

namespace phone_book
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            string verb = args[1];

            using (var store = new NaiveLsmTreePhoneBookStore(path))
            {
                if (verb == "add")
                {
                    var entry = new PhoneBookEntry
                    {
                        Name = args[2],
                        PhoneNumber = args[3]
                    };
                    store.Add(entry);
                    Console.WriteLine("Phone book entry added");
                }
                else if (verb == "list")
                {
                    var skip = int.Parse(args[2]);
                    var limit = int.Parse(args[3]);
                    foreach (var entry in store.GetEntries().Skip(skip).Take(limit))
                    {
                        Console.WriteLine($"Name: {entry.Name}, Phone: {entry.PhoneNumber}");
                    }
                }
            }
        }
    }
}
