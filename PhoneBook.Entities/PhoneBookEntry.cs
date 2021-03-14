using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook
{
    public class PhoneBookEntry
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public static readonly IComparer<PhoneBookEntry> NameComparer = new PhoneBookEntryNameComparer();
    }
}
