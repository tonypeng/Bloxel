/*
 * Bloxel - FloodfillLightManager.cs
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

using Bloxel.Engine.DataStructures;

namespace Bloxel.Engine.Core
{
    public class FloodfillLightManager : ILightManager
    {
        private IChunkManager _chunkManager;

        private EngineConfiguration _config;

        private int _worldChunkHeight;

        public FloodfillLightManager(IChunkManager chunkManager, EngineConfiguration config, int worldChunkHeight)
        {
            _chunkManager = chunkManager;

            _config = config;

            _worldChunkHeight = worldChunkHeight;
        }

        public void LightChunkColumn(int chunkSpaceX, int chunkSpaceZ)
        {
            ClearChunkColumn(chunkSpaceX, chunkSpaceZ);
            FillChunkColumn(chunkSpaceX, chunkSpaceZ);
        }

        public void ClearChunkColumn(int chunkSpaceX, int chunkSpaceZ)
        {
            for (int x = 0; x < _config.ChunkWidth; x++)
            {
                for (int z = 0; z < _config.ChunkLength; z++)
                {
                    bool canSeeSun = true;

                    for (int cy = _worldChunkHeight - 1; cy >= 0; cy--)
                    {
                        Chunk ch = _chunkManager[chunkSpaceX, cy, chunkSpaceZ];

                        if (ch.State != ChunkState.DataOutOfSync)
                            continue;

                        for (int y = ch.Height - 1; y >= 0; y--)
                        {
                            int worldX = ch.Position.X + x;
                            int worldY = ch.Position.Y + y;
                            int worldZ = ch.Position.Z + z;

                            byte light = 0;

                            if (ch.PointAt(x, y, z).Density >= 0.0f)
                                canSeeSun = false;

                            if (canSeeSun)
                                light = 15;

                            ch.SetLightLocal(x, y, z, light);

                            if (worldX == 25 && worldZ == 19)
                                Console.WriteLine("{0},{1},{2}: {3}", worldX, worldY, worldZ, light);
                        }
                    }
                }
            }
        }

        public void FillChunkColumn(int chunkSpaceX, int chunkSpaceZ)
        {
        }
    }
}
