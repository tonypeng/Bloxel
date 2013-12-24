/*
 * Bloxel - Chunk.cs
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
        private World _world;

        // Very important.
        private GridPoint[] _points;

        private Vector3I _position;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private ChunkState _state;

        private int _width, _height, _length;
        private BoundingBox _boundingBox;

        public GridPoint[] Points { get { return _points; } }

        public Vector3I Position { get { return _position; } }

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public int Length { get { return _length; } }

        public VertexBuffer VertexBuffer { get { return _vertexBuffer; } set { _vertexBuffer = value; } }
        public IndexBuffer IndexBuffer { get { return _indexBuffer; } set { _indexBuffer = value; } }
        public VertexBuffer NormalsVertexBuffer { get; set; }

        public object GraphicsSync = new object();

        public Chunk XNegative
        {
            get { return _world.ChunkManager[ChunkSpaceX - 1, ChunkSpaceY, ChunkSpaceZ]; }
        }

        public Chunk XPositive
        {
            get { return _world.ChunkManager[ChunkSpaceX + 1, ChunkSpaceY, ChunkSpaceZ]; }
        }

        public Chunk YNegative
        {
            get { return _world.ChunkManager[ChunkSpaceX, ChunkSpaceY - 1, ChunkSpaceZ]; }
        }

        public Chunk YPositive
        {
            get { return _world.ChunkManager[ChunkSpaceX, ChunkSpaceY + 1, ChunkSpaceZ]; }
        }

        public Chunk ZNegative
        {
            get { return _world.ChunkManager[ChunkSpaceX, ChunkSpaceY, ChunkSpaceZ - 1]; }
        }

        public Chunk ZPositive
        {
            get { return _world.ChunkManager[ChunkSpaceX, ChunkSpaceY, ChunkSpaceZ + 1]; }
        }

        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
        }

        public Vector3I ChunkSpacePosition
        {
            get { return new Vector3I(ChunkSpaceX, ChunkSpaceY, ChunkSpaceZ); }
        }

        public int ChunkSpaceX
        {
            get { return _position.X / _width; }
        }

        public int ChunkSpaceY
        {
            get { return _position.Y / _height; }
        }

        public int ChunkSpaceZ
        {
            get { return _position.Z / _length; }
        }

        public ChunkState State { get { return _state; } }

        public Chunk(World world, Vector3I position, int width, int height, int length)
        {
            Contract.Assert(width > 0);
            Contract.Assert(height > 0);
            Contract.Assert(length > 0);

            _world = world;

            _position = position;

            _width = width;
            _height = height;
            _length = length;

            _boundingBox = new Microsoft.Xna.Framework.BoundingBox(_position.ToVector3(), (_position + new Vector3I(_width, _height, _length)).ToVector3());

            _points = new GridPoint[width * height * length];

            _vertexBuffer = null;
            _indexBuffer = null;
        }

        public GridPoint PointAt(int x, int y, int z)
        {
            return _world.PointAt(x + _position.X, y + _position.Y, z + _position.Z);
        }

        public void SetPointLocal(int x, int y, int z, GridPoint gp, bool suppressRebuild = false)
        {
            _points[ArrayUtil.Convert3DTo1D(x, y, z, _length, _height)] = gp;

            if (!suppressRebuild)
            {
                MarkDataOutOfSync(false);

                List<Chunk> rebuildChunks = new List<Chunk>(1);
                rebuildChunks.Add(this);

                Chunk xNegative = XNegative;
                Chunk xPositive = XPositive;
                Chunk yNegative = YNegative;
                Chunk yPositive = YPositive;
                Chunk zNegative = ZNegative;
                Chunk zPositive = ZPositive;

                Chunk xNegativezNegative = _world.ChunkManager[ChunkSpaceX - 1, ChunkSpaceY, ChunkSpaceZ - 1];
                Chunk xNegativeyNegative = _world.ChunkManager[ChunkSpaceX - 1, ChunkSpaceY - 1, ChunkSpaceZ];
                Chunk yNegativezNegative = _world.ChunkManager[ChunkSpaceX, ChunkSpaceY - 1, ChunkSpaceZ - 1];

                bool minx = false;
                bool miny = false;
                bool minz = false;

                if ((x == 0 || x == 1) && xNegative != null)
                {
                    rebuildChunks.Add(xNegative);
                    minx = true;
                }
                if (x == _width - 1 && xPositive != null) rebuildChunks.Add(xPositive);

                if ((y == 0 || y == 1) && yNegative != null)
                {
                    rebuildChunks.Add(yNegative);
                    miny = true;
                }
                if (y == _height - 1 && yPositive != null) rebuildChunks.Add(yPositive);

                if ((z == 0 || z == 1) && zNegative != null)
                {
                    rebuildChunks.Add(zNegative);
                    minz = true;
                }
                if (z == _length - 1 && zPositive != null) rebuildChunks.Add(zPositive);

                if (minx && minz && xNegativezNegative != null) rebuildChunks.Add(xNegativezNegative);
                if (minx && miny && xNegativeyNegative != null) rebuildChunks.Add(xNegativeyNegative);
                if (miny && minz && yNegativezNegative != null) rebuildChunks.Add(yNegativezNegative);

                _world.ChunkManager.EnqueueChunkForBuild(rebuildChunks.ToArray());
            }
        }

        public void SetPoint(int x, int y, int z, GridPoint gp, bool suppressRebuild=false)
        {
            _world.SetPoint(x + _position.X, y + _position.Y, z + _position.Z, gp, suppressRebuild);
        }

        public void SetLightLocal(int x, int y, int z, byte light)
        {
            int index = ArrayUtil.Convert3DTo1D(x, y, z, _length, _height);
            GridPoint gp = _points[index];
            gp.Metadata[0] = light;
        }

        public void SetLight(int x, int y, int z, byte light)
        {
            _world.SetLight(x + _position.X, y + _position.Y, z + _position.Z, light);
        }

        public void MarkDataOutOfSync(bool enqueueChunk = true)
        {
            _state = ChunkState.DataOutOfSync;

            if(enqueueChunk)
                _world.ChunkManager.EnqueueChunkForBuild(this);
        }

        public void MarkChunkBuilding()
        {
            _state = ChunkState.Building;
        }

        public void MarkChunkLighting()
        {
            _state = ChunkState.Lighting;
        }

        public void MarkChunkAwaitingBuild()
        {
            _state = ChunkState.AwaitingBuild;
        }

        public void MarkDataInSync()
        {
            _state = ChunkState.Ready;
        }
    }
}
