/*
 * Bloxel - NoiseGenerator.cs
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
