using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ConsoleChess {
    public static class BitHelpers {
        //mostly from https://github.com/bytefire/Shutranj/blob/master/Shutranj.Engine/BitHelper.cs

        private static int[] Index64 = new int[64] 
        {
                0, 47,  1, 56, 48, 27,  2, 60,
               57, 49, 41, 37, 28, 16,  3, 61,
               54, 58, 35, 52, 50, 42, 21, 44,
               38, 32, 29, 23, 17, 11,  4, 62,
               46, 55, 26, 59, 40, 36, 15, 53,
               34, 51, 20, 43, 31, 22, 10, 45,
               25, 39, 14, 33, 19, 30,  9, 24,
               13, 18,  8, 12,  7,  6,  5, 63
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetNumberOfSetBits(UInt64 bitBoard) {
            bitBoard = bitBoard - ((bitBoard >> 1) & 0x5555555555555555UL);
            bitBoard = (bitBoard & 0x3333333333333333UL) + ((bitBoard >> 2) & 0x3333333333333333UL);
            return (int)(unchecked(((bitBoard + (bitBoard >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 GetLeastSignificantBit(UInt64 val) {
            return val & ~(val - 1); ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 GetLeastSignificantBitMask(UInt64 val) {
            UInt64 leastSignificant1Bit = val & ~(val - 1);
            UInt64 mask = leastSignificant1Bit;
            for (int i = 1; i <= 63; i++)
            {
                mask = mask | (leastSignificant1Bit << i);
            }
            return mask;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 GetMostSignificantBitMask(UInt64 val) {
            val = val | (val >> 1);
            val = val | (val >> 2);
            val = val | (val >> 4);
            val = val | (val >> 8);
            val = val | (val >> 16);
            val = val | (val >> 32);
            return val;
        }

        //todo: make better
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 GetLeastSignificant1BitMask(UInt64 val) {
            UInt64 leastSignificant1Bit = val & ~(val - 1);
            UInt64 mask = leastSignificant1Bit;
            for (int i = 1; i <= 63; i++) {
                mask = mask | (leastSignificant1Bit << i);
            }
            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMostSignificant1BitIndex2(UInt64 val) {
            UInt64 debruijn64 = 0x03f79d71b4cb0a89;
            val |= val >> 1;
            val |= val >> 2;
            val |= val >> 4;
            val |= val >> 8;
            val |= val >> 16;
            val |= val >> 32;
            return Index64[(val * debruijn64) >> 58];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLeastSignificant1BitIndex2(UInt64 val) {
            // from: https://chessprogramming.wikispaces.com/BitScan#Bitscan%20forward-De%20Bruijn%20Multiplication-With%20separated%20LS1B
            UInt64 debruijn64 = 0x03f79d71b4cb0a89;
            return Index64[((val ^ (val - 1)) * debruijn64) >> 58];
        }

        /*public static int[] GetSetBitIndexes2(UInt64 bitboard) {
            // from: https://chessprogramming.wikispaces.com/Bitboard+Serialization#Converting%20Sets%20to%20Lists-Square%20Index%20Serialization-Scanning%20Forward
            List<int> indexes = new List<int>(64);
            while (bitboard != 0)
            {
                indexes.Add(GetLeastSignificant1BitIndex2(bitboard));
                bitboard &= bitboard - 1;
            }
            return indexes.ToArray();
        }*/
    }
}