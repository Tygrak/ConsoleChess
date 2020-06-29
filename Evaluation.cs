using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleChess {
    public class Evaluation {
        private static int[] WhitePawnSquareTable = new int[64] {
             0,  0,  0,  0,  0,  0,  0,  0,
            20, 30, 20,  5,  5, 20, 30, 20,
             5, 10, 20, 25, 25, 20, 10,  5,
             5,  5, 10, 45, 45, 10,  5,  5,
            10, 10, 10, 15, 15, 10, 10, 10,
            15,  5, 10, 20, 20, 10, 15, 15,
            30, 25, 25, 25, 25, 25, 25, 30,
             0,  0,  0,  0,  0,  0,  0,  0
        };

        private static int[] BlackPawnSquareTable = new int[64] {
            0,  0,  0,  0,  0,  0,  0,  0,
           30, 25, 25, 25, 25, 25, 25, 30,
           15,  5, 10, 20, 20, 10, 15, 15,
           10, 10, 10, 15, 15, 10, 10, 10,
            5,  5, 10, 45, 45, 10,  5,  5,
            5, 10, 20, 25, 25, 20, 10,  5,
           20, 30, 20,  5,  5, 20, 30, 20,
            0,  0,  0,  0,  0,  0,  0,  0,
        };

        private static int[] KnightSquareTable = new int[64] {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -35, -3, 10, 15, 15, 10, -3,-35,
            -30, -3, 15, 20, 20, 15, -3,-30,
            -30, -3, 15, 20, 20, 15, -3,-30,
            -35, -3, 10, 15, 15, 10, -3,-35,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50,
        };

        private static int[] BishopSquareTable = new int[64] {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10,  0, 10, 12, 12, 10,  0,-10,
            -10,  0, 10, 12, 12, 10,  0,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
        };

        private static int[] BlackKingSquareTable = new int[64] {
            -35,-40,-45,-50,-50,-45,-40,-35,
            -35,-40,-45,-50,-50,-45,-40,-35,
            -35,-40,-45,-50,-50,-45,-40,-35,
            -30,-35,-40,-45,-45,-40,-35,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
              5,  5,  0,  0,  0,  0,  5,  5,
             20, 30,  5,  0,  0,  5, 30, 20,
        };

        private static int[] WhiteKingSquareTable = new int[64] {
             20, 30,  5,  0,  0,  5, 30, 20,
              5,  5,  0,  0,  0,  0,  5,  5,
            -10,-20,-20,-20,-20,-20,-20,-10,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -30,-35,-40,-45,-45,-40,-35,-30,
            -35,-40,-45,-50,-50,-45,-40,-35,
            -35,-40,-45,-50,-50,-45,-40,-35,
            -35,-40,-45,-50,-50,-45,-40,-35,
        };

        public float RateBoard(Board board) {
            float result = 0;
            UInt64 whitePieces = board.WhitePieces;
            UInt64 blackPieces = board.BlackPieces;
            int whitePiecesAmount = BitHelpers.GetNumberOfSetBits(whitePieces);
            int blackPiecesAmount = BitHelpers.GetNumberOfSetBits(blackPieces);
            foreach (var piece in board.PiecePositions) {
                float value = 1;
                (int x, int y) pos = Board.Position1DTo2D(piece.pos);
                if (PieceType.IsPawn(piece.type)) {
                    value = 1;
                    //value += 0.1f*(4.5f-Math.Abs(pos.x-4.5f));
                    if (PieceType.IsWhite(piece.type)) {
                        value += 0.01f*WhitePawnSquareTable[piece.pos];
                    } else {
                        value += 0.01f*BlackPawnSquareTable[piece.pos];
                    }
                } else if (PieceType.IsKnight(piece.type)) {
                    value = 3;
                    value += 0.01f*KnightSquareTable[piece.pos];
                    //value += pos.x > 1 && pos.x < 6 ? 0.25f : pos.x > 0 && pos.x < 7 ? 0.125f : 0;
                    //value += pos.y > 1 && pos.y < 6 ? 0.25f : pos.y > 0 && pos.y < 7 ? 0.125f : 0;
                } else if (PieceType.IsBishop(piece.type)) {
                    value = 3;
                    value += 0.01f*BishopSquareTable[piece.pos];
                    value += BitHelpers.GetNumberOfSetBits(board.GetMovesForBishop(PieceType.IsWhite(piece.type), piece.pos))*0.04f;
                } else if (PieceType.IsRook(piece.type)) {
                    value = 5;
                } else if (PieceType.IsQueen(piece.type)) {
                    value = 9;
                } else if (PieceType.IsKing(piece.type)) {
                    value = 0;
                    value += pos.x < 2 && pos.x > 5 ? 0.2f : pos.x < 3 && pos.x > 4 ? 0.075f : -0.1f;
                    if (PieceType.IsWhite(piece.type)) {
                        value += 0.015f*WhiteKingSquareTable[piece.pos];
                        value += 0.2f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingShield[piece.pos][1] & board.BitBoard[PieceType.WhitePawn]);
                        value -= 0.1f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingSphere[piece.pos] & blackPieces);
                        value = value*(1f+blackPiecesAmount)/16;
                    } else {
                        value += 0.015f*BlackKingSquareTable[piece.pos];
                        value += 0.2f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingShield[piece.pos][0] & board.BitBoard[PieceType.BlackPawn]);
                        value -= 0.1f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingSphere[piece.pos] & whitePieces);
                        value = value*(1f+whitePiecesAmount)/16;
                    }
                }
                result += PieceType.IsWhite(piece.type) ? value : -value;
            }
            for (int i = 0; i < 8; i++) {
                int whitePawnsOnFile = BitHelpers.GetNumberOfSetBits(RayHelpers.Files[i] & board.BitBoard[PieceType.WhitePawn]);
                int blackPawnsOnFile = BitHelpers.GetNumberOfSetBits(RayHelpers.Files[i] & board.BitBoard[PieceType.BlackPawn]);
                if (whitePawnsOnFile >= 3) {
                    result -= 1;
                } else if (whitePawnsOnFile >= 2) {
                    result -= 0.5f;
                }
                if (blackPawnsOnFile >= 3) {
                    result += 1;
                } else if (blackPawnsOnFile >= 2) {
                    result += 0.5f;
                }
            }
            return result;
        }
    }
}