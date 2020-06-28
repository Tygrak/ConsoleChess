using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleChess {
    public class Board {
        internal UInt64[] BitBoard = new UInt64[12];
        internal UInt16 CastlingRights = 0b1111;
        internal byte EnPassant = 255;
        internal int HalfMovesSincePawnOrCapture = 0;
        internal bool WhiteTurn = true;
        internal List<(byte type, byte pos)> PiecePositions = new List<(byte type, byte pos)>(); 

        internal UInt64 WhitePieces {
            get {
                return BitBoard[PieceType.WhitePawn] | BitBoard[PieceType.WhiteKnight] | BitBoard[PieceType.WhiteBishop] |
                       BitBoard[PieceType.WhiteRook] | BitBoard[PieceType.WhiteQueen] | BitBoard[PieceType.WhiteKing];
            }
        }

        internal UInt64 BlackPieces {
            get {
                return BitBoard[PieceType.BlackPawn] | BitBoard[PieceType.BlackKnight] | BitBoard[PieceType.BlackBishop] |
                       BitBoard[PieceType.BlackRook] | BitBoard[PieceType.BlackQueen] | BitBoard[PieceType.BlackKing];
            }
        }

        internal UInt64 AllPieces {
            get {
                return WhitePieces | BlackPieces;
            }
        }

        public Board() {
        }

        public Board(Board board) {
            BitBoard = (UInt64[]) board.BitBoard.Clone();
            CastlingRights = board.CastlingRights;
            EnPassant = board.EnPassant;
            HalfMovesSincePawnOrCapture = board.HalfMovesSincePawnOrCapture;
            WhiteTurn = board.WhiteTurn;
            PiecePositions = new List<(byte type, byte pos)>(board.PiecePositions);
        }

        public void MakeMove(byte type, byte from, byte to) {
            HalfMovesSincePawnOrCapture++;
            UInt64 fromPos = 1ul << from;
            UInt64 toPos = 1ul << to;
            for (int i = 0; i < 12; i++) {
                BitBoard[i] = BitBoard[i] & (~toPos);
            }
            BitBoard[type] = (fromPos | toPos) ^ BitBoard[type];
            for (int i = PiecePositions.Count-1; i >= 0; i--) {
                if (PiecePositions[i].pos == from) {
                    PiecePositions[i] = (PiecePositions[i].type, to);
                }
                if (PiecePositions[i].pos == to && PieceType.IsWhite(PiecePositions[i].type) != PieceType.IsWhite(type)) {
                    PiecePositions.RemoveAt(i);
                    HalfMovesSincePawnOrCapture = 0;
                }
            }
            if (PieceType.IsPawn(type)) {
                int yPos = to/8;
                HalfMovesSincePawnOrCapture = 0;
                if (PieceType.IsWhite(type)) {
                    if (to-from == 16) {
                        EnPassant = (byte) (from+8);
                    } else if (to == EnPassant) {
                        ClearPieceFromSquare(PieceType.BlackPawn, (byte) (to+8));
                    }
                    if (yPos == 7) {
                        BitBoard[type] &= (~toPos);
                        BitBoard[PieceType.WhiteQueen] |= toPos;
                        int index = PiecePositions.FindIndex(p => p.pos == to && p.type == type);
                        PiecePositions[index] = (PieceType.WhiteQueen, to);
                    }
                } else if (!PieceType.IsWhite(type)) {
                    if (from-to == 16) {
                        EnPassant = (byte) (from-8);
                    } else if (to == EnPassant) {
                        ClearPieceFromSquare(PieceType.BlackPawn, (byte) (to-8));
                    }
                    if (yPos == 0) {
                        BitBoard[type] &= (~toPos);
                        BitBoard[PieceType.BlackQueen] |= toPos;
                        int index = PiecePositions.FindIndex(p => p.pos == to && p.type == type);
                        PiecePositions[index] = (PieceType.BlackQueen, to);
                    }
                }
            } else {
                EnPassant = 255;
            }
            if (PieceType.IsKing(type)) {
                if (PieceType.IsWhite(type)) {
                    CastlingRights &= 0b0011;
                } else if (!PieceType.IsWhite(type)) {
                    CastlingRights &= 0b1100;
                }
                if (to-from == 2) {
                    MakeMove(PieceType.RookOfColor(PieceType.IsWhite(type)), (byte) (to+1), (byte) (to-1));
                    WhiteTurn = !WhiteTurn;
                } else if (from-to == 2) {
                    MakeMove(PieceType.RookOfColor(PieceType.IsWhite(type)), (byte) (to-2), (byte) (to+1));
                    WhiteTurn = !WhiteTurn;
                }
            }
            if (PieceType.IsRook(type)) {
                if (PieceType.IsWhite(type) && from%8 == 0) {
                    CastlingRights &= 0b0111;
                } else if (PieceType.IsWhite(type) && from%8 == 7) {
                    CastlingRights &= 0b1011;
                } else if (!PieceType.IsWhite(type) && from%8 == 0) {
                    CastlingRights &= 0b1101;
                } else if (!PieceType.IsWhite(type) && from%8 == 7) {
                    CastlingRights &= 0b1110;
                }
            }
            WhiteTurn = !WhiteTurn;
        }

        public void ClearPieceFromSquare(byte type, byte position) {
            UInt64 boardPos = 1ul << position;
            BitBoard[type] = BitBoard[type] & (~boardPos);
            for (int i = PiecePositions.Count-1; i >= 0; i--) {
                if (PiecePositions[i].pos == position && PiecePositions[i].type == type) {
                    PiecePositions.RemoveAt(i);
                    break;
                }
            }
        }

        public UInt64 SquareAttackers(bool white, byte pos) {
            int yPos = pos/8;
            UInt64 boardPos = 1ul << pos;
            UInt64 pawnMoves = 0;
            if (white && yPos != 7) {
                pawnMoves = ((boardPos << 7) | (boardPos << 9)) & RayHelpers.Ranks[yPos+1] & BitBoard[PieceType.BlackPawn];
            } else if (!white && yPos != 0) {
                pawnMoves = ((boardPos >> 7) | (boardPos >> 9)) & RayHelpers.Ranks[yPos-1] & BitBoard[PieceType.WhitePawn];
            }
            UInt64 knightMoves = GetMovesForKnight(white, pos) & BitBoard[PieceType.KnightOfColor(!white)];
            UInt64 bishopMoves = GetMovesForBishop(white, pos) & (BitBoard[PieceType.BishopOfColor(!white)] | BitBoard[PieceType.QueenOfColor(!white)]);
            UInt64 rookMoves = GetMovesForRook(white, pos) & (BitBoard[PieceType.RookOfColor(!white)] | BitBoard[PieceType.QueenOfColor(!white)]);
            UInt64 kingMoves = GetMovesForKing(white, pos) & BitBoard[PieceType.KingOfColor(!white)];
            return pawnMoves | knightMoves | bishopMoves | rookMoves | kingMoves;
        }

        public GameResult GetGameResultFromMoves(List<(byte, byte, byte)> moves) {
            if (HalfMovesSincePawnOrCapture >= 50) {
                return GameResult.Draw;
            }
            if (moves.Count == 0) {
                var king = PiecePositions.Find(p => p.type == PieceType.KingOfColor(WhiteTurn));
                if (SquareAttackers(WhiteTurn, king.pos) != 0) {
                    return WhiteTurn ? GameResult.BlackWin : GameResult.WhiteWin;
                } else {
                    return GameResult.Draw;
                }
            }
            return GameResult.Ongoing;
        }

        public List<(byte, byte, byte)> GetRealMoves() {
            var pseudoMoves = EncodePseudoMoves(AvailablePiecePseudoMoves());
            RemoveIllegalMoves(pseudoMoves);
            return pseudoMoves;
        }

        public void RemoveIllegalMoves(List<(byte, byte, byte)> pseudoMoves) {
            for (int i = pseudoMoves.Count-1; i >= 0; i--) {
                Board board = new Board(this);
                board.MakeMove(pseudoMoves[i].Item1, pseudoMoves[i].Item2, pseudoMoves[i].Item3);
                var king = board.PiecePositions.Find(p => p.type == PieceType.KingOfColor(WhiteTurn));
                if (board.SquareAttackers(WhiteTurn, king.pos) != 0) {
                    pseudoMoves.RemoveAt(i);
                }
            }
        }

        public List<(byte, byte, byte)> GetPseudoMoves() {
            return EncodePseudoMoves(AvailablePiecePseudoMoves());
        }

        public List<(byte, byte, byte)> EncodePseudoMoves(List<(byte, byte, UInt64)> pieceMoves) {
            List<(byte, byte, byte)> result = new List<(byte, byte, byte)>();
            foreach (var pieceMove in pieceMoves) {
                if (pieceMove.Item3 == 0) {
                    continue;
                }
                for (byte to = 0; to < 64; to++) {
                    UInt64 pos = 1ul << to;
                    if ((pieceMove.Item3 & pos) != 0) {
                        result.Add((pieceMove.Item1, pieceMove.Item2, to));
                    }
                }
            }
            return result;
        }

        public List<(byte, byte, UInt64)> AvailablePiecePseudoMoves() {
            List<(byte, byte, UInt64)> moves = new List<(byte, byte, UInt64)>();
            //6 bits from, 6 bits to, 4 bits promotion?
            foreach (var piece in PiecePositions) {
                if (PieceType.IsWhite(piece.type) != WhiteTurn) {
                    continue;
                }
                moves.Add(AvailablePiecePseudoMoves(piece));
            }
            return moves;
        }

        public (byte, byte, UInt64) AvailablePiecePseudoMoves((byte, byte) pos) {
            UInt64 moves = 0;
            bool whitePiece = PieceType.IsWhite(pos.Item1);
            if (PieceType.IsPawn(pos.Item1)) {
                moves = GetMovesForPawn(whitePiece, pos.Item2);
            } else if (PieceType.IsKnight(pos.Item1)) {
                moves = GetMovesForKnight(whitePiece, pos.Item2);
            } else if (PieceType.IsBishop(pos.Item1)) {
                moves = GetMovesForBishop(whitePiece, pos.Item2);
            } else if (PieceType.IsRook(pos.Item1)) {
                moves = GetMovesForRook(whitePiece, pos.Item2);
            } else if (PieceType.IsQueen(pos.Item1)) {
                moves = GetMovesForQueen(whitePiece, pos.Item2);
            } else if (PieceType.IsKing(pos.Item1)) {
                moves = GetMovesForKing(whitePiece, pos.Item2);
            }
            return (pos.Item1, pos.Item2, moves);
        }

        public UInt64 GetMovesForPawn(bool white, byte pos) {
            UInt64 piece = 1ul << pos;
            UInt64 push;
            UInt64 attack = 0;
            UInt64 allPieces = AllPieces;
            UInt64 enPassant = 1ul << EnPassant;
            int yPos = pos/8;
            if (white) {
                push = (piece << 8) & (~allPieces);
                if (yPos == 1 && push != 0) {
                    push = (push | piece << 16) & (~allPieces);
                }
                attack = ((piece << 7) | (piece << 9)) & RayHelpers.Ranks[yPos+1] & (BlackPieces | enPassant);
            } else {
                push = (piece >> 8) & (~allPieces);
                if (yPos == 6 && push != 0) {
                    push = (push | piece >> 16) & (~allPieces);
                }
                attack = ((piece >> 7) | (piece >> 9)) & RayHelpers.Ranks[yPos-1] & (WhitePieces | enPassant);
            }
            return push | attack;
        }

        public UInt64 GetMovesForKnight(bool white, byte pos) {
            UInt64 moves = RayHelpers.KnightMoves[pos];
            if (white) {
                moves = moves & (~WhitePieces);
            } else {
                moves = moves & (~BlackPieces);
            }
            return moves;
        }

        //todo: can be done faster afaik with bitscans and some shit
        public UInt64 GetMovesForBishop(bool white, byte pos) {
            UInt64 moves = 0;
            UInt64 blackPieces = BlackPieces;
            UInt64 whitePieces = WhitePieces;
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.NorthEast]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.SouthEast]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.SouthWest]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.NorthWest]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            return moves;
        }

        public UInt64 GetMovesForRook(bool white, byte pos) {
            UInt64 moves = 0;
            UInt64 blackPieces = BlackPieces;
            UInt64 whitePieces = WhitePieces;
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.North]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.East]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.South]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.West]) {
                UInt64 boardMove = 1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            return moves;
        }

        public UInt64 GetMovesForQueen(bool white, byte pos) {
            return GetMovesForRook(white, pos) | GetMovesForBishop(white, pos);
        }

        public UInt64 GetMovesForKing(bool white, byte pos) {
            UInt64 moves = RayHelpers.KingMoves[pos];
            UInt64 allPieces = AllPieces;
            UInt64 boardPos = 1ul << pos;
            if (white) {
                moves = moves & (~WhitePieces);
                if ((CastlingRights & 0b1000) != 0 && (((boardPos << 1) | (boardPos << 2)) & allPieces) == 0) {
                    moves |= boardPos << 2;
                }
                if ((CastlingRights & 0b0100) != 0 && (((boardPos >> 1) | (boardPos >> 2) | (boardPos >> 3)) & allPieces) == 0) {
                    moves |= boardPos >> 2;
                }
            } else {
                moves = moves & (~BlackPieces);
                if ((CastlingRights & 0b0010) != 0 && (((boardPos << 1) | (boardPos << 2)) & allPieces) == 0) {
                    moves |= boardPos << 2;
                }
                if ((CastlingRights & 0b0001) != 0 && (((boardPos >> 1) | (boardPos >> 2) | (boardPos >> 3)) & allPieces) == 0) {
                    moves |= boardPos >> 2;
                }
            }
            return moves;
        }

        public static byte Position1DFrom2D(int x, int y) {
            return (byte) (x+y*8);
        }

        public static (int, int) Position2DFrom1D(int pos) {
            return (pos%8, pos/8);
        }

        public string BoardToFen() {
            string result = "";
            for (int y = 7; y >= 0; y--) {
                if (y < 7) {
                    result += "/";
                }
                int empty = 0;
                for (UInt64 x = 0; x < 8; x++) {
                    UInt64 pos = 0b1ul << ((int) x+y*8);
                    int prevLength = result.Length;
                    if ((BitBoard[PieceType.WhitePawn] & pos) != 0) {
                        result += 'P';
                    } else if ((BitBoard[PieceType.BlackPawn] & pos) != 0) {
                        result += ('p');
                    } else if ((BitBoard[PieceType.WhiteKnight] & pos) != 0) {
                        result += ('N');
                    } else if ((BitBoard[PieceType.BlackKnight] & pos) != 0) {
                        result += ('n');
                    } else if ((BitBoard[PieceType.WhiteBishop] & pos) != 0) {
                        result += ('B');
                    } else if ((BitBoard[PieceType.BlackBishop] & pos) != 0) {
                        result += ('b');
                    } else if ((BitBoard[PieceType.WhiteRook] & pos) != 0) {
                        result += ('R');
                    } else if ((BitBoard[PieceType.BlackRook] & pos) != 0) {
                        result += ('r');
                    } else if ((BitBoard[PieceType.WhiteQueen] & pos) != 0) {
                        result += ('Q');
                    } else if ((BitBoard[PieceType.BlackQueen] & pos) != 0) {
                        result += ('q');
                    } else if ((BitBoard[PieceType.WhiteKing] & pos) != 0) {
                        result += ('K');
                    } else if ((BitBoard[PieceType.BlackKing] & pos) != 0) {
                        result += ('k');
                    } else {
                        empty++;
                    }
                    if (result.Length > prevLength && empty > 0) {
                        result = result.Insert(result.Length-1, empty.ToString());
                        empty = 0;
                    }
                    if (x == 7 && empty > 0) {
                        result += empty;
                        empty = 0;
                    }
                }
            }
            result += " " + (WhiteTurn ? 'w' : 'b');
            result += " ";
            if (CastlingRights == 0) {
                result += "-";
            } else {
                if ((CastlingRights & 0b1000) != 0) {
                    result += "K";
                }
                if ((CastlingRights & 0b0100) != 0) {
                    result += "Q";
                }
                if ((CastlingRights & 0b0010) != 0) {
                    result += "k";
                }
                if ((CastlingRights & 0b0001) != 0) {
                    result += "q";
                }
            }
            result += " ";
            if (EnPassant < 64) {
                result += (char) ('a'+Position2DFrom1D(EnPassant).Item1);
                result += (char) ('1'+Position2DFrom1D(EnPassant).Item2);
            } else {
                result += "-";
            }
            result += " " + HalfMovesSincePawnOrCapture;
            result += " 1";
            return result;
        }

        public static Board InitializeFromFen(string fen) {
            Board board = new Board();
            Match match = Regex.Match(fen, 
                @"([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/"
                +@"([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})"
                +@" (w|b) ([KQkq-]{1,4}) ([a-h][1-8]|-) (\d+)");
            //todo: add reading of turn count?
            if (match.Groups[9].Value == "b") {
                board.WhiteTurn = false;
            } else {
                board.WhiteTurn = true;
            }
            board.CastlingRights = 0;
            if (match.Groups[10].Value.Contains("K")) {
                board.CastlingRights |= 0b1000;
            } 
            if (match.Groups[10].Value.Contains("Q")) {
                board.CastlingRights |= 0b0100;
            }
            if (match.Groups[10].Value.Contains("k")) {
                board.CastlingRights |= 0b0010;
            }
            if (match.Groups[10].Value.Contains("q")) {
                board.CastlingRights |= 0b0001;
            }
            if (match.Groups[11].Value != "-") {
                board.EnPassant = (byte) (match.Groups[11].Value[0]-'a'+(match.Groups[11].Value[1]-'1')*8);
            }
            board.HalfMovesSincePawnOrCapture = int.Parse(match.Groups[12].Value);
            List<(byte type, byte pos)> piecePositions = new List<(byte type, byte pos)>(); 
            for (int y = 7; y >= 0; y--) {
                UInt64 x = 0;
                string row = match.Groups[8-y].Value;
                for (int i = 0; i < row.Length; i++) {
                    if (Char.IsDigit(row[i])) {
                        x += (UInt64) (row[i]-'0');
                        continue;
                    }
                    byte boardPos = (byte) (x+(ulong) y*8);
                    UInt64 pos = 1ul << boardPos;
                    if (row[i] == 'p') {
                        board.BitBoard[PieceType.BlackPawn] |= pos;
                        piecePositions.Add((PieceType.BlackPawn, boardPos));
                    } else if (row[i] == 'P') {
                        board.BitBoard[PieceType.WhitePawn] |= pos;
                        piecePositions.Add((PieceType.WhitePawn, boardPos));
                    } else if (row[i] == 'n') {
                        board.BitBoard[PieceType.BlackKnight] |= pos;
                        piecePositions.Add((PieceType.BlackKnight, boardPos));
                    } else if (row[i] == 'N') {
                        board.BitBoard[PieceType.WhiteKnight] |= pos;
                        piecePositions.Add((PieceType.WhiteKnight, boardPos));
                    } else if (row[i] == 'b') {
                        board.BitBoard[PieceType.BlackBishop] |= pos;
                        piecePositions.Add((PieceType.BlackBishop, boardPos));
                    } else if (row[i] == 'B') {
                        board.BitBoard[PieceType.WhiteBishop] |= pos;
                        piecePositions.Add((PieceType.WhiteBishop, boardPos));
                    } else if (row[i] == 'r') {
                        board.BitBoard[PieceType.BlackRook] |= pos;
                        piecePositions.Add((PieceType.BlackRook, boardPos));
                    } else if (row[i] == 'R') {
                        board.BitBoard[PieceType.WhiteRook] |= pos;
                        piecePositions.Add((PieceType.WhiteRook, boardPos));
                    } else if (row[i] == 'q') {
                        board.BitBoard[PieceType.BlackQueen] |= pos;
                        piecePositions.Add((PieceType.BlackQueen, boardPos));
                    } else if (row[i] == 'Q') {
                        board.BitBoard[PieceType.WhiteQueen] |= pos;
                        piecePositions.Add((PieceType.WhiteQueen, boardPos));
                    } else if (row[i] == 'k') {
                        board.BitBoard[PieceType.BlackKing] |= pos;
                        piecePositions.Add((PieceType.BlackKing, boardPos));
                    } else if (row[i] == 'K') {
                        board.BitBoard[PieceType.WhiteKing] |= pos;
                        piecePositions.Add((PieceType.WhiteKing, boardPos));
                    }
                    x++;
                }
            }
            board.PiecePositions = piecePositions;
            return board;
        }
    }
}
