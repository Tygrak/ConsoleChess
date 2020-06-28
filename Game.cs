using System;

namespace ConsoleChess {
    public class Game {
        internal Board CurrentBoard;
        public ConsoleColor WhiteColor = ConsoleColor.White;
        public ConsoleColor BlackColor = ConsoleColor.Red;

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
                int selected = random.Next(moves.Count);
                CurrentBoard.MakeMove(moves[selected].Item1, moves[selected].Item2, moves[selected].Item3);
                DrawBoard();
                //System.Threading.Thread.Sleep(1000);
            }
            Console.WriteLine(CurrentBoard.BoardToFen());
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
