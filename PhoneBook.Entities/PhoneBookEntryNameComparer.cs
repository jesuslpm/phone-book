using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook
{
    class PhoneBookEntryNameComparer : IComparer<PhoneBookEntry>
    {
        public int Compare(PhoneBookEntry x, PhoneBookEntry y)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));
            if (y == null) throw new ArgumentNullException(nameof(y));
            return StringComparer.InvariantCulture.Compare(x.Name, y.Name);
        }
    }
}
