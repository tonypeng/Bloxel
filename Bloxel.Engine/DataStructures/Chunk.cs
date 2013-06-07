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

 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.

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

namespace Bloxel.Engine.DataStructures
{
    /// <summary>
    /// Represents a section of blocks in the world.
    /// </summary>
    public class Chunk
    {
        // Very important.
        private Block[] _blocks;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        public Block[] Blocks { get { return _blocks; } }

        public VertexBuffer VertexBuffer { get { return _vertexBuffer; } set { _vertexBuffer = value; } }
        public IndexBuffer IndexBuffer { get { return _indexBuffer; } set { _indexBuffer = value; } }

        public Chunk(World world, int width, int height, int length)
        {
            Contract.Assert(width > 0);
            Contract.Assert(height > 0);
            Contract.Assert(length > 0);

            _blocks = new Block[width * height * length];

            _vertexBuffer = null;
            _indexBuffer = null;
        }
    }
}
