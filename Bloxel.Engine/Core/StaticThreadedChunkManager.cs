/*
 * Bloxel - StaticThreadedChunkManager.cs
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

using Bloxel.Engine.DataStructures;
using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.Core
{
    /// <summary>
    /// Basic chunk manager that does not dynamically load/unload chunks.
    /// </summary>
    public class StaticThreadedChunkManager : IChunkManager
    {
        private IChunkSystem _chunkSystem;

        private int _worldWidth, _worldHeight, _worldLength;
        private Chunk[] _chunks;

        public IChunkSystem ChunkSystem { get { return _chunkSystem; } set { _chunkSystem = value; } }

        public Chunk this[int x, int y, int z] { get { return Get(x, y, z); } }

        public StaticThreadedChunkManager(int worldWidth, int worldHeight, int worldLength)
        {
            Contract.Assert(worldWidth > 0);
            Contract.Assert(worldHeight > 0);
            Contract.Assert(worldLength > 0);

            _worldWidth = worldWidth;
            _worldHeight = worldHeight;
            _worldLength = worldLength;

            _chunks = new Chunk[worldWidth * worldHeight * worldLength];

            _chunkSystem = null;
        }

        public void Update(Vector3 cameraPosition)
        {
            Contract.Assert(_chunkSystem != null);
        }

        public void Render()
        {
            Contract.Assert(_chunkSystem != null);
        }

        public Chunk Get(int x, int y, int z)
        {
            int index = ArrayUtil.Convert3DTo1D(x, y, z, _worldLength, _worldHeight);

            if (index < 0 || index >= _chunks.Length)
                return null;

            return _chunks[index];
        }
    }
}
