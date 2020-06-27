using System;
using System.Collections.Generic;

namespace ConsoleChess {
    public static class RayHelpers {
        public const int North = 0;
        public const int NorthEast = 1;
        public const int East = 2;
        public const int SouthEast = 3;
        public const int South = 4;
        public const int SouthWest = 5;
        public const int West = 6;
        public const int NorthWest = 7;

        public readonly static byte[][][] PositionRays;
        public readonly static UInt64[] Ranks;
        public readonly static UInt64[] Files;
        public readonly static UInt64[] KnightMoves;
        public readonly static UInt64[] KingMoves;

        static RayHelpers() {
            PositionRays = CalculateAllPositionRays();
            Ranks = CalculateRanks();
            Files = CalculateFiles();
            KnightMoves = CalculateKnightMoves();
            KingMoves = CalculateKingMoves();
        }

        private static UInt64[] CalculateRanks() {
            UInt64[] ranks = new UInt64[8];
            ranks[0] = 255 << 0;
            ranks[1] = 255 << 1;
            ranks[2] = 255 << 2;
            ranks[3] = 255 << 3;
            ranks[4] = 255 << 4;
            ranks[5] = 255 << 5;
            ranks[6] = 255 << 6;
            ranks[7] = 255 << 7;
            return ranks;
        }

        private static UInt64[] CalculateFiles() {
            UInt64[] files = new UInt64[8];
            files[0] = 1|(1 << 8)|(1 << 16)|(1 << 24)|(1 << 32)|(1 << 40)|(1 << 48)|(1 << 56);
            files[1] = files[0] << 1;
            files[2] = files[0] << 2;
            files[3] = files[0] << 3;
            files[4] = files[0] << 4;
            files[5] = files[0] << 5;
            files[6] = files[0] << 6;
            files[7] = files[0] << 7;
            return files;
        }

        private static UInt64[] CalculateKnightMoves() {
            UInt64[] knightMoves = new UInt64[64];
            for (int i = 0; i < 64; i++) {
                knightMoves[i] = CalculateKnightMove(i);
            }
            return knightMoves;
        }

        private static UInt64 CalculateKnightMove(int pos) {
            UInt64 knightPosition = 0;
            int x = pos%8;
            if (x+1 < 8 && pos+17 < 64) {
                knightPosition |= 0b1ul << (pos+17);
            }
            if (x+2 < 8 && pos+10 < 64) {
                knightPosition |= 0b1ul << (pos+10);
            }
            if (x+2 < 8 && pos-6 >= 0) {
                knightPosition |= 0b1ul << (pos-6);
            }
            if (x+1 < 8 && pos-15 >= 0) {
                knightPosition |= 0b1ul << (pos-15);
            }
            if (x-1 >= 0 && pos-17 >= 0) {
                knightPosition |= 0b1ul << (pos-17);
            }
            if (x-2 >= 0 && pos-10 >= 0) {
                knightPosition |= 0b1ul << (pos-10);
            }
            if (x-2 >= 0 && pos+6 < 64) {
                knightPosition |= 0b1ul << (pos+6);
            }
            if (x-1 >= 0 && pos+15 < 64) {
                knightPosition |= 0b1ul << (pos+15);
            }
            return knightPosition;
        }

        private static UInt64[] CalculateKingMoves() {
            UInt64[] kingMoves = new UInt64[64];
            for (int i = 0; i < 64; i++) {
                kingMoves[i] = CalculateKingMove(i);
            }
            return kingMoves;
        }

        private static UInt64 CalculateKingMove(int pos) {
            UInt64 kingPosition = 0;
            int x = pos%8;
            if (pos+8 < 64) {
                kingPosition |= 0b1ul << (pos+8);
            }
            if (x+1 < 8 && pos+9 < 64) {
                kingPosition |= 0b1ul << (pos+9);
            }
            if (x+1 < 8) {
                kingPosition |= 0b1ul << (pos+1);
            }
            if (x+1 < 8 && pos-7 >= 0) {
                kingPosition |= 0b1ul << (pos-7);
            }
            if (pos-8 >= 0) {
                kingPosition |= 0b1ul << (pos-8);
            }
            if (x-1 >= 0 && pos-9 >= 0) {
                kingPosition |= 0b1ul << (pos-9);
            }
            if (x-1 >= 0) {
                kingPosition |= 0b1ul << (pos-1);
            }
            if (x-1 >= 0 && pos+7 < 64) {
                kingPosition |= 0b1ul << (pos+7);
            }
            return kingPosition;
        }

        private static byte[][][] CalculateAllPositionRays() {
            byte[][][] result = new byte[64][][];
            for (int i = 0; i < 64; i++) {
                result[i] = new byte[8][];
                result[i][North] = CalculateNorthRay((byte) i);
                result[i][NorthEast] = CalculateNorthEastRay((byte) i);
                result[i][East] = CalculateEastRay((byte) i);
                result[i][SouthEast] = CalculateSouthEastRay((byte) i);
                result[i][South] = CalculateSouthRay((byte) i);
                result[i][SouthWest] = CalculateSouthWestRay((byte) i);
                result[i][West] = CalculateWestRay((byte) i);
                result[i][NorthWest] = CalculateNorthWestRay((byte) i);
            }
            return result;
        }

        private static byte[] CalculateNorthRay(byte pos) {
            int currPos = pos;
            List<byte> positions = new List<byte>();
            currPos += 8;
            while (currPos < 64) {
                positions.Add((byte) currPos);
                currPos += 8;
            }
            return positions.ToArray();
        }

        private static byte[] CalculateNorthEastRay(byte pos) {
            int currPos = pos;
            int currX = pos%8;
            List<byte> positions = new List<byte>();
            currPos += 8;
            currX++;
            while (currPos < 64 && currX < 8) {
                positions.Add((byte) currPos);
                currPos += 9;
                currX++;
            }
            return positions.ToArray();
        }

        private static byte[] CalculateEastRay(byte pos) {
            int currPos = pos;
            int currX = pos%8;
            List<byte> positions = new List<byte>();
            currPos += 1;
            currX++;
            while (currX < 8) {
                positions.Add((byte) currPos);
                currPos += 1;
                currX++;
            }
            return positions.ToArray();
        }

        private static byte[] CalculateSouthEastRay(byte pos) {
            int currPos = pos;
            int currX = pos%8;
            List<byte> positions = new List<byte>();
            currPos -= 7;
            currX++;
            while (currPos >= 0 && currX < 8) {
                positions.Add((byte) currPos);
                currPos -= 7;
                currX++;
            }
            return positions.ToArray();
        }

        private static byte[] CalculateSouthRay(byte pos) {
            int currPos = pos;
            List<byte> positions = new List<byte>();
            currPos -= 8;
            while (currPos >= 0) {
                positions.Add((byte) currPos);
                currPos -= 8;
            }
            return positions.ToArray();
        }

        private static byte[] CalculateSouthWestRay(byte pos) {
            int currPos = pos;
            int currX = pos%8;
            List<byte> positions = new List<byte>();
            currPos -= 9;
            currX--;
            while (currPos >= 0 && currX >= 0) {
                positions.Add((byte) currPos);
                currPos -= 9;
                currX--;
            }
            return positions.ToArray();
        }

        private static byte[] CalculateWestRay(byte pos) {
            int currPos = pos;
            int currX = pos%8;
            List<byte> positions = new List<byte>();
            currPos -= 1;
            currX--;
            while (currX >= 8) {
                positions.Add((byte) currPos);
                currPos -= 1;
                currX--;
            }
            return positions.ToArray();
        }

        private static byte[] CalculateNorthWestRay(byte pos) {
            int currPos = pos;
            int currX = pos%8;
            List<byte> positions = new List<byte>();
            currPos += 7;
            currX--;
            while (currPos < 64 && currX >= 0) {
                positions.Add((byte) currPos);
                currPos += 7;
                currX--;
            }
            return positions.ToArray();
        }
    }
}