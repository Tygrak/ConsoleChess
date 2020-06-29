using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleChess {
    public class AlphaBeta {
        Stopwatch stopwatch = new Stopwatch();
        Stopwatch copying = new Stopwatch();
        Stopwatch findingMoves = new Stopwatch();
        Stopwatch moving = new Stopwatch();
        Stopwatch rating = new Stopwatch();

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
                    value += 0.1f*(4.5f-Math.Abs(pos.x-4.5f));
                    if (PieceType.IsWhite(piece.type)) {
                        value += 0.05f*pos.y;
                    }
                } else if (PieceType.IsKnight(piece.type)) {
                    value = 3;
                    value += pos.x > 1 && pos.x < 6 ? 0.25f : pos.x > 0 && pos.x < 7 ? 0.125f : 0;
                    value += pos.y > 1 && pos.y < 6 ? 0.25f : pos.y > 0 && pos.y < 7 ? 0.125f : 0;
                } else if (PieceType.IsBishop(piece.type)) {
                    value = 3;
                    value += BitHelpers.GetNumberOfSetBits(board.GetMovesForBishop(PieceType.IsWhite(piece.type), piece.pos))*0.05f;
                } else if (PieceType.IsRook(piece.type)) {
                    value = 5;
                } else if (PieceType.IsQueen(piece.type)) {
                    value = 9;
                } else if (PieceType.IsKing(piece.type)) {
                    value = 0;
                    value += pos.x < 2 && pos.x > 5 ? 0.2f : pos.x < 3 && pos.x > 4 ? 0.075f : -0.1f;
                    if (PieceType.IsWhite(piece.type)) {
                        value += pos.y == 0 ? 0.5f : 0;
                        value += 0.5f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingShield[piece.pos][1] & board.BitBoard[PieceType.WhitePawn]);
                        value -= 0.25f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingSphere[piece.pos] & blackPieces);
                        value = value*(1f+blackPiecesAmount)/16;
                    } else {
                        value += pos.y == 7 ? 0.5f : 0;
                        value += 0.5f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingShield[piece.pos][0] & board.BitBoard[PieceType.BlackPawn]);
                        value -= 0.25f*BitHelpers.GetNumberOfSetBits(RayHelpers.KingSphere[piece.pos] & whitePieces);
                        value = value*(1f+whitePiecesAmount)/16;
                    }
                }
                result += PieceType.IsWhite(piece.type) ? value : -value;
            }
            return result;
        }

        public List<(byte, byte, byte, double)> FindMoveAlphaBeta(Board board, int depth) {
            List<(byte, byte, byte, double)> result = new List<(byte, byte, byte, double)>();
            var moves = board.GetRealMoves();
            stopwatch.Reset();
            copying.Reset();
            findingMoves.Reset();
            rating.Reset();
            moving.Reset();
            stopwatch.Start();
            Parallel.ForEach(moves, move => {
                Board clone = new Board(board);
                clone.MakeMove(move.Item1, move.Item2, move.Item3);
                result.Add((move.Item1, move.Item2, move.Item3, EvaluateAlphaBeta(clone, depth, float.NegativeInfinity, float.PositiveInfinity, !board.WhiteTurn)));
            });
            /*foreach (var move in moves) {
                Board clone = new Board(board);
                clone.MakeMove(move.Item1, move.Item2, move.Item3);
                result.Add((move.Item1, move.Item2, move.Item3, 
                            EvaluateAlphaBeta(clone, depth, float.NegativeInfinity, float.PositiveInfinity, !board.WhiteTurn)));
            }*/
            if (board.WhiteTurn) {
                result.Sort((a, b) => b.Item4.CompareTo(a.Item4));
            } else {
                result.Sort((a, b) => -b.Item4.CompareTo(a.Item4));
            }
            stopwatch.Stop();
            copying.Stop();
            findingMoves.Stop();
            rating.Stop();
            moving.Stop();
            Console.WriteLine("Time:" + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Copying:" + copying.ElapsedMilliseconds);
            Console.WriteLine("Finding:" + findingMoves.ElapsedMilliseconds);
            Console.WriteLine("Rating:" + rating.ElapsedMilliseconds);
            Console.WriteLine("Moving:" + moving.ElapsedMilliseconds);
            return result;
        }

        public float EvaluateAlphaBeta(Board board, int depth, float alpha, float beta, bool maximizingPlayer) {
            if (depth == 0) {
                rating.Start();
                float rate = RateBoard(board);
                rating.Stop();
                return rate;
            }
            findingMoves.Start();
            var moves = board.GetPseudoMoves();
            findingMoves.Stop();
            bool foundLegalMove = false;
            float value = 0;
            Board clone = new Board(board);
            if (maximizingPlayer) {
                value = float.NegativeInfinity;
                foreach (var move in moves) {
                    moving.Start();
                    bool isMoveLegal = board.TryMakeMove(move.Item1, move.Item2, move.Item3);
                    moving.Stop();
                    if (isMoveLegal) {
                        foundLegalMove = true;
                        value = MathF.Max(value, EvaluateAlphaBeta(board, depth-1, alpha, beta, false));
                        alpha = MathF.Max(alpha, value);
                        copying.Start();
                        board.SetStateTo(clone);
                        copying.Stop();
                        if (alpha >= beta) {
                            break;
                        }
                    } else {
                        copying.Start();
                        board.SetStateTo(clone);
                        copying.Stop();
                    }
                }
            } else {
                value = float.PositiveInfinity;
                foreach (var move in moves) {
                    moving.Start();
                    bool isMoveLegal = board.TryMakeMove(move.Item1, move.Item2, move.Item3);
                    moving.Stop();
                    if (isMoveLegal) {
                        foundLegalMove = true;
                        value = Math.Min(value, EvaluateAlphaBeta(board, depth-1, alpha, beta, true));
                        beta = Math.Min(beta, value);
                        copying.Start();
                        board.SetStateTo(clone);
                        copying.Stop();
                        if (beta <= alpha) {
                            break;
                        }
                    } else {
                        copying.Start();
                        board.SetStateTo(clone);
                        copying.Stop();
                    }
                }
            }
            if (!foundLegalMove) {
                var king = board.PiecePositions.Find(p => p.type == PieceType.KingOfColor(board.WhiteTurn));
                if (board.SquareAttackers(board.WhiteTurn, king.pos) != 0) {
                    return (board.WhiteTurn ? -1000000 : 1000000)*(depth+1);
                } else {
                    return 0;
                }
            }
            return value;
        }
    }
}
