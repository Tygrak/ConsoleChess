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

        //public readonly static int[][][] OrderedPositionRays;
        public readonly static UInt64[][] PositionRays;
        public readonly static UInt64[] Ranks;
        public readonly static UInt64[] Files;
        public readonly static UInt64[][] PawnMoves;
        public readonly static UInt64[][] PawnDoubleMoves;
        public readonly static UInt64[][] PawnAttacks;
        public readonly static UInt64[] KnightMoves;
        public readonly static UInt64[] KingMoves;
        public readonly static UInt64[][] KingShield;
        public readonly static UInt64[] KingSphere;

        static RayHelpers() {
            PositionRays = CalculateAllPositionRays();
            Ranks = CalculateRanks();
            Files = CalculateFiles();
            PawnMoves = CalculatePawnMoves();
            PawnDoubleMoves = CalculatePawnDoubleMoves();
            PawnAttacks = CalculatePawnAttacks();
            KnightMoves = CalculateKnightMoves();
            KingMoves = CalculateKingMoves();
            KingShield = CalculateKingShield();
            KingSphere = CalculateKingSphere();
        }

        private static UInt64[] CalculateRanks() {
            UInt64[] ranks = new UInt64[8];
            ranks[0] = 255ul << 0;
            ranks[1] = 255ul << 8;
            ranks[2] = 255ul << 16;
            ranks[3] = 255ul << 24;
            ranks[4] = 255ul << 32;
            ranks[5] = 255ul << 40;
            ranks[6] = 255ul << 48;
            ranks[7] = 255ul << 56;
            return ranks;
        }

        private static UInt64[] CalculateFiles() {
            UInt64[] files = new UInt64[8];
            files[0] = 1ul|(1ul << 8)|(1ul << 16)|(1ul << 24)|(1ul << 32)|(1ul << 40)|(1ul << 48)|(1ul << 56);
            files[1] = files[0] << 1;
            files[2] = files[0] << 2;
            files[3] = files[0] << 3;
            files[4] = files[0] << 4;
            files[5] = files[0] << 5;
            files[6] = files[0] << 6;
            files[7] = files[0] << 7;
            return files;
        }

        private static UInt64[][] CalculatePawnMoves() {
            UInt64[][] pawnMoves = new UInt64[64][];
            for (int i = 0; i < 64; i++) {
                pawnMoves[i] = new UInt64[2];
                pawnMoves[i][0] = 1ul << (i-8);
                pawnMoves[i][1] = 1ul << (i+8);
            }
            return pawnMoves;
        }

        private static UInt64[][] CalculatePawnAttacks() {
            UInt64[][] pawnAttacks = new UInt64[64][];
            for (int i = 0; i < 64; i++) {
                int y = i/8;
                pawnAttacks[i] = new UInt64[2];
                if (y < 7) {
                    pawnAttacks[i][1] = ((1ul << (i+7)) | (1ul << (i+9))) & Ranks[y+1];
                } else {
                    pawnAttacks[i][1] = 0;
                }
                if (y > 0) {
                    pawnAttacks[i][0] = ((1ul << (i-7)) | (1ul << (i-9))) & Ranks[y-1];
                } else {
                    pawnAttacks[i][0] = 0;
                }
            }
            return pawnAttacks;
        }

        private static UInt64[][] CalculatePawnDoubleMoves() {
            UInt64[][] pawnMoves = new UInt64[64][];
            for (int i = 0; i < 64; i++) {
                int y = i/8;
                pawnMoves[i] = new UInt64[2];
                if (y == 1) {
                    pawnMoves[i][0] = 0;
                    pawnMoves[i][1] = 1ul << (i+16);
                } else if (y == 6) {
                    pawnMoves[i][0] = 1ul << (i-16);
                    pawnMoves[i][1] = 0;
                } else {
                    pawnMoves[i][0] = 0;
                    pawnMoves[i][1] = 0;
                }
            }
            return pawnMoves;
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

        private static UInt64[][] CalculateKingShield() {
            UInt64[][] result = new UInt64[64][];
            for (int pos = 0; pos < 64; pos++) {
                int y = pos/8;
                result[pos] = new UInt64[2];
                for (int j = 0; j < 2; j++) {
                    if (j == 0 && y > 0) {
                        result[pos][j] = ((0b1ul << (pos-7)) | (0b1ul << (pos-8)) | (0b1ul << (pos-9))) & Ranks[y-1];
                    } else if (j == 1 && y < 7) {
                        result[pos][j] = ((0b1ul << (pos+7)) | (0b1ul << (pos+8)) | (0b1ul << (pos+9))) & Ranks[y+1];
                    } else {
                        result[pos][j] = 0;
                    }
                }
            }
            return result;
        }

        private static UInt64[] CalculateKingSphere() {
            UInt64[] result = new UInt64[64];
            for (int pos = 0; pos < 64; pos++) {
                int kingX = pos%8;
                int kingY = pos/8;
                result[pos] = 0;
                for (int x = kingX-2; x <= kingX+2; x++) {
                    for (int y = kingY-2; y <= kingY+2; y++) {
                        if (x < 0 || x > 7 || y < 0 || y > 7) {
                            continue;
                        }
                        result[pos] |= 1ul << (x+y*8);
                    }
                }
            }
            return result;
        }

        private static UInt64[][] CalculateAllPositionRays() {
            UInt64[][] result = new UInt64[64][];
            for (int i = 0; i < 64; i++) {
                result[i] = new UInt64[8];
                result[i][North] = CalculateNorthRay((int) i);
                result[i][NorthEast] = CalculateNorthEastRay((int) i);
                result[i][East] = CalculateEastRay((int) i);
                result[i][SouthEast] = CalculateSouthEastRay((int) i);
                result[i][South] = CalculateSouthRay((int) i);
                result[i][SouthWest] = CalculateSouthWestRay((int) i);
                result[i][West] = CalculateWestRay((int) i);
                result[i][NorthWest] = CalculateNorthWestRay((int) i);
            }
            return result;
        }

        private static UInt64 CalculateNorthRay(int pos) {
            int currPos = pos;
            UInt64 result = 0;
            currPos += 8;
            while (currPos < 64) {
                result |= 1ul << currPos;
                currPos += 8;
            }
            return result;
        }

        private static UInt64 CalculateNorthEastRay(int pos) {
            int currPos = pos;
            int currX = pos%8;
            UInt64 result = 0;
            currPos += 9;
            currX++;
            while (currPos < 64 && currX < 8) {
                result |= 1ul << currPos;
                currPos += 9;
                currX++;
            }
            return result;
        }

        private static UInt64 CalculateEastRay(int pos) {
            int currPos = pos;
            int currX = pos%8;
            UInt64 result = 0;
            currPos += 1;
            currX++;
            while (currX < 8) {
                result |= 1ul << currPos;
                currPos += 1;
                currX++;
            }
            return result;
        }

        private static UInt64 CalculateSouthEastRay(int pos) {
            int currPos = pos;
            int currX = pos%8;
            UInt64 result = 0;
            currPos -= 7;
            currX++;
            while (currPos >= 0 && currX < 8) {
                result |= 1ul << currPos;
                currPos -= 7;
                currX++;
            }
            return result;
        }

        private static UInt64 CalculateSouthRay(int pos) {
            int currPos = pos;
            UInt64 result = 0;
            currPos -= 8;
            while (currPos >= 0) {
                result |= 1ul << currPos;
                currPos -= 8;
            }
            return result;
        }

        private static UInt64 CalculateSouthWestRay(int pos) {
            int currPos = pos;
            int currX = pos%8;
            UInt64 result = 0;
            currPos -= 9;
            currX--;
            while (currPos >= 0 && currX >= 0) {
                result |= 1ul << currPos;
                currPos -= 9;
                currX--;
            }
            return result;
        }

        private static UInt64 CalculateWestRay(int pos) {
            int currPos = pos;
            int currX = pos%8;
            UInt64 result = 0;
            currPos -= 1;
            currX--;
            while (currX >= 0) {
                result |= 1ul << currPos;
                currPos -= 1;
                currX--;
            }
            return result;
        }

        private static UInt64 CalculateNorthWestRay(int pos) {
            int currPos = pos;
            int currX = pos%8;
            UInt64 result = 0;
            currPos += 7;
            currX--;
            while (currPos < 64 && currX >= 0) {
                result |= 1ul << currPos;
                currPos += 7;
                currX--;
            }
            return result;
        }

        public static void DrawBitBoard(UInt64 bitBoard) {
            for (int y = 7; y >= 0; y--) {
                if (y < 7) {
                    Console.Write('\n');
                }
                Console.Write((char) ('1'+y));
                Console.Write(' ');
                for (UInt64 x = 0; x < 8; x++) {
                    UInt64 pos = 0b1ul << ((int) x+y*8);
                    if ((bitBoard & pos) != 0) {
                        Console.Write('X');
                    } else {
                        Console.Write('.');
                    }
                }
                Console.Write(' ');
                Console.Write((char) ('1'+y));
            }
            Console.WriteLine();
            Console.Write("  ");
            for (int x = 0; x < 8; x++) {
                Console.Write((char) ('a'+x));
            }
            Console.WriteLine();
        }
    }
}