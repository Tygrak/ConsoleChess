namespace ConsoleChess {
    public static class PieceType {
        public const int Empty = 15;
        public const int WhitePawn = 0;
        public const int BlackPawn = 1;
        public const int WhiteBishop = 2;
        public const int BlackBishop = 3;
        public const int WhiteKnight = 4;
        public const int BlackKnight = 5;
        public const int WhiteRook = 6;
        public const int BlackRook = 7;
        public const int WhiteQueen = 8;
        public const int BlackQueen = 9;
        public const int WhiteKing = 10;
        public const int BlackKing = 11;

        public static bool IsWhite(int type) {
            return type % 2 == 0;
        }

        public static bool IsPawn(int type) {
            return type == WhitePawn || type == BlackPawn;
        }

        public static bool IsKnight(int type) {
            return type == WhiteKnight || type == BlackKnight;
        }

        public static bool IsBishop(int type) {
            return type == WhiteBishop || type == BlackBishop;
        }

        public static bool IsRook(int type) {
            return type == WhiteRook || type == BlackRook;
        }

        public static bool IsQueen(int type) {
            return type == WhiteQueen || type == BlackQueen;
        }

        public static bool IsKing(int type) {
            return type == WhiteKing || type == BlackKing;
        }

        public static int PawnOfColor(bool white) {
            return white ? WhitePawn : BlackPawn;
        }

        public static int KnightOfColor(bool white) {
            return white ? WhiteKnight : BlackKnight;
        }

        public static int BishopOfColor(bool white) {
            return white ? WhiteBishop : BlackBishop;
        }

        public static int RookOfColor(bool white) {
            return white ? WhiteRook : BlackRook;
        }

        public static int QueenOfColor(bool white) {
            return white ? WhiteQueen : BlackQueen;
        }

        public static int KingOfColor(bool white) {
            return white ? WhiteKing : BlackKing;
        }

        public static int GetPieceZobristStartingIndex(int pieceType) {
            if (pieceType == WhitePawn) {
                return BitHelpers.ZobristWhitePawnStartingIndex;
            } else if (pieceType == WhiteKnight) {
                return BitHelpers.ZobristWhiteKnightStartingIndex;
            } else if (pieceType == WhiteBishop) {
                return BitHelpers.ZobristWhiteBishopStartingIndex;
            } else if (pieceType == WhiteRook) {
                return BitHelpers.ZobristWhiteRookStartingIndex;
            } else if (pieceType == WhiteQueen) {
                return BitHelpers.ZobristWhiteQueenStartingIndex;
            } else if (pieceType == WhiteKing) {
                return BitHelpers.ZobristWhiteKingStartingIndex;
            } else if (pieceType == BlackPawn) {
                return BitHelpers.ZobristBlackPawnStartingIndex;
            } else if (pieceType == BlackKnight) {
                return BitHelpers.ZobristBlackKnightStartingIndex;
            } else if (pieceType == BlackBishop) {
                return BitHelpers.ZobristBlackBishopStartingIndex;
            } else if (pieceType == BlackRook) {
                return BitHelpers.ZobristBlackRookStartingIndex;
            } else if (pieceType == BlackQueen) {
                return BitHelpers.ZobristBlackQueenStartingIndex;
            } else {
                return BitHelpers.ZobristBlackKingStartingIndex;
            }
        }
    }
}
