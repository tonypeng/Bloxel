/*
 * Bloxel - World.cs
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

        public void SetLight(int x, int y, int z, byte light)
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

            c.SetLightLocal(lx, ly, lz, light);
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
