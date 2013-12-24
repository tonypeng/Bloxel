using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Bloxel.Engine;
using Bloxel.Engine.Cameras;
using Bloxel.Engine.Core;
using Bloxel.Engine.DataStructures;

namespace MeshExtractor
{
    class Program
    {
        static float sphereCx = 0, sphereCy = 0, sphereCz = 0;

        static float sphereImplicitFunction(float x, float y, float z, float radius)
        {
            float dx = x - sphereCx;
            float dy = y - sphereCy;
            float dz = z - sphereCz;

            return radius * radius - (dx * dx + dy * dy + dz * dz);
        }

        static Vector3 sphereGradientFunction(float x, float y, float z)
        {
            return new Vector3(x - sphereCx, y - sphereCy, z - sphereCz);
        }

        static void Main(string[] args)
        {
            EngineConfiguration config = new EngineConfiguration();

            config.ChunkWidth = 16;
            config.ChunkHeight = 16;
            config.ChunkLength = 16;
            config.CPULightingEnabled = true;
            config.RenderDistance = 5;

            CameraManager camManager = new CameraManager();
            World world = new World(config, camManager);
            IChunkManager chunkManager = new StaticThreadedChunkManager(config, world, 1, 1, 1);
            Chunk created = chunkManager[0, 0, 0];

            sphereCx = sphereCy = sphereCz = 8.0f;

            for (int x = 0; x < config.ChunkWidth; x++)
            {
                for (int z = 0; z < config.ChunkLength; z++)
                {
                    for (int y = 0; y < config.ChunkHeight; y++)
                    {
                        float density = sphereImplicitFunction(x, y, z, 4.0f);

                        GridPoint gp = new GridPoint(0, density);

                        if (density >= 0.0f)
                            Console.WriteLine("Solid at ({0}, {1}, {2}); density: {3}\n", x, y, z, density);

                        created.SetPoint(x, y, z, gp);
                    }
                }
            }

            //DualContourChunkBuilder dccb = new DualContourChunkBuilder(null, world, null, 0.0f);
        }
    }
}
