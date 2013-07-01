/*
 * Bloxel - SphereDensityFunction.cs
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

namespace Bloxel.Engine.Core
{
    public class SphereDensityFunction : IDensityFunction, ITerrainGradientFunction
    {
        private Vector3 _center;
        private float _radius;

        public SphereDensityFunction(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        public float f(float x, float y, float z)
        {
            // implicit function of a sphere:
            // 0 = (x-a)^2 + (y-b)^2 + (z-c)^2 - r^2

            x -= _center.X;
            y -= _center.Y;
            z -= _center.Z;

            return -1 * (x * x + y * y + z * z - _radius * _radius); // multiply by -1 because we want outside to be negative
        }

        public Vector3 df(float x, float y, float z)
        {
            x -= _center.X;
            y -= _center.Y;
            z -= _center.Z;

            Vector3 v3 = new Vector3(x, y, z);
            v3.Normalize();

            return v3;
        }
    }
}
