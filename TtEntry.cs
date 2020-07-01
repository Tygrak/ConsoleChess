
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChess {
    public struct TtEntry {
        public UInt64 Zobrist;
        public UInt64 Data;

        //Data structure: 
        //      0-2 - entry type
        //      2-6 - best move piece type
        //      6-12 - best move from
        //      12-18 - best move to
        //      18-50 - score (float)
        //      50-56 - depth

        public TtEntry(UInt64 hash, UInt64 data) {
            Zobrist = hash;
            Data = data;
        }

        public bool IsValid() {
            return Data != 0;
        }

        public const int EntryTypeLowerBound = 0;
        public const int EntryTypeExact = 1;
        public const int EntryTypeHigherBound = 2;

        public const UInt64 Lowest2BitsMask = 0b11ul;
        public const UInt64 Lowest4BitsMask = 0b1111ul;
        public const UInt64 Lowest6BitsMask = 0b111111ul;
        public const UInt64 Lowest32BitsMask = 0xFFFFFFFFul;

        public int EntryType() {
            return (int) (Data & Lowest2BitsMask);
        }

        public int BestMovePieceType() {
            return (int) ((Data >> 2) & Lowest4BitsMask);
        }

        public int BestMoveFrom() {
            return (int) ((Data >> 6) & Lowest6BitsMask);
        }

        public int BestMoveTo() {
            return (int) ((Data >> 12) & Lowest6BitsMask);
        }

        public float Score() {
            return BitConverter.Int32BitsToSingle((int) ((Data >> 18) & Lowest32BitsMask));
        }

        public int Depth() {
            return (int) ((Data >> 50) & Lowest6BitsMask);
        }

        public static UInt64 CreateData(int entryType, int movePieceType, int moveFrom, int moveTo, float score, int depth) {
            UInt64 moveScore = (UInt64) (BitConverter.SingleToInt32Bits(score)) & Lowest32BitsMask;
            UInt64 result = ((UInt64) depth << 50) | (moveScore << 18) | ((UInt64) moveTo << 12) | 
                            ((UInt64) moveFrom << 6) | ((UInt64) movePieceType << 2) | ((UInt64) entryType & Lowest2BitsMask);
            return result;
        }
    }
}