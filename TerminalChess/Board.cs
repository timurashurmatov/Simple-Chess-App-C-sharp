using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TerminalChess
{
    public class Board
    {
        public Piece?[,] BoardArr =
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

            Console.Write("\n------------------\n");
            Console.Write("  ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file++}|");
            }
            Console.Write("\n------------------\n");

            for (int i = 0; i < 8; i++)
            {
                Console.Write($"{rank--}");

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
                Console.Write("\n------------------\n");
            }

            file = 'a';
            Console.Write("  ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file++}|");
            }
            Console.Write("\n------------------\n");
        }

        public void ShowBlackBoard()
        {
            int rank = 1;
            char file = 'h';
            Console.Clear();

            Console.Write("\n------------------\n");
            Console.Write("  ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file--}|");
            }
            Console.Write("\n------------------\n");

            for (int i = 7; i >= 0; i--)
            {
                Console.Write($"{rank++}");
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
                Console.Write("\n------------------\n");
            }

            file = 'h';
            Console.Write("  ");
            for (int f = 0; f < 8; f++)
            {
                Console.Write($"{file--}|");
            }
            Console.Write("\n------------------\n");
        }

        public bool ApplyMove(Move move, bool white, int moveCount)
        {
            Position? kingPos;
            Piece? movingPiece = BoardArr[move.From.Y, move.From.X];
            Piece? temp = BoardArr[move.To.Y, move.To.X];
            int direction = white ? -1 : 1;
            bool EnPassant = false;

            if (movingPiece.IsWhite && moveCount % 2 != 0)
                return false;
            else if (!movingPiece.IsWhite && moveCount % 2 == 0)
                return false;

            if (movingPiece == null)
                return false;
            if (movingPiece.IsWhite != white)
                return false;
            if (movingPiece.IsValidMove(move, BoardArr) == false)
                return false;
            if (movingPiece.IsValidMove(move, BoardArr) == null)
            {
                EnPassant = true;
                temp = BoardArr[move.To.Y - direction, move.To.X];
                BoardArr[move.To.Y - direction, move.To.X] = null;
            }

            BoardArr[move.To.Y, move.To.X] = movingPiece;
            BoardArr[move.From.Y, move.From.X] = null;

            if (movingPiece is King)
                kingPos = new Position(move.To.X, move.To.Y);
            else
                kingPos = FindKing(movingPiece.IsWhite, BoardArr);

            if (IsKingInCheck(kingPos, BoardArr[move.To.Y, move.To.X].IsWhite, BoardArr))
            {
                BoardArr[move.From.Y, move.From.X] = BoardArr[move.To.Y, move.To.X];
                if (EnPassant)
                {
                    BoardArr[move.To.Y - direction, move.To.X] = temp;
                }

                BoardArr[move.To.Y, move.To.X] = temp;
                return false;
            }

            BoardArr[move.To.Y, move.To.X].MoveCount++;
            return true;
        }

        public bool IsItMate(bool white)
        {
            Position? kingPos = FindKing(white, BoardArr);

            if (!IsKingInCheck(kingPos, white, BoardArr))
                return false;

            for (int fromY = 0; fromY < 8; fromY++)
            {
                for (int fromX = 0; fromX < 8; fromX++)
                {
                    Piece piece = BoardArr[fromY, fromX];

                    if (piece == null)
                        continue;
                    if (piece.IsWhite != white)
                        continue;

                    for (int toY = 0; toY < 8; toY++)
                    {
                        for (int toX = 0; toX < 8; toX++)
                        {
                            Move move = new Move(
                                new Position(fromX, fromY),
                                new Position(toX, toY)
                            );

                            if (CanMove(move, white))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool CanMove(Move move, bool white)
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
            bool EnPassant = false;
            int direction = white ? -1 : 1;

            if (movingPiece.IsValidMove(move, copyBoard) == false)
                return false;
            if (movingPiece.IsValidMove(move, copyBoard) == null)
            {
                EnPassant = true;
                temp = copyBoard[move.To.Y - direction, move.To.X];
                copyBoard[move.To.Y - direction, move.To.X] = null;
            }

            copyBoard[move.To.Y, move.To.X] = movingPiece;
            copyBoard[move.From.Y, move.From.X] = null;

            if (movingPiece is King)
                kingPos = new Position(move.To.X, move.To.Y);
            else
                kingPos = FindKing(movingPiece.IsWhite, copyBoard);
            if (IsKingInCheck(kingPos, movingPiece.IsWhite, copyBoard))
            {
                copyBoard[move.From.Y, move.From.X] = copyBoard[move.To.Y, move.To.X];
                if (EnPassant)
                {
                    copyBoard[move.To.Y - direction, move.To.X] = temp;
                }
                else
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

                    if (board[i, j].IsValidMove(move, board) == true || board[i, j].IsValidMove(move, board) == null)
                        return true;
                }
            }


            return false;
        }


        public bool Castle(string input, bool white)
        {
            int y = white ? 7 : 0;
            if (input.Equals("O-O"))
            {
                var king = BoardArr[y, 4];
                var rook = BoardArr[y, 7];

                if (king is not King || rook is not Rook)
                    return false;
                if (king.IsWhite != white || rook.IsWhite != white)
                    return false;
                if (king.MoveCount != 0 || rook.MoveCount != 0)
                    return false;

                if (BoardArr[y, 5] != null || BoardArr[y, 6] != null)
                    return false;

                if (IsKingInCheck(new Position(4, y), white, BoardArr))
                    return false;

                Piece?[,] copyBoard = new Piece?[8, 8];
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        copyBoard[j, i] = BoardArr[j, i];
                    }
                }

                copyBoard[y, 4] = null;
                copyBoard[y, 5] = king;
                if (IsKingInCheck(new Position(5, y), white, BoardArr))
                    return false;

                copyBoard[y, 5] = null;
                copyBoard[y, 6] = king;
                if (IsKingInCheck(new Position(6, y), white, BoardArr))
                    return false;

                BoardArr[y, 4] = null;
                BoardArr[y, 5] = rook;
                BoardArr[y, 6] = king;
                BoardArr[y, 7] = null;
            }
            else if (input.Equals("O-O-O"))
            {
                var king = BoardArr[y, 4];
                var rook = BoardArr[y, 0];

                if (king is not King || rook is not Rook)
                    return false;
                if (king.IsWhite != white || rook.IsWhite != white)
                    return false;
                if (king.MoveCount != 0 || rook.MoveCount != 0)
                    return false;

                if (BoardArr[y, 3] != null || BoardArr[y, 2] != null || BoardArr[y, 1] != null)
                    return false;

                if (IsKingInCheck(new Position(4, y), white, BoardArr))
                    return false;

                Piece?[,] copyBoard = new Piece?[8, 8];
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        copyBoard[j, i] = BoardArr[j, i];
                    }
                }

                copyBoard[y, 4] = null;
                copyBoard[y, 3] = king;
                if (IsKingInCheck(new Position(3, y), white, BoardArr))
                    return false;

                copyBoard[y, 3] = null;
                copyBoard[y, 2] = king;
                if (IsKingInCheck(new Position(2, y), white, BoardArr))
                    return false;

                BoardArr[y, 4] = null;
                BoardArr[y, 3] = rook;
                BoardArr[y, 2] = king;
                BoardArr[y, 1] = null;
                BoardArr[y, 0] = null;
            }
            else
                return false;

            return true;
        }

        public bool BackRankPawn(bool isWhite)
        {
            int y = isWhite ? 0 : 7;

            for (int x = 0; x < 8; x++)
            {
                if (BoardArr[y, x] is Pawn)
                    return true;
            }

            return false;
        }

        public void Promote(Piece Promotion, bool isWhite, Position pos)
        {
            BoardArr[pos.Y, pos.X] = Promotion;
        }
    }
}
