/*
 * Bloxel - PlusSignGenerator.cs
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

using Bloxel.Engine.Core;
using Bloxel.Engine.DataStructures;

namespace Playground.Generator
{
    public class PlusSignGenerator : IChunkGenerator
    {
        public void Generate(Chunk c)
        {
            for (int x = 0; x < c.Width; x++)
            {
                for (int z = 0; z < c.Length; z++)
                {
                    for (int y = 0; y < c.Height; y++)
                    {
                        c.SetPoint(x, y, z, GridPoint.Empty);
                    }
                }
            }

            c.MarkDataOutOfSync(false);
        }
    }
}
