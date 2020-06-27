using System;

namespace ConsoleChess {
    class Program {
        static void Main(string[] args) {
            Game game = new Game();
            game.InitializeBoard();
            game.DrawBoard();
            while (true) {
                Console.WriteLine();
                Random random = new Random();
                var moves = game.CurrentBoard.EncodedPseudoMoves();
                int selected = random.Next(moves.Count);
                game.CurrentBoard.MakeMove(moves[selected].Item1, moves[selected].Item2, moves[selected].Item3);
                game.DrawBoard();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
