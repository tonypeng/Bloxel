/*
 * Bloxel - Chunk.cs
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Bloxel.Engine.Core;
using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.DataStructures
{
    /// <summary>
    /// Represents a section of blocks in the world.
    /// </summary>
    public class Chunk
    {
        // Very important.
        private GridPoint[] _points;

        private Vector3I _position;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private int _width, _height, _length;

        public GridPoint[] Points { get { return _points; } }

        public Vector3I Position { get { return _position; } }

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public int Length { get { return _length; } }

        public VertexBuffer VertexBuffer { get { return _vertexBuffer; } set { _vertexBuffer = value; } }
        public IndexBuffer IndexBuffer { get { return _indexBuffer; } set { _indexBuffer = value; } }

        public object GraphicsSync = new object();

        public Chunk(World world, Vector3I position, int width, int height, int length)
        {
            Contract.Assert(width > 0);
            Contract.Assert(height > 0);
            Contract.Assert(length > 0);

            _position = position;

            _width = width;
            _height = height;
            _length = length;

            _points = new GridPoint[width * height * length];

            _vertexBuffer = null;
            _indexBuffer = null;
        }

        public void SetPoint(int x, int y, int z, GridPoint b)
        {
            _points[ArrayUtil.Convert3DTo1D(x, y, z, _length, _height)] = b;
        }

        public GridPoint PointAt(int x, int y, int z)
        {
            return _points[ArrayUtil.Convert3DTo1D(x, y, z, _length, _height)];
        }
    }
}
