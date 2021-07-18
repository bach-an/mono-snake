using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.src
{
    public static class Constants
    {
        public const int SQUARE_DIM             = 30;
        public const int SQUARE_BORDER_OFFSET   = 2;
        public const int BORDER_WIDTH           = 15;
        public const int BORDER_HEIGHT          = 15;
        public static Color SQUARE_COLOR        = Color.White;
        public static Color SNAKE_COLOR         = Color.Gray;
        public static Color POINT_COLOR         = Color.Gold;
        public static Color SQUARE_BORDER_COLOR = Color.AliceBlue;
        public static Color BOARD_BORDER_COLOR  = Color.Black;
        public const float TIME_DELAY           = 0.35f;
        public static Vector2 BUTTON_SIZE       = new Vector2(300, 50);

        public const String PLAY_STRING         = "Play";
        public const String PLAY_AGAIN_STRING   = "Play Again?";
        public const String EXIT_STRING         = "Exit";
        public const String RESUME_STRING       = "Resume";
        public const String WIN_STRING          = "You Won!";
        public const String LOSE_STRING         = "You Lost!";

    }
}
