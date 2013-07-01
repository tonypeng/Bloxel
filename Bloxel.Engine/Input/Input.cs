/*
 * Bloxel - Input.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
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
