/*
 * Bloxel - DensityChunkGenerator.cs
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
    public class DensityChunkGenerator : IChunkGenerator
    {
        private IDensityFunction _densityFunction;

        public DensityChunkGenerator(IDensityFunction densityFunction)
        {
            _densityFunction = densityFunction;
        }

        public void Generate(Chunk c)
        {
            for (int x = 0; x < c.Width; x++)
            {
                for (int z = 0; z < c.Length; z++)
                {
                    for (int y = 0; y < c.Height; y++)
                    {
                        float value = _densityFunction.f(c.Position.X + x, c.Position.Y + y, c.Position.Z + z);

                        if (value > 1.0f)
                            value = 1.0f;
                        if (value < -1.0f)
                            value = -1.0f;

                        GridPoint b = new GridPoint(0, value);
                        c.SetPointLocal(x, y, z, b);
                    }
                }
            }

            c.MarkDataOutOfSync(false);
        }
    }
}
