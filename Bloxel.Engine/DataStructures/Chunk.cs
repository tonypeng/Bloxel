﻿/*
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
            get { return _world.ChunkManager[_position.X - _width, _position.Y, _position.Z]; }
        }

        public Chunk XPositive
        {
            get { return _world.ChunkManager[_position.X + _width, _position.Y, _position.Z]; }
        }

        public Chunk YNegative
        {
            get { return _world.ChunkManager[_position.X, _position.Y - _height, _position.Z]; }
        }

        public Chunk YPositive
        {
            get { return _world.ChunkManager[_position.X, _position.Y + _height, _position.Z]; }
        }

        public Chunk ZNegative
        {
            get { return _world.ChunkManager[_position.X, _position.Y, _position.Z - _length]; }
        }

        public Chunk ZPositive
        {
            get { return _world.ChunkManager[_position.X, _position.Y, _position.Z + _length]; }
        }

        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
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

        public void SetPointLocal(int x, int y, int z, GridPoint gp, bool suppressRebuild=false)
        {
            _points[ArrayUtil.Convert3DTo1D(x, y, z, _length, _height)] = gp;

            if(!suppressRebuild)
                MarkDataOutOfSync();
        }

        public void SetPoint(int x, int y, int z, GridPoint gp, bool suppressRebuild=false)
        {
            _world.SetPoint(x + _position.X, y + _position.Y, z + _position.Z, gp, suppressRebuild);
        }

        public void MarkDataOutOfSync()
        {
            _state = ChunkState.DataOutOfSync;

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

        public void MarkDataInSync()
        {
            _state = ChunkState.Ready;
        }
    }
}
