using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TerminalChess;

namespace TerminalChess
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Move
    {
        public Position From;
        public Position To;

        public Move(Position from, Position to)
        {
            From = from;
            To = to;
        }

        
    }

    public abstract class Piece
    {
        public bool IsWhite { get; set; }
        public int MoveCount { get; set; }
        public abstract char PieceChar { get;}

        public abstract bool IsValidMove(
            Move move,
            Piece[,] board);

    }

    public class Pawn : Piece
    {
        public override char PieceChar => IsWhite? '♙' : '♟';
        public Pawn(bool whiteOrNot)
        {
            IsWhite = whiteOrNot;
        }
        public override bool IsValidMove(Move move, Piece[,] board)
        {
            int direction = IsWhite ? -1 : 1;
            int dy = move.To.Y - move.From.Y;
            int dx = move.To.X - move.From.X;
            Piece targetSquare = board[move.To.Y, move.To.X];

            if (move.To.Y > 7 || move.To.Y < 0)
                return false;
            if (move.To.X > 7 || move.To.X < 0)
                return false;

            if (IsDiagonal(dx))
            {
                if (dy != direction)
                    return false;
                if (targetSquare == null)
                    return false;
                if (targetSquare.IsWhite == IsWhite)
                    return false;
            }
            else
            {
                if (dx != 0)
                    return false;
                if (dy != direction && dy != direction * 2)
                    return false;
                if ((dy == direction * 2 && MoveCount != 0) || (dy == direction * 2 && board[move.To.Y - direction, move.To.X] != null))
                    return false;
                if (targetSquare != null)
                    return false;
            }

            return true;
        }
         
        private bool IsDiagonal(int dx)
        {
            if (dx == 1 || dx == -1)
                return true;
            return false;
        }
    }

    public class Knight : Piece
    {
        public override char PieceChar => IsWhite ? '♘' : '♞';
        public Knight(bool whiteOrNot)
        {
            IsWhite = whiteOrNot;
        }
        public override bool IsValidMove(Move move, Piece[,] board)
        {
            int dy = Math.Abs(move.To.Y - move.From.Y);
            int dx = Math.Abs(move.To.X - move.From.X);
            Piece target = board[move.To.Y, move.To.X];

            if (move.To.Y > 7 || move.To.Y < 0)
                return false;
            if (move.To.X > 7 || move.To.X < 0)
                return false;

            if (!((dy == 2 && dx == 1) || (dy == 1 && dx == 2)))
                return false;
            if (target != null && target.IsWhite == IsWhite)
                return false;

            return true;
        }
    }

    public class Bishop : Piece
    {
        public override char PieceChar => IsWhite ? '♗' : '♝';
        public Bishop(bool whiteOrNot)
        {
            IsWhite = whiteOrNot;
        }
        public override bool IsValidMove(Move move, Piece[,] board)
        {
            int dy = Math.Abs(move.To.Y - move.From.Y);
            int dx = Math.Abs(move.To.X - move.From.X);
            Piece target = board[move.To.Y, move.To.X];

            int stepY = move.To.Y > move.From.Y ? 1 : -1;
            int stepX = move.To.X > move.From.X ? 1 : -1;

            int y = move.From.Y + stepY;
            int x = move.From.X + stepX;

            if (move.To.Y > 7 || move.To.Y < 0)
                return false;
            if (move.To.X > 7 || move.To.X < 0)
                return false;

            if (dy != dx)
                return false;
            if (target != null && target.IsWhite == IsWhite)
                return false;

            while (y != move.To.Y && x != move.To.X)
            {
                if (board[y, x] != null)
                    return false;
                y += stepY;
                x += stepX;
            }

            return true;
        }
    }
    public class Rook : Piece
    {
        public override char PieceChar => IsWhite ? '♖' : '♜';
        public Rook(bool whiteOrNot)
        {
            IsWhite = whiteOrNot;
        }
        public override bool IsValidMove(Move move, Piece[,] board)
        {
            int dy = Math.Abs(move.To.Y - move.From.Y);
            int dx = Math.Abs(move.To.X - move.From.X);
            Piece target = board[move.To.Y, move.To.X];

            int stepY = move.To.Y > move.From.Y ? 1 : -1;
            int stepX = move.To.X > move.From.X ? 1 : -1;

            int y = move.From.Y;
            int x = move.From.X;

            if (move.To.Y > 7 || move.To.Y < 0)
                return false;
            if (move.To.X > 7 || move.To.X < 0)
                return false;

            if (dy != 0 && dx != 0)
                return false;
            if (target != null && target.IsWhite == IsWhite)
                return false;
            if (dy == 0 && dx == 0)
                return false;

            if (dy == 0)
            {
                x += stepX;
                while (x != move.To.X)
                {
                    if (board[move.From.Y, x] != null)
                        return false;

                    x += stepX;
                }
            }
            else if (dx == 0)
            {
                y += stepY;
                while (y != move.To.Y)
                {
                    if (board[y, move.From.X] != null)
                        return false;

                    y += stepY;
                }
            }

            return true;
        }
    }
    
    public class Queen : Piece
    {
        public override char PieceChar => IsWhite ? '♕' : '♛';
        public Queen(bool whiteOrNot)
        {
            IsWhite = whiteOrNot;
        }
        public override bool IsValidMove(Move move, Piece[,] board)
        {
            int dy = Math.Abs(move.To.Y - move.From.Y);
            int dx = Math.Abs(move.To.X - move.From.X);
            Piece target = board[move.To.Y, move.To.X];

            int stepY = move.To.Y > move.From.Y ? 1 : -1;
            int stepX = move.To.X > move.From.X ? 1 : -1;

            int y = move.From.Y;
            int x = move.From.X;


            if (move.To.Y > 7 || move.To.Y < 0)
                return false;
            if (move.To.X > 7 || move.To.X < 0)
                return false;

            if (target != null && target.IsWhite == IsWhite)
                return false;
            if (dy == 0 && dx == 0)
                return false;

            if (dy == dx)
            {
                y += stepY;
                x += stepX;

                while (y != move.To.Y && x != move.To.X)
                {
                    if (board[y, x] != null)
                        return false;
                    y += stepY;
                    x += stepX;
                }

                return true;
            }
            else
            {
                if (dy != 0 && dx != 0)
                    return false;

                if (dy == 0)
                {
                    x += stepX;

                    while (x != move.To.X)
                    {
                        if (board[move.From.Y, x] != null)
                            return false;
                        x += stepX;
                    }
                }
                else if (dx == 0)
                {
                    y += stepY;

                    while (y != move.To.Y)
                    {
                        if (board[y, move.From.X] != null)
                            return false;
                        y += stepY;
                    }
                }

                return true;
            }
        }
    }

    public class King : Piece
    {
        public override char PieceChar => IsWhite ? '♔' : '♚';
        public King(bool whiteOrNot)
        {
            IsWhite = whiteOrNot;
        }
        public override bool IsValidMove(Move move, Piece[,] board)
        {
            int dy = Math.Abs(move.To.Y - move.From.Y);
            int dx = Math.Abs(move.To.X - move.From.X);
            Piece target = board[move.To.Y, move.To.X];

            if (dy == 0 && dx == 0)
                return false;
            if (dy > 1 || dx > 1)
                return false;
            if (target != null && target.IsWhite == IsWhite)
                return false;


            return true;
        }
    }
}
