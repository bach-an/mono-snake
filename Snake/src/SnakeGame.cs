using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Threading;

namespace Snake.src
{
    enum GameState { MainMenu, GamePlay, Paused, Won, Lost }; 
    public class SnakeGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Board _board;
        private Vector2 _startPos;
        private Direction _direction;
        private GameState _state;


        Texture2D drawnBorder, boardBorder;

        // Menu Buttons
        Texture2D button, buttonBorder;

        private Vector2 startButtonPos;
        private Vector2 exitButtonPos;
        private Vector2 resumeButtonPos;
        private Vector2 losePos;
        private Vector2 winPos;

        MouseState mouseState;
        MouseState previousMouseState;

        // Font
        SpriteFont buttonFont;


        private float timer = Constants.TIME_DELAY;
        public SnakeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _board = new Board(Constants.BORDER_WIDTH, Constants.BORDER_HEIGHT, GraphicsDevice);
            _startPos.X = (_graphics.PreferredBackBufferWidth / 2 - Constants.BORDER_WIDTH * Constants.SQUARE_DIM / 2);
            _startPos.Y = (_graphics.PreferredBackBufferHeight / 2 - Constants.BORDER_HEIGHT * Constants.SQUARE_DIM / 2);
            _direction = Direction.Still;
            _state = GameState.MainMenu;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);



            button = new Texture2D(GraphicsDevice, 1, 1);
            button.SetData(new[] { Color.White });

            startButtonPos = new Vector2(_graphics.PreferredBackBufferWidth / 2 - Constants.BUTTON_SIZE.X / 2, _graphics.PreferredBackBufferHeight / 2 - 100);
            exitButtonPos = new Vector2(_graphics.PreferredBackBufferWidth / 2 - Constants.BUTTON_SIZE.X / 2, _graphics.PreferredBackBufferHeight / 2);

            resumeButtonPos = startButtonPos;

            drawnBorder = new Texture2D(GraphicsDevice, Constants.SQUARE_DIM + Constants.SQUARE_BORDER_OFFSET, Constants.SQUARE_DIM + Constants.SQUARE_BORDER_OFFSET);
            drawnBorder.CreateBorder(1, Constants.SQUARE_BORDER_COLOR);

            buttonBorder = new Texture2D(GraphicsDevice, (int)Constants.BUTTON_SIZE.X + Constants.SQUARE_BORDER_OFFSET,
                                                           (int)Constants.BUTTON_SIZE.Y + Constants.SQUARE_BORDER_OFFSET);


            boardBorder = new Texture2D(GraphicsDevice, (Constants.SQUARE_DIM * Constants.BORDER_WIDTH) + Constants.SQUARE_BORDER_OFFSET,
                                                        (Constants.SQUARE_DIM * Constants.BORDER_HEIGHT) + Constants.SQUARE_BORDER_OFFSET);


            buttonFont = Content.Load<SpriteFont>("buttonFont");
            losePos = new Vector2(startButtonPos.X + Constants.BUTTON_SIZE.X / 2 - (buttonFont.MeasureString(Constants.LOSE_STRING) / 2).X, startButtonPos.Y - 100);
            winPos  = new Vector2(startButtonPos.X + Constants.BUTTON_SIZE.X / 2 - (buttonFont.MeasureString(Constants.WIN_STRING) / 2).X, startButtonPos.Y - 100);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime deltaTime)
        {

            base.Update(deltaTime);
            mouseState = Mouse.GetState();
            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released) 
            {
                MouseClicked(mouseState.X, mouseState.Y);
            }
            previousMouseState = mouseState;
            switch(_state)
            {
                case GameState.MainMenu:
                    UpdateMenu(deltaTime);
                    break;
                case GameState.GamePlay:
                    UpdateGamePlay(deltaTime);
                    break;
                case GameState.Paused:
                    UpdateMenu(deltaTime);
                    break;
                case GameState.Lost:
                    UpdateMenu(deltaTime);
                    break;
                case GameState.Won:
                    UpdateMenu(deltaTime);
                    break;
            }
        }

        void UpdateMenu(GameTime deltaTime)
        {
            IsMouseVisible = true;
        }

        void UpdateGamePlay(GameTime deltaTime)
        {
            IsMouseVisible = false;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                _state = GameState.Paused;
            }

            if (_board.CurrentState == BoardState.Lost)
            {
                _state = GameState.Lost;
            }

            if (_board.IsGameWon())
            {
                _state = GameState.Won;
            }

            KeyboardState ks = Keyboard.GetState();
            Direction newDirection = _direction;

            if (ks.IsKeyDown(Keys.W) || ks.IsKeyDown(Keys.Up))
            {
                newDirection = Direction.Up;
            }
            if (ks.IsKeyDown(Keys.S) || ks.IsKeyDown(Keys.Down))
            {
                newDirection = Direction.Down;
            }
            if (ks.IsKeyDown(Keys.A) || ks.IsKeyDown(Keys.Left))
            {
                newDirection = Direction.Left;
            }
            if (ks.IsKeyDown(Keys.D) || ks.IsKeyDown(Keys.Right))
            {
                newDirection = Direction.Right;
            }

            float elapsed = (float)deltaTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;

            // If the direction has changed, move the snake instantly
            // Makes everything feel smoother
            if (newDirection != _direction)
            {
                _direction = newDirection;
                _board.MoveSnake(_direction);
                timer = (float)(Constants.TIME_DELAY - _board.PointsScored() * 0.01);

            }
            else
            {
                if (timer < 0)
                {
                    _direction = newDirection;
                    _board.MoveSnake(_direction);
                    timer = (float)(Constants.TIME_DELAY - _board.PointsScored() * 0.01);
                }
            }
        }

        protected override void Draw(GameTime deltaTime)
        {
            base.Draw(deltaTime);

            switch (_state)
            {
                case GameState.MainMenu:
                    DrawMainMenu(deltaTime);
                    break;
                case GameState.GamePlay:
                    DrawGamePlay(deltaTime);
                    break;
                case GameState.Paused:
                    DrawPaused(deltaTime);
                    break;
                case GameState.Lost:
                    DrawLost(deltaTime);
                    break;
                case GameState.Won:
                    DrawWon(deltaTime);
                    break;
            }
        }

        void DrawPaused(GameTime deltaTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            DrawButton(Constants.RESUME_STRING, resumeButtonPos);
            DrawButton(Constants.EXIT_STRING, exitButtonPos);
            _spriteBatch.End();
        }
    
        void DrawMainMenu(GameTime deltaTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            DrawButton(Constants.PLAY_STRING, startButtonPos);
            DrawButton(Constants.EXIT_STRING, exitButtonPos);
            _spriteBatch.End();
        }
        
        void DrawWon(GameTime deltaTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            DrawButton(Constants.PLAY_AGAIN_STRING, startButtonPos);
            DrawButton(Constants.EXIT_STRING, exitButtonPos);
            _spriteBatch.End();
        }

        void DrawLost(GameTime deltaTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            
            _spriteBatch.DrawString(buttonFont, Constants.LOSE_STRING, losePos, Color.Red);

            DrawButton(Constants.PLAY_AGAIN_STRING, startButtonPos);
            DrawButton(Constants.EXIT_STRING, exitButtonPos);

            _spriteBatch.End();
        }

        void DrawButton(string buttonText, Vector2 buttonPos)
        {
            Vector2 textMiddlePoint = buttonFont.MeasureString(buttonText) / 2;

            _spriteBatch.Draw(button, new Rectangle((int)buttonPos.X, (int)buttonPos.Y, (int)Constants.BUTTON_SIZE.X, (int)Constants.BUTTON_SIZE.Y), Color.White);

            _spriteBatch.DrawString(buttonFont, buttonText, new Vector2(buttonPos.X + Constants.BUTTON_SIZE.X / 2 - textMiddlePoint.X, buttonPos.Y), Color.Gray);
            buttonBorder.CreateBorder(1, Constants.BOARD_BORDER_COLOR);
            _spriteBatch.Draw(buttonBorder, buttonPos, Constants.BOARD_BORDER_COLOR);
        }

        void MouseClicked(int x, int y)
        {
            Rectangle mouseClickRect   = new Rectangle(x, y, 10, 10);
            Rectangle bottomButtonRect = new Rectangle((int)exitButtonPos.X, (int)exitButtonPos.Y, (int)Constants.BUTTON_SIZE.X, (int)Constants.BUTTON_SIZE.Y);
            Rectangle topButtonRect    = new Rectangle((int)startButtonPos.X, (int)startButtonPos.Y, (int)Constants.BUTTON_SIZE.X, (int)Constants.BUTTON_SIZE.Y);
            if (mouseClickRect.Intersects(bottomButtonRect))
            {
                Exit();
            }
            switch (_state)
            {
                case GameState.MainMenu:
                    if (mouseClickRect.Intersects(topButtonRect))
                    {
                        _state = GameState.GamePlay;
                    }
                    break;
                case GameState.Paused:
                    if (mouseClickRect.Intersects(topButtonRect))
                    {
                        _state = GameState.GamePlay;
                    }
                    break;
                case GameState.Lost:
                case GameState.Won:
                    if(mouseClickRect.Intersects(topButtonRect))
                    {
                        _board = null;
                        _board = new Board(Constants.BORDER_WIDTH, Constants.BORDER_HEIGHT, GraphicsDevice);
                        _direction = Direction.Still;
                        _state = GameState.GamePlay;
                    }
                    break;
            }
        }

        void DrawGamePlay(GameTime deltaTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            DrawBoard(_startPos);
            DrawBorder(_startPos);

            _spriteBatch.End();
        }

        private void DrawBoard(Vector2 drawPos)
        {
            Vector2 startPos = drawPos;
            for (int y = 0; y < _board.Height; ++y)
            {
                for (int x = 0; x < _board.Width; ++x)
                {
                    Square sq = _board.GetSquare(x, y);
                    Texture2D drawnSquare = _board.GetTexture2D(x, y);
                    _board.SetTextureColor(x, y);

                    _spriteBatch.Draw(drawnSquare, new Rectangle((int)drawPos.X, (int)drawPos.Y, Constants.SQUARE_DIM, Constants.SQUARE_DIM), sq.color);
                    _spriteBatch.Draw(drawnBorder, drawPos, Constants.SQUARE_BORDER_COLOR);

                    drawPos.X += Constants.SQUARE_DIM;
                }
                drawPos.X = startPos.X;
                drawPos.Y += Constants.SQUARE_DIM;
            }
            drawnBorder.CreateBorder(1, Constants.SQUARE_BORDER_COLOR);
        }

        // Draw a border around the entire board
        private void DrawBorder(Vector2 drawPos)
        {
            boardBorder.CreateBorder(1, Constants.BOARD_BORDER_COLOR);
            _spriteBatch.Draw(boardBorder, drawPos, Constants.BOARD_BORDER_COLOR);
        }
    }
}
