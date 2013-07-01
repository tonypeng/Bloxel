/*
 * Bloxel - StaticThreadedChunkManager.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
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
        private EngineConfiguration _config;

        private World _world;

        private IChunkGenerator _chunkGenerator;
        private IChunkSystem _chunkSystem;
        private ILightManager _lightManager;

        private int _worldWidth, _worldHeight, _worldLength;
        private Chunk[] _chunks;

        private Queue<Chunk> _buildQueue;
        private Queue<Chunk> _postProcessQueue;

        public IChunkGenerator ChunkGenerator { get { return _chunkGenerator; } set { _chunkGenerator = value; } }
        public IChunkSystem ChunkSystem { get { return _chunkSystem; } set { _chunkSystem = value; } }
        public ILightManager LightManager { get { return _lightManager; } set { _lightManager = value; } }

        public Chunk this[int x, int y, int z] { get { return Get(x, y, z); } }

        public int MinimumX { get { return 0; } }
        public int MaximumX { get { return _worldWidth - 1; } }

        public int MinimumY { get { return 0; } }
        public int MaximumY { get { return _worldHeight - 1; } }

        public int MinimumZ { get { return 0; } }
        public int MaximumZ { get { return _worldLength - 1; } }

        public StaticThreadedChunkManager(EngineConfiguration config, World world, int worldWidth, int worldHeight, int worldLength)
        {
            Contract.Assert(worldWidth > 0);
            Contract.Assert(worldHeight > 0);
            Contract.Assert(worldLength > 0);

            _config = config;

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
                for (int z = 0; z < _worldLength; z++)
                {
                    if (_config.CPULightingEnabled)
                        _lightManager.LightChunkColumn(x, z);
                    else
                    {
                        for (int y = 0; y < _worldHeight; y++)
                        {
                            int index = ArrayUtil.Convert3DTo1D(x, y, z, _worldLength, _worldHeight);

                            _chunks[index].MarkChunkAwaitingBuild();
                        }
                    }
                }
            }

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

        public void EnqueueChunkForBuild(params Chunk[] c)
        {
            for (int i = 0; i < c.Length; i++)
            {
                if (!_buildQueue.Contains(c[i]))
                    _buildQueue.Enqueue(c[i]);
            }
        }

        public void Update(Vector3 cameraPosition)
        {
            Contract.Assert(_chunkSystem != null);

            //if (_buildQueue.Count > 0)
            //    Console.WriteLine("{0} chunks in queue.", _buildQueue.Count);

            while (_buildQueue.Count > 0)
            {
                Chunk c = _buildQueue.Dequeue();

                if (_config.CPULightingEnabled)
                    _lightManager.LightChunkColumn(c.ChunkSpaceX, c.ChunkSpaceZ);
                else
                    c.MarkChunkAwaitingBuild();

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
