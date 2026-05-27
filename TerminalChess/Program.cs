using System;
using TerminalChess;
using System.Threading;

class Program
{
    static Board board = new Board();
    static Move? move;
    static Position position;
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int moveCount = 0;
        while (true)
        {
            bool IsWhite = moveCount % 2 == 0;

            if (IsWhite)
            {
                board.ShowWhiteBoard();
                Console.Write("White move: ");
            }
            else
            {
                board.ShowBlackBoard();
                Console.Write("Black move: ");
            }

            string input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input))
            {
                ErrorMessage();
                continue;
            }

            move = ParseMove(input);

            if (move == null)
            {
                ErrorMessage();
                continue;
            }

            if (!board.ApplyMove(move, IsWhite))
            {
                ErrorMessage();
                continue;
            }

            moveCount++;

            if (board.IsItMate(!IsWhite))
            {
                if (!IsWhite)
                    board.ShowWhiteBoard();
                else
                    board.ShowBlackBoard();

                Console.WriteLine("GAME OVER");

                if (!IsWhite)
                    Console.WriteLine("Black Wins!");
                else
                    Console.WriteLine("White Wins!");

                Console.WriteLine($"Move count: {moveCount}");

                return;
            }
        }
    }

    static (int x, int y) ParseSquare(string input)
    {
        char file = input[0];
        char rank = input[1];
        int x;

        if (file >= 'a' && file <= 'z')
            x = file - 'a';
        else
            x = file - 'A';
        int y = 8 - (rank - '0');

        return (x, y);
    }

    static Move? ParseMove(string input)
    {
        string[] parts = input.Split();

        if (parts.Length != 2)
            return null;

        var (fromX, fromY) = ParseSquare(parts[0]);
        var (toX, toY) = ParseSquare(parts[1]);


        return new Move(
            new Position(fromX, fromY),
            new Position(toX, toY)
        );
    }

    static void ErrorMessage()
    {
        Console.WriteLine("Invalid move. Try again");
        Thread.Sleep(1500);
    }
}