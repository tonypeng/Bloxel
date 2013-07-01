/*
 * Bloxel - EngineConfiguration.cs
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

namespace Bloxel.Engine.Core
{
    public class EngineConfiguration
    {
        private int _chunkWidth;
        private int _chunkHeight;
        private int _chunkLength;

        private bool _cpuLightingEnabled;

        private int _renderDistance;

        public int ChunkWidth
        {
            get { return _chunkWidth; }
            set { _chunkWidth = value; }
        }

        public int ChunkHeight
        {
            get { return _chunkHeight; }
            set { _chunkHeight = value; }
        }

        public int ChunkLength
        {
            get { return _chunkLength; }
            set { _chunkLength = value; }
        }

        public bool CPULightingEnabled
        {
            get { return _cpuLightingEnabled; }
            set { _cpuLightingEnabled = value; }
        }

        public int RenderDistance
        {
            get { return _renderDistance; }
            set { _renderDistance = value; }
        }
    }
}
