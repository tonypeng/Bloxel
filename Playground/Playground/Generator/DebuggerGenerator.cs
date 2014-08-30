/*
 * Bloxel - DebuggerGenerator.cs
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

using Bloxel.Engine.Core;
using Bloxel.Engine.DataStructures;
using Bloxel.Engine.Utilities;

namespace Playground.Generator
{
    public class DebuggerGenerator : IChunkGenerator, ITerrainGradientFunction
    {
        public void Generate(Chunk c)
        {
            for (int x = 0; x < c.Width; x++)
            {
                for (int z = 0; z < c.Length; z++)
                {
                    for (int y = 0; y < c.Height; y++)
                    {
                        int worldX = c.Position.X + x;
                        int worldZ = c.Position.Z + z;
                        int worldHeight = c.Position.Y + y;

                        if (worldHeight < 20)
                            c.SetPointLocal(x, y, z, GridPoint.Full, true);
                        else if (worldHeight == 20)
                        {
                            if ((worldX == 16 && worldZ == 16) || ((worldX == 17) && worldZ == 17))
                                c.SetPointLocal(x, y, z, GridPoint.Full, true);
                            else
                                c.SetPointLocal(x, y, z, GridPoint.Empty, true);
                        }
                        else
                        {
                            c.SetPointLocal(x, y, z, GridPoint.Empty, true);
                        }
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
