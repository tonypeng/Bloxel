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
        private World _world;

        private IChunkGenerator _chunkGenerator;
        private IChunkSystem _chunkSystem;

        private int _worldWidth, _worldHeight, _worldLength;
        private Chunk[] _chunks;

        private Queue<Chunk> _buildQueue;
        private Queue<Chunk> _postProcessQueue;

        public IChunkGenerator ChunkGenerator { get { return _chunkGenerator; } set { _chunkGenerator = value; } }
        public IChunkSystem ChunkSystem { get { return _chunkSystem; } set { _chunkSystem = value; } }

        public Chunk this[int x, int y, int z] { get { return Get(x, y, z); } }

        public int MinimumX { get { return 0; } }
        public int MaximumX { get { return _worldWidth - 1; } }

        public int MinimumY { get { return 0; } }
        public int MaximumY { get { return _worldHeight - 1; } }

        public int MinimumZ { get { return 0; } }
        public int MaximumZ { get { return _worldLength - 1; } }

        public StaticThreadedChunkManager(World world, int worldWidth, int worldHeight, int worldLength)
        {
            Contract.Assert(worldWidth > 0);
            Contract.Assert(worldHeight > 0);
            Contract.Assert(worldLength > 0);

            _world = world;

            _worldWidth = worldWidth;
            _worldHeight = worldHeight;
            _worldLength = worldLength;

            _chunks = new Chunk[worldWidth * worldHeight * worldLength];

            _buildQueue = new Queue<Chunk>();
            _postProcessQueue = new Queue<Chunk>();

            _chunkSystem = null;
        }

        public void GenerateChunks()
        {
            int chunkWidth = _world.EngineConfiguration.ChunkWidth;
            int chunkHeight = _world.EngineConfiguration.ChunkHeight;
            int chunkLength = _world.EngineConfiguration.ChunkLength;

            for (int x = 0; x < _worldWidth; x++)
            {
                for (int y = 0; y < _worldHeight; y++)
                {
                    for (int z = 0; z < _worldLength; z++)
                    {
                        int index = ArrayUtil.Convert3DTo1D(x, y, z, _worldLength, _worldHeight);

                        Chunk c = new Chunk(_world, new Vector3I(x * chunkWidth, y * chunkHeight, z * chunkLength), chunkWidth, chunkHeight, chunkLength);
                        _chunkGenerator.Generate(c);

                        _chunks[index] = c;
                    }
                }
            }
        }

        public void BuildAllChunks()
        {
            for (int x = 0; x < _worldWidth; x++)
            {
                for (int y = 0; y < _worldHeight; y++)
                {
                    for (int z = 0; z < _worldLength; z++)
                    {
                        int index = ArrayUtil.Convert3DTo1D(x, y, z, _worldLength, _worldHeight);

                        _chunkSystem.Builder.Build(_chunks[index]);
                    }
                }
            }

            if (_chunkSystem.Builder.RequiresPostProcess)
            {
                for (int x = 0; x < _worldWidth; x++)
                {
                    for (int y = 0; y < _worldHeight; y++)
                    {
                        for (int z = 0; z < _worldLength; z++)
                        {
                            int index = ArrayUtil.Convert3DTo1D(x, y, z, _worldLength, _worldHeight);

                            _chunkSystem.Builder.PostProcess(_chunks[index]);
                        }
                    }
                }
            }
        }

        public void EnqueueChunkForBuild(Chunk c)
        {
            if (!_buildQueue.Contains(c))
                _buildQueue.Enqueue(c);
        }

        public void Update(Vector3 cameraPosition)
        {
            Contract.Assert(_chunkSystem != null);

            while (_buildQueue.Count > 0)
            {
                Chunk c = _buildQueue.Dequeue();

                // WARNING THIS IS ONLY A QUICK FIX
                _chunkSystem.Builder.Build(c);

                _postProcessQueue.Enqueue(c);
            }

            while (_postProcessQueue.Count > 0)
            {
                Chunk c = _postProcessQueue.Dequeue();

                _chunkSystem.Builder.PostProcess(c);
            }
        }

        public void Render()
        {
            Contract.Assert(_chunkSystem != null);

            for (int i = 0; i < _chunks.Length; i++)
            {
                _chunkSystem.Renderer.Render(_chunks[i]);
            }
        }

        public Chunk Get(int x, int y, int z)
        {
            if (x < 0 || x >= _worldWidth ||
                y < 0 || y >= _worldHeight ||
                z < 0 || z >= _worldLength)
                return null;

            int index = ArrayUtil.Convert3DTo1D(x, y, z, _worldLength, _worldHeight);

            return _chunks[index];
        }
    }
}
