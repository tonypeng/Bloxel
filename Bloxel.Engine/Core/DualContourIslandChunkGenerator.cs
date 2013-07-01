/*
 * Bloxel - DualContourIslandChunkGenerator.cs
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
    public class DualContourIslandChunkGenerator : IChunkGenerator, ITerrainGradientFunction
    {
        int _worldMaxHeight;
        NoiseGenerator _noiseGenerator;
        bool _isCubic;

        public bool RequiresPostProcess { get { return true; } }

        public DualContourIslandChunkGenerator(int worldMaxHeight, NoiseGenerator noiseGenerator)
            : this(worldMaxHeight, noiseGenerator, false)
        { }

        public DualContourIslandChunkGenerator(int worldMaxHeight, NoiseGenerator noiseGenerator, bool cubic)
        {
            _worldMaxHeight = worldMaxHeight;
            _noiseGenerator = noiseGenerator;
            _isCubic = cubic;
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

                    Vector3F8 normal = Vector3F8.Zero;

                    for (int y = c.Height - 1; y >= 0; y--)
                    {
                        GridPoint g;

                        int worldY = c.Position.Y + y;

                        if (worldY > groundHeight)
                        {
                            g = new GridPoint(GridPoint.Empty, (int)DualContouringMetadataIndex.Length);
                        }
                        else if (worldY == groundHeightFloor)
                        {
                            float delta = groundHeight - groundHeightFloor;

                            g = _isCubic ? new GridPoint(GridPoint.Full, (int)DualContouringMetadataIndex.Length) : new GridPoint(0, delta / (1 - delta), (int)DualContouringMetadataIndex.Length);
                        }
                        else if (worldY > lowerGroundHeight)
                        {
                            g = new GridPoint(GridPoint.Full, (int)DualContouringMetadataIndex.Length);
                        }
                        else
                        {
                            g = new GridPoint(GridPoint.Full, (int)DualContouringMetadataIndex.Length);
                        }

                        c.SetPointLocal(x, y, z, g, true);
                    }
                }
            }

            c.MarkDataOutOfSync(false);
        }

        private float UpperGround(float worldX, float worldZ, int lowerGround)
        {
            //float theNoise = noise.noise2d(worldX * 0.1f, worldZ * 0.1f) + 2f;

            //return lowerGround + theNoise * 0.75f;

            float theNoise = (_noiseGenerator.GenerateNoise(worldX, worldZ, 0.02f, 0.5f, 0.5f, 3, null) + 1f) * 0.4f;

            return (theNoise * ((float)_worldMaxHeight / 2f)) + lowerGround;
        }

        private float LowerGround(float worldX, float worldZ)
        {
            int minGroundHeight = _worldMaxHeight / 4;
            int minGroundDepth = (int)(_worldMaxHeight * 0.3f);

            float theNoise = (_noiseGenerator.GenerateNoise(worldX, worldZ, 0.1f, 0.5f, 0.7f, 3, null) + 1f) * 0.4f;

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
