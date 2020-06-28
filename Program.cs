using System;

namespace ConsoleChess {
    class Program {
        static void Main(string[] args) {
            Game testGame = new Game();
            testGame.InitializeBoard();
            testGame.CurrentBoard.MakeMove(PieceType.WhitePawn, Board.Position1DFrom2D(4, 1), Board.Position1DFrom2D(4, 3));
            testGame.CurrentBoard.MakeMove(PieceType.BlackPawn, Board.Position1DFrom2D(5, 6), Board.Position1DFrom2D(5, 4));
            testGame.CurrentBoard.MakeMove(PieceType.WhiteBishop, Board.Position1DFrom2D(5, 0), Board.Position1DFrom2D(0, 5));
            testGame.CurrentBoard.MakeMove(PieceType.BlackPawn, Board.Position1DFrom2D(3, 6), Board.Position1DFrom2D(3, 4));
            testGame.CurrentBoard.MakeMove(PieceType.WhiteQueen, Board.Position1DFrom2D(3, 0), Board.Position1DFrom2D(7, 4));
            testGame.CurrentBoard.MakeMove(PieceType.BlackKing, Board.Position1DFrom2D(4, 7), Board.Position1DFrom2D(3, 6));
            testGame.CurrentBoard.MakeMove(PieceType.WhiteBishop, Board.Position1DFrom2D(0, 5), Board.Position1DFrom2D(1, 4));
            //testGame.CurrentBoard.MakeMove(PieceType.BlackKing, Board.Position1DFrom2D(3, 6), Board.Position1DFrom2D(4, 5));
            //testGame.CurrentBoard.MakeMove(PieceType.WhiteQueen, Board.Position1DFrom2D(7, 4), Board.Position1DFrom2D(5, 4));
            //testGame.CurrentBoard.MakeMove(PieceType.BlackKing, Board.Position1DFrom2D(4, 5), Board.Position1DFrom2D(3, 5));
            //testGame.CurrentBoard.MakeMove(PieceType.WhiteQueen, Board.Position1DFrom2D(5, 4), Board.Position1DFrom2D(3, 4));
            //RayHelpers.DrawBitBoard(testGame.CurrentBoard.SquareAttackers(true, Board.Position1DFrom2D(0, 5)));
            //RayHelpers.DrawBitBoard(testGame.CurrentBoard.SquareAttackers(true, Board.Position1DFrom2D(3, 5)));
            //RayHelpers.DrawBitBoard(testGame.CurrentBoard.SquareAttackers(false, Board.Position1DFrom2D(3, 5)));
            testGame.CurrentBoard.MakeMove(PieceType.BlackPawn, Board.Position1DFrom2D(2, 6), Board.Position1DFrom2D(2, 5));
            testGame.CurrentBoard.MakeMove(PieceType.WhiteQueen, Board.Position1DFrom2D(7, 4), Board.Position1DFrom2D(4, 7));
            RayHelpers.DrawBitBoard(testGame.CurrentBoard.AllPieces);
            RayHelpers.DrawBitBoard(testGame.CurrentBoard.SquareAttackers(false, Board.Position1DFrom2D(3, 6)));
            Console.WriteLine(testGame.CurrentBoard.GetGameResultFromMoves(testGame.CurrentBoard.GetRealMoves()));
            Console.WriteLine(testGame.CurrentBoard.BoardToFen());
            testGame.CurrentBoard = Board.InitializeFromFen("r3k2r/1bppqppp/p1n2n2/1pb5/2PNp2P/2NPP3/PPQBBPP1/R3K2R b KQkq - 2 11");
            Console.WriteLine(testGame.CurrentBoard.GetRealMoves());
            testGame.CurrentBoard.MakeMove(PieceType.BlackKing, Board.Position1DFrom2D(4, 7), Board.Position1DFrom2D(6, 7));
            testGame.DrawBoard();
            testGame.CurrentBoard.MakeMove(PieceType.WhiteKing, Board.Position1DFrom2D(4, 0), Board.Position1DFrom2D(2, 0));
            testGame.DrawBoard();
            Game game = new Game();
            game.InitializeBoard();
            //game.CurrentBoard = Board.InitializeFromFen("8/2B5/1k2K3/7b/6p1/6P1/8/8 b KQkq - 50 1");
            game.StartGame();
        }
    }
}
