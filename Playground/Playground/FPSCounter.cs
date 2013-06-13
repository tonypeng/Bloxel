using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Playground
{
    public class FPSCounter
    {
        public Game GameInstance;

        TimeSpan elapsedTime;

        int frameRate = 0, frameCounter = 0;

        SpriteBatch spriteBatch;

        SpriteFont _font;

        public FPSCounter(Game game, SpriteFont spriteFont)
        {
            GameInstance = game;

            _font = spriteFont;
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(GameInstance.GraphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime = TimeSpan.Zero;

                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public void Draw()
        {
            frameCounter++;

            string fps = frameRate.ToString();

            spriteBatch.Begin();

            Vector2 dim = _font.MeasureString(fps);

            spriteBatch.DrawString(_font, fps, new Vector2(0, GameInstance.GraphicsDevice.Viewport.Height - dim.Y - 1), Color.Black);
            spriteBatch.DrawString(_font, fps, new Vector2(0, GameInstance.GraphicsDevice.Viewport.Height - dim.Y), Color.Yellow);

            spriteBatch.End();
        }
    }
}
