using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Snake.src
{
    enum SquareState { Board, Snake, Point };
    class Square
    {
        public Color color;
        private int X { get; set; }
        private int Y { get; set; }
        public Vector2 Coord { get; private set; }
        public SquareState State { get; private set; }

        public Square(int x, int y)
        {
            X = x;
            Y = y;
            Coord = new Vector2(x, y);
            color = Constants.SQUARE_COLOR;
            State = SquareState.Board;
        }

        public void MakeSnakeSquare()
        {
            State = SquareState.Snake;
            color = Constants.SNAKE_COLOR;
        }

        public void MakeBoardSquare()
        {
            State = SquareState.Board;
            color = Constants.SQUARE_COLOR;
        }

        public void MakePointSquare()
        {
            State = SquareState.Point;
            color = Constants.POINT_COLOR;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Square s = (Square)obj;
                return (X == s.X) && (Y == s.Y);
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return "X: " + X + " | Y: " + Y;

        }
    }
}
