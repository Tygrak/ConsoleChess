using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleChess {
    public class Board {
        internal UInt64[] BitBoard = new UInt64[12];
        internal UInt16 CastlingEnPassant = 0b1111;
        internal int HalfMovesSinceCapture = 0;
        internal bool WhiteTurn = true;
        internal (int type, byte pos)[] PiecePositions = new (int type, byte pos)[0]; 

        private UInt64 WhitePieces {
            get {
                return BitBoard[PieceType.WhitePawn] | BitBoard[PieceType.WhiteKnight] | BitBoard[PieceType.WhiteBishop] |
                       BitBoard[PieceType.WhiteRook] | BitBoard[PieceType.WhiteQueen] | BitBoard[PieceType.WhiteKing];
            }
        }

        private UInt64 BlackPieces {
            get {
                return BitBoard[PieceType.BlackPawn] | BitBoard[PieceType.BlackKnight] | BitBoard[PieceType.BlackBishop] |
                       BitBoard[PieceType.BlackRook] | BitBoard[PieceType.BlackQueen] | BitBoard[PieceType.BlackKing];
            }
        }

        private UInt64 AllPieces {
            get {
                return WhitePieces | BlackPieces;
            }
        }

        public void MakeMove(int type, byte from, byte to) {
            UInt64 fromPos = 0b1ul << from;
            UInt64 toPos = 0b1ul << to;
            for (int i = 0; i < 12; i++) {
                BitBoard[i] = BitBoard[i] & (~toPos);
            }
            BitBoard[type] = (fromPos | toPos) ^ BitBoard[type];
            for (int i = PiecePositions.Length-1; i >= 0; i--) {
                if (PiecePositions[i].pos == to && PieceType.IsWhite(PiecePositions[i].type) != PieceType.IsWhite(type)) {
                    PiecePositions[i].pos = to;
                }
                if (PiecePositions[i].pos == from) {
                    PiecePositions[i].pos = to;
                }
            }
            WhiteTurn = !WhiteTurn;
        }

        public List<(int, byte, byte)> EncodedPseudoMoves() {
            return EncodePseudoMoves(AvailablePiecePseudoMoves());
        }

        public List<(int, byte, byte)> EncodePseudoMoves(List<(int, byte, UInt64)> pieceMoves) {
            List<(int, byte, byte)> result = new List<(int, byte, byte)>();
            foreach (var pieceMove in pieceMoves) {
                if (pieceMove.Item3 == 0) {
                    continue;
                }
                for (byte to = 0; to < 64; to++) {
                    UInt64 pos = 0b1ul << to;
                    if ((pieceMove.Item3 & pos) != 0) {
                        result.Add((pieceMove.Item1, pieceMove.Item2, to));
                    }
                }
            }
            return result;
        }

        public List<(int, byte, UInt64)> AvailablePiecePseudoMoves() {
            List<(int, byte, UInt64)> moves = new List<(int, byte, UInt64)>();
            //6 bits from, 6 bits to, 4 bits promotion?
            foreach (var piece in PiecePositions) {
                if (PieceType.IsWhite(piece.type) != WhiteTurn) {
                    continue;
                }
                moves.Add(AvailablePiecePseudoMoves(piece));
            }
            return moves;
        }

        public (int, byte, UInt64) AvailablePiecePseudoMoves((int, byte) pos) {
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
            //todo: enpassant
            UInt64 piece = 0b1ul << pos;
            UInt64 push;
            UInt64 attack = 0;
            UInt64 allPieces = AllPieces;
            int yPos = pos/8;
            if (white) {
                push = (piece << 8) & (~allPieces);
                if (yPos == 1 && push != 0) {
                    push = (push | piece << 16) & (~allPieces);
                }
                attack = ((piece << 7) | (piece << 9)) & RayHelpers.Ranks[yPos+1] & BlackPieces;
            } else {
                push = (piece >> 8) & (~allPieces);
                if (yPos == 6 && push != 0) {
                    push = (push | piece >> 16) & (~allPieces);
                }
                attack = ((piece >> 7) | (piece >> 9)) & RayHelpers.Ranks[yPos-1] & WhitePieces;
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
                UInt64 boardMove = 0b1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.SouthEast]) {
                UInt64 boardMove = 0b1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.SouthWest]) {
                UInt64 boardMove = 0b1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.NorthWest]) {
                UInt64 boardMove = 0b1ul << move;
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
                UInt64 boardMove = 0b1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.East]) {
                UInt64 boardMove = 0b1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.South]) {
                UInt64 boardMove = 0b1ul << move;
                if ((white && (boardMove & whitePieces) != 0) || (!white && (boardMove & blackPieces) != 0)) {
                    break;
                }
                moves |= boardMove;
                if ((white && (boardMove & blackPieces) != 0) || (!white && (boardMove & whitePieces) != 0)) {
                    break;
                }
            }
            foreach (var move in RayHelpers.PositionRays[pos][RayHelpers.West]) {
                UInt64 boardMove = 0b1ul << move;
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
            //todo: castling
            UInt64 moves = RayHelpers.KingMoves[pos];
            if (white) {
                moves = moves & (~WhitePieces);
            } else {
                moves = moves & (~BlackPieces);
            }
            return moves;
        }

        public static Board InitializeFromFen(string fen) {
            Board board = new Board();
            Match match = Regex.Match(fen, @"([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})\/([\w\d]{1,8})");
            //todo: add reading of half moves since capture/pawn move, current turn, white/black curr, en passant, castling rights 
            List<(int type, byte pos)> piecePositions = new List<(int type, byte pos)>(); 
            for (int y = 7; y >= 0; y--) {
                UInt64 x = 0;
                string row = match.Groups[8-y].Value;
                for (int i = 0; i < row.Length; i++) {
                    if (Char.IsDigit(row[i])) {
                        x += (UInt64) (row[i]-'0');
                        continue;
                    }
                    byte boardPos = (byte) (x+(ulong) y*8);
                    UInt64 pos = 0b1ul << boardPos;
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
            board.PiecePositions = piecePositions.ToArray();
            return board;
        }
    }
}
