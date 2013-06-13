/*
 * Bloxel - DensityChunkGenerator.cs
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
                        float value = _densityFunction.f(x, y, z);

                        if (value > 1.0f)
                            value = 1.0f;
                        if (value < -1.0f)
                            value = -1.0f;

                        GridPoint b = new GridPoint(0, value);
                        c.SetPoint(x, y, z, b);
                    }
                }
            }
        }
    }
}
