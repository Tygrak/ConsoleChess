namespace ConsoleChess {
    public static class PieceType {
        public const byte WhitePawn = 0;
        public const byte BlackPawn = 1;
        public const byte WhiteBishop = 2;
        public const byte BlackBishop = 3;
        public const byte WhiteKnight = 4;
        public const byte BlackKnight = 5;
        public const byte WhiteRook = 6;
        public const byte BlackRook = 7;
        public const byte WhiteQueen = 8;
        public const byte BlackQueen = 9;
        public const byte WhiteKing = 10;
        public const byte BlackKing = 11;

        public static bool IsWhite(byte type) {
            return type % 2 == 0;
        }

        public static bool IsPawn(byte type) {
            return type == WhitePawn || type == BlackPawn;
        }

        public static bool IsKnight(byte type) {
            return type == WhiteKnight || type == BlackKnight;
        }

        public static bool IsBishop(byte type) {
            return type == WhiteBishop || type == BlackBishop;
        }

        public static bool IsRook(byte type) {
            return type == WhiteRook || type == BlackRook;
        }

        public static bool IsQueen(byte type) {
            return type == WhiteQueen || type == BlackQueen;
        }

        public static bool IsKing(byte type) {
            return type == WhiteKing || type == BlackKing;
        }

        public static byte PawnOfColor(bool white) {
            return white ? WhitePawn : BlackPawn;
        }

        public static byte KnightOfColor(bool white) {
            return white ? WhiteKnight : BlackKnight;
        }

        public static byte BishopOfColor(bool white) {
            return white ? WhiteBishop : BlackBishop;
        }

        public static byte RookOfColor(bool white) {
            return white ? WhiteRook : BlackRook;
        }

        public static byte QueenOfColor(bool white) {
            return white ? WhiteQueen : BlackQueen;
        }

        public static byte KingOfColor(bool white) {
            return white ? WhiteKing : BlackKing;
        }
    }
}
