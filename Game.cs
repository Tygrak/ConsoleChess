using System;
using System.Text.RegularExpressions;

namespace ConsoleChess {
    public class Game {
        internal Board CurrentBoard;
        public ConsoleColor WhiteColor = ConsoleColor.White;
        public ConsoleColor BlackColor = ConsoleColor.Red;
        public AlphaBeta alphaBeta = new AlphaBeta();
        int halfTurnCount = 1;
        bool playerWhite = false;
        bool playerBlack = false;
        int whiteDepth = 5;
        int blackDepth = 3;

        public void InitializeBoard() {
            CurrentBoard = Board.InitializeFromFen(@"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public void StartGame() {
            DrawBoard();
            while (true) {
                Console.WriteLine();
                Random random = new Random();
                var moves = CurrentBoard.GetRealMoves();
                GameResult gameResult = CurrentBoard.GetGameResultFromMoves(moves);
                if (gameResult == GameResult.Draw) {
                    Console.WriteLine("Draw!");
                    break;
                } else if (gameResult == GameResult.WhiteWin) {
                    Console.WriteLine("Checkmate! White wins!");
                    break;
                } else if (gameResult == GameResult.BlackWin) {
                    Console.WriteLine("Checkmate! Black wins!");
                    break;
                }
                if ((CurrentBoard.WhiteTurn && !playerWhite) || (!CurrentBoard.WhiteTurn && !playerBlack)) {
                    var sortedMoves = alphaBeta.FindMoveAlphaBeta(CurrentBoard, CurrentBoard.WhiteTurn ? whiteDepth : blackDepth);
                    int selected = 0;
                    if (halfTurnCount < 4) {
                        selected = random.Next(Math.Min(moves.Count, 3));
                    } else if (halfTurnCount < 8) {
                        selected = random.Next(Math.Min(moves.Count, 2));
                    }
                    Console.WriteLine($"{sortedMoves[selected].Item4.ToString("0.000")}: "
                                      + $"{Board.Position1DToAlgebraic(sortedMoves[selected].Item2)} -> "
                                      + $"{Board.Position1DToAlgebraic(sortedMoves[selected].Item3)}");
                    CurrentBoard.MakeMove(sortedMoves[selected].Item1, sortedMoves[selected].Item2, sortedMoves[selected].Item3);
                } else {
                    var playerMove = GetPlayerMove();
                    var moveIndex = moves.FindIndex(m => m.Item2 == playerMove.Item1 && m.Item3 == playerMove.Item2);
                    while (moveIndex == -1) {
                        Console.WriteLine("Invalid move, try again:");
                        playerMove = GetPlayerMove();
                        moveIndex = moves.FindIndex(m => m.Item2 == playerMove.Item1 && m.Item3 == playerMove.Item2);
                    }
                    CurrentBoard.MakeMove(moves[moveIndex].Item1, moves[moveIndex].Item2, moves[moveIndex].Item3);
                }
                halfTurnCount++;
                DrawBoard();
                //System.Threading.Thread.Sleep(1000);
            }
            Console.WriteLine(CurrentBoard.BoardToFen((halfTurnCount+1)/2));
        }

        public (byte, byte) GetPlayerMove() {
            Console.Write("Move:");
            string input = "";
            Match match = Regex.Match(input, @"x");
            while (!match.Success) {
                if (input == "fen") {
                    Console.WriteLine(CurrentBoard.BoardToFen((halfTurnCount+1)/2));
                }
                input = Console.ReadLine().ToLowerInvariant();
                match = Regex.Match(input, @"([a-h][1-8])[ -]+([a-h][1-8])");
            }
            return (Board.PositionAlgebraicTo1D(match.Groups[1].Value), Board.PositionAlgebraicTo1D(match.Groups[2].Value));
        }

        public void DrawBoard() {
            Console.Write("  ");
            for (int x = 0; x < 8; x++) {
                if (x > 0) {
                    Console.Write(' ');
                }
                Console.Write((char) ('a'+x));
            }
            Console.Write('\n');
            Console.Write('\n');
            for (int y = 7; y >= 0; y--) {
                if (y < 7) {
                    Console.Write('\n');
                    Console.Write("  ");
                    for (UInt64 x = 0; x < 8; x++) {
                        if (x > 0) {
                            Console.Write('╋');
                        }
                        Console.Write('━');
                    }
                    Console.Write('\n');
                }
                Console.Write((char) ('1'+y));
                Console.Write(' ');
                for (UInt64 x = 0; x < 8; x++) {
                    if (x > 0) {
                        Console.Write('┃');
                    }
                    if ((y % 2 == 0 && x % 2 != 0) || (y % 2 != 0 && x % 2 == 0)) {
                        //Console.BackgroundColor = ConsoleColor.White;
                    } else {
                        //Console.BackgroundColor = ConsoleColor.Cyan;
                    }
                    UInt64 pos = 0b1ul << ((int) x+y*8);
                    if ((CurrentBoard.BitBoard[PieceType.WhitePawn] & pos) != 0) {
                        Console.ForegroundColor = WhiteColor;
                        Console.Write('P');
                    } else if ((CurrentBoard.BitBoard[PieceType.BlackPawn] & pos) != 0) {
                        Console.ForegroundColor = BlackColor;
                        Console.Write('p');
                    } else if ((CurrentBoard.BitBoard[PieceType.WhiteKnight] & pos) != 0) {
                        Console.ForegroundColor = WhiteColor;
                        Console.Write('N');
                    } else if ((CurrentBoard.BitBoard[PieceType.BlackKnight] & pos) != 0) {
                        Console.ForegroundColor = BlackColor;
                        Console.Write('n');
                    } else if ((CurrentBoard.BitBoard[PieceType.WhiteBishop] & pos) != 0) {
                        Console.ForegroundColor = WhiteColor;
                        Console.Write('B');
                    } else if ((CurrentBoard.BitBoard[PieceType.BlackBishop] & pos) != 0) {
                        Console.ForegroundColor = BlackColor;
                        Console.Write('b');
                    } else if ((CurrentBoard.BitBoard[PieceType.WhiteRook] & pos) != 0) {
                        Console.ForegroundColor = WhiteColor;
                        Console.Write('R');
                    } else if ((CurrentBoard.BitBoard[PieceType.BlackRook] & pos) != 0) {
                        Console.ForegroundColor = BlackColor;
                        Console.Write('r');
                    } else if ((CurrentBoard.BitBoard[PieceType.WhiteQueen] & pos) != 0) {
                        Console.ForegroundColor = WhiteColor;
                        Console.Write('Q');
                    } else if ((CurrentBoard.BitBoard[PieceType.BlackQueen] & pos) != 0) {
                        Console.ForegroundColor = BlackColor;
                        Console.Write('q');
                    } else if ((CurrentBoard.BitBoard[PieceType.WhiteKing] & pos) != 0) {
                        Console.ForegroundColor = WhiteColor;
                        Console.Write('K');
                    } else if ((CurrentBoard.BitBoard[PieceType.BlackKing] & pos) != 0) {
                        Console.ForegroundColor = BlackColor;
                        Console.Write('k');
                    } else {
                        Console.Write(' ');
                    }
                    Console.ResetColor();
                }
                Console.Write(' ');
                Console.Write((char) ('1'+y));
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("  ");
            for (int x = 0; x < 8; x++) {
                if (x > 0) {
                    Console.Write(' ');
                }
                Console.Write((char) ('a'+x));
            }
            Console.WriteLine();
        }
    }
}
