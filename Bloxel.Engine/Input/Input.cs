/*
 * Bloxel - Input.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bloxel.Engine.Input
{
    public class Input
    {
        private static Input _instance = null;

        public static Input Get()
        {
            if (_instance == null)
                throw new NullReferenceException("Input must be created before it is used!");

            return _instance;
        }

        public static void Create(Game game)
        {
            _instance = new Input(game);
        }

        public static void Destroy()
        {
            _instance = null;
        }

        private Game _game;

        private KeyboardState _prevKeyboardState;
        private KeyboardState _currentKeyboardState;

        private MouseState _prevMouseState;
        private MouseState _currentMouseState;

        private Input(Game game)
        {
            _game = game;
        }

        public void Update()
        {
            _prevKeyboardState = _currentKeyboardState;

            _currentKeyboardState = Keyboard.GetState();

            _prevMouseState = _currentMouseState;

            _currentMouseState = Mouse.GetState();
        }

        public bool IsKeyDown(Keys key, bool isnew = false)
        {
            if (isnew)
            {
                return !_prevKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyDown(key) && _game.IsActive;
            }

            return _currentKeyboardState.IsKeyDown(key) && _game.IsActive;
        }

        public bool IsLeftMouseButtonDown(bool isnew = false)
        {
            if (isnew)
                return _prevMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed && _game.IsActive;

            return _currentMouseState.LeftButton == ButtonState.Pressed && _game.IsActive;
        }

        public bool IsMiddleMouseButtonDown(bool isnew = false)
        {
            if (isnew)
                return _prevMouseState.MiddleButton == ButtonState.Released && _currentMouseState.MiddleButton == ButtonState.Pressed && _game.IsActive;

            return _currentMouseState.MiddleButton == ButtonState.Pressed && _game.IsActive;
        }

        public bool IsRightMouseButtonDown(bool isnew = false)
        {
            if (isnew)
                return _prevMouseState.RightButton == ButtonState.Released && _currentMouseState.RightButton == ButtonState.Pressed && _game.IsActive;

            return _currentMouseState.RightButton == ButtonState.Pressed && _game.IsActive;
        }

        public int MouseXCoordinate()
        {
            return _currentMouseState.X;
        }

        public int MouseYCoordinate()
        {
            return _currentMouseState.Y;
        }
    }
}
