/*
 * Bloxel - NoiseGenerator.cs
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

namespace Bloxel.Engine.Utilities
{
    public abstract class NoiseGenerator
    {
        public abstract float noise2d(float x, float y);
        public abstract float noise3d(float x, float y, float z);

        public virtual float GenerateNoise(float x, float y, float frequency, float amplitude, float persistence, int octaves, Func<float, float, float> smooth)
        {
            float noise = 0.0f;

            float maxAmplitude = 0f;

            for (int i = 0; i < octaves; i++)
            {
                noise += noise2d(x * frequency, y * frequency) * amplitude;
                frequency *= 2f;
                maxAmplitude += amplitude;
                amplitude *= persistence;
            }

            return noise / maxAmplitude;
        }

        public virtual float GenerateNoise(float x, float y, float z, float frequency, float amplitude, float persistence, int octaves, Func<float, float, float> smooth)
        {
            float noise = 0.0f;

            float maxAmplitude = 0f;

            for (int i = 0; i < octaves; i++)
            {
                noise += noise3d(x * frequency, y * frequency, z * frequency) * amplitude;
                frequency *= 2f;
                maxAmplitude += amplitude;
                amplitude *= persistence;
            }

            return noise / maxAmplitude;
        }
    }
}
