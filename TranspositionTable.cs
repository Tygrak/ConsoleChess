
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChess {
    public class TranspositionTable {
        private readonly TtEntry InvalidEntry = new TtEntry(0, 0);

        private const int TableSize = 18000000;

        private TtEntry[] entries = new TtEntry[TableSize];

        public void Add(TtEntry entry) {
            int index = (int)(entry.Zobrist % TableSize);
            //entry.Zobrist ^= entry.Data;
            entries[index] = entry;
        }

        public TtEntry Get(UInt64 hash) {
            int index = (int)(hash % TableSize);
            TtEntry entry = entries[index];
            if (entry.Zobrist == hash) {
                return entry;
            }
            return InvalidEntry;
        }
    }
}