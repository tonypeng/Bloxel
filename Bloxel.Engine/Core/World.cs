/*
 * Bloxel - World.cs
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

using Bloxel.Engine.Cameras;
using Bloxel.Engine.DataStructures;
using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.Core
{
    public class World
    {
        private EngineConfiguration _config;
        private IChunkManager _chunkManager;

        private CameraManager _cameraManager;

        public EngineConfiguration EngineConfiguration { get { return _config; } }

        public IChunkManager ChunkManager
        {
            get { return _chunkManager; }
            set { _chunkManager = value; }
        }

        public World(EngineConfiguration config, CameraManager cameraManager)
        {
            _config = config;

            _cameraManager = cameraManager;
        }

        public void Update(GameTime gameTime)
        {
            _chunkManager.Update(_cameraManager.MainCamera.Position);
        }

        public bool InBounds(Vector3 pos)
        {
            return InBounds(pos.X, pos.Y, pos.Z);
        }

        public bool InBounds(float x, float y, float z)
        {
            return x >= _config.ChunkWidth * _chunkManager.MinimumX && x < _config.ChunkWidth * (_chunkManager.MaximumX + 1) &&
                y >= _config.ChunkHeight * _chunkManager.MinimumY && y < _config.ChunkHeight * (_chunkManager.MaximumY + 1) &&
                z >= _config.ChunkLength * _chunkManager.MinimumZ && z < _config.ChunkLength * (_chunkManager.MaximumZ + 1);
        }

        public GridPoint PointAt(int x, int y, int z)
        {
            int cx = x / _config.ChunkWidth;
            int cy = y / _config.ChunkHeight;
            int cz = z / _config.ChunkLength;

            int lx = x % _config.ChunkWidth;
            int ly = y % _config.ChunkHeight;
            int lz = z % _config.ChunkLength;

            Chunk c = _chunkManager[cx, cy, cz];

            if (c == null || lx < 0 || ly < 0 || lz < 0)
                return new GridPoint(GridPoint.Empty);

            return c.Points[ArrayUtil.Convert3DTo1D(lx, ly, lz, _config.ChunkLength, _config.ChunkHeight)];
        }

        public void SetPoint(int x, int y, int z, GridPoint gp, bool suppressRebuild=false)
        {
            int cx = x / _config.ChunkWidth;
            int cy = y / _config.ChunkHeight;
            int cz = z / _config.ChunkLength;

            int lx = x % _config.ChunkWidth;
            int ly = y % _config.ChunkHeight;
            int lz = z % _config.ChunkLength;

            Chunk c = _chunkManager[cx, cy, cz];

            if (c == null)
                return;

            c.SetPointLocal(lx, ly, lz, gp, suppressRebuild);
        }

        public Chunk ChunkAt(float x, float y, float z)
        {
            return ChunkAt((int)x, (int)y, (int)z);
        }

        public Chunk ChunkAt(int x, int y, int z)
        {
            int cx = x / _config.ChunkWidth;
            int cy = y / _config.ChunkHeight;
            int cz = z / _config.ChunkLength;

            return _chunkManager[cx, cy, cz];
        }
    }
}
