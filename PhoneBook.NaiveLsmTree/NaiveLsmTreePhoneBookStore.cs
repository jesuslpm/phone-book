using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneBook
{
    public class NaiveLsmTreePhoneBookStore : IDisposable
    {
        private readonly string bookStoreFilePath;
        private readonly string bookStoreLogFilePath;
        private int maxLogFileSize;
        private StreamWriter logWriter;

        public NaiveLsmTreePhoneBookStore(string bookStoreFilePath, int maxLogFileSizeInMi = 64)
        {
            this.bookStoreFilePath = bookStoreFilePath;
            this.bookStoreLogFilePath = this.bookStoreFilePath + ".log";
            this.maxLogFileSize = maxLogFileSizeInMi * 1024 * 1024;
        }

        private void EnsureLogWriter()
        {
            if (logWriter == null)
            {
                logWriter = new StreamWriter(this.bookStoreLogFilePath, true, Encoding.UTF8);
            }
        }

        public void Add(PhoneBookEntry entry)
        {
            EnsureLogWriter();
            logWriter.WriteLine(entry.Name);
            logWriter.WriteLine(entry.PhoneNumber);
            if (logWriter.BaseStream.Length > maxLogFileSize) Merge();
        }

        private void Merge()
        {


            var tempSstFilePath = this.bookStoreFilePath + ".temp";
            using (var writer = new StreamWriter(tempSstFilePath, false, Encoding.UTF8))
            {
                foreach (var entry in GetEntries())
                {
                    writer.WriteLine(entry.Name);
                    writer.WriteLine(entry.PhoneNumber);
                }
            }
            if (File.Exists(this.bookStoreFilePath))
            {
                File.Replace(tempSstFilePath, bookStoreFilePath, null);
            }
            else 
            {
                File.Move(tempSstFilePath, bookStoreFilePath);
            }
            File.Delete(bookStoreLogFilePath);
        }

        private IEnumerable<PhoneBookEntry> GetEntriesFromLog()
        {
            if (File.Exists(this.bookStoreLogFilePath) == false) return Enumerable.Empty<PhoneBookEntry>();
            if (logWriter != null) logWriter.Dispose();
            logWriter = null;
            var entries = new List<PhoneBookEntry>();
            using (var reader = new StreamReader(this.bookStoreLogFilePath, Encoding.UTF8))
            {
                while (reader.EndOfStream == false)
                {
                    var entry = new PhoneBookEntry
                    {
                        Name = reader.ReadLine(),
                        PhoneNumber = reader.ReadLine()
                    };
                    if (entry.Name != null) entries.Add(entry);
                }
            }
            entries.Sort(PhoneBookEntry.NameComparer);
            return entries;
        }

        private IEnumerable<PhoneBookEntry> GetEntriesFromSst()
        {
            if (File.Exists(this.bookStoreFilePath) == false) yield break;
            using (var reader = new StreamReader(this.bookStoreFilePath, Encoding.UTF8))
            {
                while (reader.EndOfStream == false)
                {
                    var entry = new PhoneBookEntry
                    {
                        Name = reader.ReadLine(),
                        PhoneNumber = reader.ReadLine()
                    };
                    if (entry.Name != null) yield return entry;
                }
            }
        }

        public IEnumerable<PhoneBookEntry> GetEntries()
        {
            var sstEnumerator = GetEntriesFromSst().GetEnumerator();
            var logEnumerator = GetEntriesFromLog().GetEnumerator();
            var sstEntry = sstEnumerator.MoveNext() ? sstEnumerator.Current : null;
            var logEntry = logEnumerator.MoveNext() ? logEnumerator.Current : null;
            var comparer = PhoneBookEntry.NameComparer;
            while(sstEntry != null || logEntry != null)
            {
                if (logEntry == null || sstEntry != null && comparer.Compare(sstEntry, logEntry) < 0)
                {
                    yield return sstEntry;
                    sstEntry = sstEnumerator.MoveNext() ? sstEnumerator.Current : null;
                }
                else
                {
                    yield return logEntry;
                    logEntry = logEnumerator.MoveNext() ? logEnumerator.Current : null;
                }
            }
        }

        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (this.logWriter != null) logWriter.Dispose();
            logWriter = null;
        }
    }
}
