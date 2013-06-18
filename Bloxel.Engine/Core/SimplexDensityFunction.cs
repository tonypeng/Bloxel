/*
 * Bloxel - SimplexDensityFunction.cs
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

using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.Core
{
    public class SimplexDensityFunction : IDensityFunction, ITerrainGradientFunction
    {
        private SimplexNoiseGenerator _noise;

        private float _delta;
        private float _magnification;

        public SimplexDensityFunction(float delta, float magnification)
        {
            _delta = delta;
            _magnification = magnification;

            _noise = new SimplexNoiseGenerator(Environment.TickCount);
        }

        public float f(float x, float y, float z)
        {
            float octave1 = _noise.noise3d(x * _magnification, y * _magnification, z * _magnification);
            float octave2 = _noise.noise3d(x * _magnification * 0.5f, y * _magnification * 0.5f, z * _magnification * 0.5f);

            return octave1 * 0.75f + octave2 * 0.25f;
        }

        public Vector3 df(float x, float y, float z)
        {
            float baseDensity = f(x, y, z);

            float normalX = f(x + _delta, y, z) - baseDensity;
            float normalY = f(x, y + _delta, z) - baseDensity;
            float normalZ = f(x, y, z + _delta) - baseDensity;

            Vector3 gradient = new Vector3(normalX, normalY, normalZ);
            gradient.Normalize();

            return gradient;
        }
    }
}
