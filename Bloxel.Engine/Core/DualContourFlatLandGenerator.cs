/*
 * Bloxel - DualContourFlatLandGenerator.cs
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
using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.Core
{
    public class DualContourFlatLandGenerator : IChunkGenerator, ITerrainGradientFunction
    {
        public void Generate(Chunk c)
        {
            for (int x = 0; x < c.Width; x++)
            {
                for (int z = 0; z < c.Length; z++)
                {
                    for (int y = 0; y < c.Height; y++)
                    {
                        int worldHeight = c.Position.Y + y;

                        if (worldHeight < 15)
                            c.SetPointLocal(x, y, z, GridPoint.Full, true);
                        else
                            c.SetPointLocal(x, y, z, GridPoint.Empty, true);
                    }
                }
            }

            c.MarkDataOutOfSync(false);
        }

        public Vector3 df(float x, float y, float z)
        {
            return Vector3.Up;
        }
    }
}
