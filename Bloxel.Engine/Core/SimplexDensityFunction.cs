/*
 * Bloxel - SimplexDensityFunction.cs
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
