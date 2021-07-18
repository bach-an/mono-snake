using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.src
{
    enum Direction { Up, Down, Left, Right, Still };
    enum BoardState { InProgress, Lost, Won };
    class Board
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private List<List<Square>> Squares { get; set; }
        private List<List<Texture2D>> SquareTextures { get; set; }
        private readonly Random rd = new Random();
        private List<Square> Snake { get; set; }
        public BoardState CurrentState { get; private set;  }

        public Board(int boardWidth, int boardHeight, GraphicsDevice gd)
        {
            Debug.WriteLine("Board constructed");
            Width = boardWidth;
            Height = boardHeight;
            Squares = new List<List<Square>>();
            SquareTextures = new List<List<Texture2D>>();
            Snake = new List<Square>();
            List<Square> row = new List<Square>();
            List<Texture2D> textureRow = new List<Texture2D>();
            for (int h = 0; h < boardHeight; h++)
            {
                for (int w = 0; w < boardWidth; w++)
                {
                    row.Add(new Square(w, h));
                    textureRow.Add(new Texture2D(gd, 1, 1));
                }
                SquareTextures.Add(textureRow);
                Squares.Add(row);
                row = new List<Square>();
                textureRow = new List<Texture2D>();
            }
            int randX = rd.Next(boardWidth);
            int randY = rd.Next(boardHeight);
            AddSnakeSquare(randX, randY);
            GeneratePoint();
            CurrentState = BoardState.InProgress;
            
        }

        public void MoveSnake(Direction dir)
        {
            int firstSquarePosX = (int)Snake[0].Coord.X;
            int firstSquarePosY = (int)Snake[0].Coord.Y;
            int newX = -1;
            int newY = -1;
            Square lastSnakeSquare = Snake[^1];
            switch (dir) {
                case Direction.Up:
                    newX = firstSquarePosX;
                    newY = firstSquarePosY - 1;
                    break;
                case Direction.Down:
                    newX = firstSquarePosX;
                    newY = firstSquarePosY + 1;
                    break;
                case Direction.Left:
                    newX = firstSquarePosX - 1;
                    newY = firstSquarePosY;
                    break;
                case Direction.Right:
                    newX = firstSquarePosX + 1;
                    newY = firstSquarePosY;
                    break;
                default:
                    newX = firstSquarePosX;
                    newY = firstSquarePosY;
                    break;
            }
            if (DoesCollide(newX, newY, dir))
            {
                Debug.WriteLine("Game Lost!");
                CurrentState = BoardState.Lost;
                return;
            }
            if (Squares[newY][newX].State == SquareState.Point)
            {
                AddSnakeSquare(newX, newY);
                GeneratePoint();
                return;
            }
            else
            {
                RemoveSnakeSquare((int)lastSnakeSquare.Coord.X, (int)lastSnakeSquare.Coord.Y);
                AddSnakeSquare(newX, newY);
                return;
            }
        }

        public Square GetSquare(int x, int y)
        {
            return Squares[y][x];
        }

        public Texture2D GetTexture2D(int x, int y)
        {
            return SquareTextures[y][x];
        }

        public void SetTextureColor(int x, int y)
        {
            GetTexture2D(x, y).SetData(new[] { GetSquare(x, y).color });
        }

        public void GeneratePoint()
        {
            List<Square> flattenedBoard = Squares.SelectMany(s => s).ToList();
            List<Square> boardSquares = flattenedBoard.FindAll(s => s.State == SquareState.Board);
            Square randomSquare = boardSquares[rd.Next(boardSquares.Count)];
            randomSquare.MakePointSquare();
        }

        private void AddSnakeSquare(int x, int y)
        {
            Squares[y][x].MakeSnakeSquare();
            Snake.Insert(0, Squares[y][x]);
        }

        private void RemoveSnakeSquare(int x, int y)
        {
            Squares[y][x].MakeBoardSquare();
            Snake.Remove(Squares[y][x]);
        }

        private bool DoesCollide(int x, int y, Direction d)
        {
            bool inBounds = 0 <= x && x < Width && 0 <= y && y < Height;
            if (inBounds)
            {
                bool snakeSquare = Squares[y][x].State == SquareState.Snake && d != Direction.Still;
                return snakeSquare;
            }
            return true;
        }

        public int PointsScored()
        {
            return Snake.Count - 1;
        }

        public bool IsGameWon()
        {
            List<Square> flattenedBoard = Squares.SelectMany(s => s).ToList();
            return Snake.Count == flattenedBoard.Count - 1; 
        }
    }
}
