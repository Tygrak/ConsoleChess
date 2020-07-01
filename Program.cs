using System;

namespace ConsoleChess {
    class Program {
        //viezha
        //lots of stuff modified from https://www.chessprogramming.org/ and https://github.com/bytefire/Shutranj/blob/master/Shutranj.Engine
        static void Main(string[] args) {
            TtEntry entry = new TtEntry(9, TtEntry.CreateData(0, 15, 0, 0, 69, 1));
            /*RayHelpers.DrawBitBoard(entry.Data);
            entry = new TtEntry(9, TtEntry.CreateData(1, 15, 0, 63, 0, 63));
            RayHelpers.DrawBitBoard(entry.Data);
            entry = new TtEntry(9, TtEntry.CreateData(2, 0, 0, 0, BitConverter.Int32BitsToSingle(-1), 0));
            RayHelpers.DrawBitBoard(entry.Data);*/
            Game testGame = new Game();
            /*testGame.InitializeBoard();
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
            Console.WriteLine(testGame.CurrentBoard.BoardToFen());*/
            testGame.CurrentBoard = Board.InitializeFromFen("r1qkbbr1/1p3p1n/p5p1/2N1B1Np/2P1K2P/1n3B2/R4P2/4Q1R1 w - - 14 2");
            //Console.WriteLine(testGame.CurrentBoard.GetRealMoves());
            //testGame.DrawBoard();
            //RayHelpers.DrawBitBoard(testGame.CurrentBoard.GetPositiveRayAttacks(testGame.CurrentBoard.AllPieces, 1, 9));
            //RayHelpers.DrawBitBoard(testGame.CurrentBoard.GetPositiveRayAttacks(testGame.CurrentBoard.AllPieces, 2, 9));
            //RayHelpers.DrawBitBoard(testGame.CurrentBoard.GetNegativeRayAttacks(testGame.CurrentBoard.AllPieces, RayHelpers.SouthWest, 63));
            Game game = new Game();
            game.InitializeBoard();
            //game.CurrentBoard = Board.InitializeFromFen("8/2B5/1k2K3/7b/6p1/6P1/8/8 b KQkq - 50 1");
            game.StartGame();
        }
    }
}
