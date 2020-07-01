using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ConsoleChess {
    public class AlphaBeta {
        const float PositiveInfinity = 100000000;
        const float NegativeInfinity = -100000000;

        Stopwatch stopwatch = new Stopwatch();
        Stopwatch copying = new Stopwatch();
        Stopwatch findingMoves = new Stopwatch();
        Stopwatch moving = new Stopwatch();
        Stopwatch rating = new Stopwatch();

        Evaluation evaluation = new Evaluation();

        public List<(int, int, int, float)> FindMoveAlphaBeta(Board board, int depth) {
            List<(int, int, int, float)> result = new List<(int, int, int, float)>();
            var moves = board.GetRealMoves();
            stopwatch.Reset();
            copying.Reset();
            findingMoves.Reset();
            rating.Reset();
            moving.Reset();
            stopwatch.Start();
            /*Parallel.ForEach(moves, move => {
                Board clone = new Board(board);
                clone.MakeMove(move.Item1, move.Item2, move.Item3);
                result.Add((move.Item1, move.Item2, move.Item3, EvaluateAlphaBeta(clone, depth, NegativeInfinity, PositiveInfinity, !board.WhiteTurn)));
            });*/
            foreach (var move in moves) {
                Board clone = new Board(board);
                clone.MakeMove(move.Item1, move.Item2, move.Item3);
                result.Add((move.Item1, move.Item2, move.Item3, 
                            EvaluateAlphaBeta(clone, depth, NegativeInfinity, PositiveInfinity, !board.WhiteTurn)));
            }
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
                float rate = evaluation.RateBoard(board);
                rating.Stop();
                return rate;
            } else if (board.HalfMovesSincePawnOrCapture > 46) {
                return 0;
            }
            findingMoves.Start();
            var moves = board.GetPseudoMoves();
            findingMoves.Stop();
            bool foundLegalMove = false;
            float value = 0;
            Board clone = new Board(board);
            if (maximizingPlayer) {
                value = NegativeInfinity;
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
                value = PositiveInfinity;
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
                if (board.SquareAttackers(board.WhiteTurn, board.GetKingOfColorPosition(board.WhiteTurn)) != 0) {
                    return (board.WhiteTurn ? -1000000 : 1000000)*(depth+1);
                } else {
                    return 0;
                }
            }
            return value;
        }
    }
}
