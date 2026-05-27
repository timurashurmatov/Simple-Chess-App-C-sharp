using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TerminalChess
{
    public class Board
    {
        private Piece?[,] BoardArr =
        {
            {new Rook(false), new Knight(false), new Bishop(false), new Queen(false), new King(false), new Bishop(false), new Knight(false), new Rook(false) },
            {new Pawn(false), new Pawn(false), new Pawn(false), new Pawn(false), new Pawn(false), new Pawn(false), new Pawn(false), new Pawn(false)},
            {null, null, null, null, null, null, null, null },
            {null, null, null, null, null, null, null, null },
            {null, null, null, null, null, null, null, null },
            {null, null, null, null, null, null, null, null },
            {new Pawn(true), new Pawn(true), new Pawn(true), new Pawn(true), new Pawn(true), new Pawn(true), new Pawn(true), new Pawn(true)},
            {new Rook(true), new Knight(true), new Bishop(true), new Queen(true), new King(true), new Bishop(true), new Knight(true), new Rook(true) }
        };

        public void ShowWhiteBoard()
        {
            int rank = 8;
            char file = 'a';
            Console.Clear();

            Console.Write("\n-------------------\n");
            Console.Write("   ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file++}|");
            }
            Console.Write("\n-------------------\n");

            for (int i = 0; i < 8; i++)
            {
                Console.Write($"{rank--} ");

                for (int j = 0; j < 8; j++)
                {
                    if (j == 0)
                        Console.Write("|");

                    if (BoardArr[i, j] == null)
                        Console.Write(" |");
                    else
                    {
                        Console.Write($"{BoardArr[i, j].PieceChar}|");
                    }
                }
                Console.Write("\n-------------------\n");
            }

            file = 'a';
            Console.Write("   ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file++}|");
            }
            Console.Write("\n-------------------\n");
        }

        public void ShowBlackBoard()
        {
            int rank = 1;
            char file = 'h';
            Console.Clear();

            Console.Write("\n-------------------\n");
            Console.Write("   ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file--}|");
            }
            Console.Write("\n-------------------\n");

            for (int i = 7; i >= 0; i--)
            {
                Console.Write($"{rank++} ");
                for (int j = 7; j >= 0; j--)
                {
                    if (j == 7)
                        Console.Write("|");

                    if (BoardArr[i, j] == null)
                        Console.Write(" |");
                    else
                    {
                        Console.Write($"{BoardArr[i, j].PieceChar}|");
                    }
                }
                Console.Write("\n-------------------\n");
            }

            file = 'h';
            Console.Write("   ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file--}|");
            }
            Console.Write("\n-------------------\n");
        }

        public bool ApplyMove(Move move, bool whiteOrNot)
        {
            Position? kingPos;
            Piece? movingPiece = BoardArr[move.From.Y, move.From.X];
            Piece? temp = BoardArr[move.To.Y, move.To.X];

            if (movingPiece == null)
                return false;
            if (movingPiece.IsWhite != whiteOrNot)
                return false;
            if (!movingPiece.IsValidMove(move, BoardArr))
                return false;

            BoardArr[move.To.Y, move.To.X] = movingPiece;
            BoardArr[move.From.Y, move.From.X] = null;

            if (movingPiece is King)
                kingPos = new Position(move.To.X, move.To.Y);
            else
                kingPos = FindKing(movingPiece.IsWhite, BoardArr);

            if (IsKingInCheck(kingPos, BoardArr[move.To.Y, move.To.X].IsWhite, BoardArr))
            {
                BoardArr[move.From.Y, move.From.X] = BoardArr[move.To.Y, move.To.X];
                BoardArr[move.To.Y, move.To.X] = temp;

                return false;
            }

            BoardArr[move.To.Y, move.To.X].MoveCount++;
            return true;
        }

        public bool IsItMate(bool whiteOrNot)
        {
            Position? kingPos = FindKing(whiteOrNot, BoardArr);

            if (!IsKingInCheck(kingPos, whiteOrNot, BoardArr))
                return false;

            for (int fromY = 0; fromY < 8; fromY++)
            {
                for (int fromX = 0; fromX < 8; fromX++)
                {
                    Piece piece = BoardArr[fromY, fromX];

                    if (piece == null)
                        continue;
                    if (piece.IsWhite != whiteOrNot)
                        continue;

                    for (int toY = 0; toY < 8; toY++)
                    {
                        for (int toX = 0; toX < 8; toX++)
                        {
                            Move move = new Move(
                                new Position(fromX, fromY),
                                new Position(toX, toY)
                            );

                            if (CanMove(move))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool CanMove(Move move)
        {
            Piece?[,] copyBoard = new Piece[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    copyBoard[i, j] = BoardArr[i, j];
                }
            }

            Piece movingPiece = copyBoard[move.From.Y, move.From.X];
            Piece temp = copyBoard[move.To.Y, move.To.X];
            Position kingPos;

            if (!movingPiece.IsValidMove(move, copyBoard))
                return false;

            copyBoard[move.To.Y, move.To.X] = movingPiece;
            copyBoard[move.From.Y, move.From.X] = null;

            if (movingPiece is King)
                kingPos = new Position(move.To.X, move.To.Y);
            else
                kingPos = FindKing(movingPiece.IsWhite, copyBoard);
            if (IsKingInCheck(kingPos, movingPiece.IsWhite, copyBoard))
            {
                copyBoard[move.From.Y, move.From.X] = copyBoard[move.To.Y, move.To.X];
                copyBoard[move.To.Y, move.To.X] = temp;
                return false;
            }

            copyBoard[move.From.Y, move.From.X] = copyBoard[move.To.Y, move.To.X];
            copyBoard[move.To.Y, move.To.X] = temp;

            return true;
        }

        public Position? FindKing(bool white, Piece[,] board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] is King && board[i, j].IsWhite == white)
                    {
                        return new Position(j, i);
                    }
                }
            }


            return null;
        }

        public bool IsKingInCheck(Position position, bool white, Piece[,] board)
        {
            Move move;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == null)
                        continue;
                    if (board[i, j].IsWhite == white)
                        continue;
                    move = new Move(new Position(j, i), position);

                    if (board[i, j].IsValidMove(move, board))
                        return true;
                }
            }


            return false;
        }
    }
}
