/*
 * Bloxel - IChunkManager.cs
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

using Bloxel.Engine.DataStructures;

namespace Bloxel.Engine.Core
{
    public interface IChunkManager
    {
        IChunkGenerator ChunkGenerator { get; set; }
        IChunkSystem ChunkSystem { get; set; }
        ILightManager LightManager { get; set; }

        Chunk this[int x, int y, int z] { get; }

        int MinimumX { get; }
        int MaximumX { get; }

        int MinimumY { get; }
        int MaximumY { get; }

        int MinimumZ { get; }
        int MaximumZ { get; }

        Chunk Get(int x, int y, int z);

        void GenerateChunks();
        void BuildAllChunks();

        void EnqueueChunkForBuild(params Chunk[] chunks);

        void Update(Vector3 cameraPosition);

        void Render();
    }
}
