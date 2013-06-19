/*
 * Bloxel - IslandSimplexDensityFunction.cs
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
using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.Core
{
    public class IslandChunkGenerator : IChunkGenerator, ITerrainGradientFunction
    {
        int _worldMaxHeight;
        NoiseGenerator _noiseGenerator;

        public IslandChunkGenerator(int worldMaxHeight, NoiseGenerator noiseGenerator)
        {
            _worldMaxHeight = worldMaxHeight;
            _noiseGenerator = noiseGenerator;
        }

        public void Generate(Chunk c)
        {
            for (int x = 0; x < c.Width; x++)
            {
                for (int z = 0; z < c.Length; z++)
                {
                    int worldX = c.Position.X + x;
                    int worldZ = c.Position.Z + z;

                    int lowerGroundHeight = (int)LowerGround(worldX, worldZ);
                    lowerGroundHeight = 10;

                    float groundHeight = UpperGround(worldX, worldZ, lowerGroundHeight);
                    int groundHeightFloor = (int)groundHeight;

                    bool canSeeSun = true;
                    bool nextBlockIsGrass = false;

                    for (int y = c.Height - 1; y >= 0; y--)
                    {
                        GridPoint g;

                        int worldY = c.Position.Y + y;

                        if (worldY > groundHeight)
                        {
                            g = GridPoint.Empty;
                        }
                        else if (worldY == groundHeightFloor)
                        {
                            float delta = groundHeight - groundHeightFloor;

                            g = new GridPoint(0, delta / (1 - delta));
                        }
                        else if (worldY > lowerGroundHeight)
                        {
                            g = GridPoint.Full;
                        }
                        else
                        {
                            g = GridPoint.Full;
                        }

                        c.SetPoint(x, y, z, g);
                    }
                }
            }
        }

        private float UpperGround(float worldX, float worldZ, int lowerGround)
        {
            //float theNoise = noise.noise2d(worldX * 0.1f, worldZ * 0.1f) + 2f;

            //return lowerGround + theNoise * 0.75f;

            float theNoise = (_noiseGenerator.GenerateNoise(worldX, worldZ, 0.01f, 0.5f, 0.5f, 3, null) + 1f) * 0.4f;

            return (theNoise * ((float)_worldMaxHeight / 2f)) + lowerGround;
        }

        private float LowerGround(float worldX, float worldZ)
        {
            int minGroundHeight = _worldMaxHeight / 4;
            int minGroundDepth = (int)(_worldMaxHeight * 0.3f);

            float theNoise = (_noiseGenerator.GenerateNoise(worldX, worldZ, 0.05f, 0.5f, 0.7f, 3, null) + 1f) * 0.4f;

            return (theNoise * minGroundDepth + minGroundHeight);
        }

        public Vector3 df(float x, float y, float z)
        {
            int lowerGroundHeight = (int)LowerGround(x, z);
            lowerGroundHeight = 10;

            float groundHeight = UpperGround(x, z, lowerGroundHeight);

            // now apply a small delta
            int lowerGroundHeightX = (int)LowerGround(x + 0.01f, z);
            lowerGroundHeightX = 10;

            float groundHeightX = UpperGround(x + 0.01f, z, lowerGroundHeightX);

            int lowerGroundHeightXZ = (int)LowerGround(x + 0.01f, z + 0.01f);
            lowerGroundHeightXZ = 10;

            float groundHeightXZ = UpperGround(x + 0.01f, z + 0.01f, lowerGroundHeightXZ);

            // cross product
            Vector3 first = new Vector3(0.01f, groundHeightX - groundHeight, 0f);
            Vector3 second = new Vector3(0f, lowerGroundHeightXZ - groundHeightX, 0.01f);

            return Vector3.Cross(first, second);
        }
    }
}
