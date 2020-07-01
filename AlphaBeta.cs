using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

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

        private TranspositionTable transpositionTable = new TranspositionTable();

        public List<(int, int, int, float)> FindMoveAlphaBeta(Board board, int depth) {
            List<(int, int, int, float)> result = new List<(int, int, int, float)>();
            var moves = board.GetRealMoves();
            stopwatch.Reset();
            copying.Reset();
            findingMoves.Reset();
            rating.Reset();
            moving.Reset();
            stopwatch.Start();
            ConcurrentBag<(int, int, int, float)> searchResults = new ConcurrentBag<(int, int, int, float)>(); 
            Parallel.ForEach(moves, move => {
                Board clone = new Board(board);
                clone.MakeMove(move.Item1, move.Item2, move.Item3);
                searchResults.Add((move.Item1, move.Item2, move.Item3, EvaluateAlphaBetaTt(clone, depth, NegativeInfinity, PositiveInfinity, !board.WhiteTurn)));
            });
            result = new List<(int, int, int, float)>(searchResults);
            /*foreach (var move in moves) {
                Board clone = new Board(board);
                clone.MakeMove(move.Item1, move.Item2, move.Item3);
                result.Add((move.Item1, move.Item2, move.Item3, 
                            EvaluateAlphaBetaTt(clone, depth, NegativeInfinity, PositiveInfinity, !board.WhiteTurn)));
            }*/
            if (board.WhiteTurn) {
                result.Sort((a, b) => b.Item4.CompareTo(a.Item4));
            } else {
                result.Sort((a, b) => -b.Item4.CompareTo(a.Item4));
            }
            //result.Sort((a, b) => b.Item4.CompareTo(a.Item4));
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

        public float EvaluateAlphaBetaTt(Board board, int depth, float alpha, float beta, bool maximizingPlayer) {
            TtEntry entry = transpositionTable.Get(board.Zobrist);
            if (entry.IsValid() && entry.Depth() >= depth) {
                if (entry.EntryType() == TtEntry.EntryTypeExact) {
                    //if (MathF.Abs(entry.Score()) >= 1000000)
                    return entry.Score();
                } else if ((entry.EntryType() == TtEntry.EntryTypeLowerBound) && (entry.Score() > alpha)) {
                    alpha = entry.Score();
                } else if ((entry.EntryType() == TtEntry.EntryTypeHigherBound) && (entry.Score() < beta)) {
                    beta = entry.Score();
                }
                if (alpha >= beta) {
                    return alpha;
                }
            }
            if (depth == 0) {
                rating.Start();
                float rate = evaluation.RateBoard(board);
                rating.Stop();
                if (rate <= alpha) {
                    transpositionTable.Add(
                        new TtEntry(
                            board.Zobrist, 
                            TtEntry.CreateData(TtEntry.EntryTypeLowerBound, PieceType.Empty, 0, 0, rate, depth)
                        )
                    );
                } else if (rate >= beta) {
                    transpositionTable.Add(
                        new TtEntry(
                            board.Zobrist, 
                            TtEntry.CreateData(TtEntry.EntryTypeHigherBound, PieceType.Empty, 0, 0, rate, depth)
                        )
                    );
                } else {
                    transpositionTable.Add(
                        new TtEntry(
                            board.Zobrist, 
                            TtEntry.CreateData(TtEntry.EntryTypeExact, PieceType.Empty, 0, 0, rate, depth)
                        )
                    );
                }
                return rate;
            } else if (board.HalfMovesSincePawnOrCapture > 46) {
                return 0;
            }
            findingMoves.Start();
            var moves = board.GetPseudoMoves();
            findingMoves.Stop();
            bool foundLegalMove = false;
            float value = NegativeInfinity;
            Board clone = new Board(board);
            if (maximizingPlayer) {
                value = NegativeInfinity;
                foreach (var move in moves) {
                    moving.Start();
                    bool isMoveLegal = board.TryMakeMove(move.Item1, move.Item2, move.Item3);
                    moving.Stop();
                    if (isMoveLegal) {
                        foundLegalMove = true;
                        value = MathF.Max(value, EvaluateAlphaBetaTt(board, depth-1, alpha, beta, false));
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
                        value = Math.Min(value, EvaluateAlphaBetaTt(board, depth-1, alpha, beta, true));
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
                    if (board.WhiteTurn) {
                        value += 1000000*(depth+1);
                    } else {
                        value += -1000000*(depth+1);
                    }
                } else {
                    value = 0;
                }
            }
            if (value <= alpha) {
                transpositionTable.Add(
                    new TtEntry(
                        board.Zobrist, 
                        TtEntry.CreateData(TtEntry.EntryTypeLowerBound, PieceType.Empty, 0, 0, value, depth)
                    )
                );
            } else if (value >= beta) {
                transpositionTable.Add(
                    new TtEntry(
                        board.Zobrist, 
                        TtEntry.CreateData(TtEntry.EntryTypeHigherBound, PieceType.Empty, 0, 0, value, depth)
                    )
                );
            } else {
                transpositionTable.Add(
                    new TtEntry(
                        board.Zobrist, 
                        TtEntry.CreateData(TtEntry.EntryTypeExact, PieceType.Empty, 0, 0, value, depth)
                    )
                );
            }
            return value;
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
