using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Bloxel.Engine.Core
{
    public class SineWaveDensityFunction : IDensityFunction, ITerrainGradientFunction
    {
        private float _amplitude;
        private float _vertShift;
        private float _xScale;

        public SineWaveDensityFunction(float amplitude, float vertShift, float xscale)
        {
            _amplitude = amplitude;
            _vertShift = vertShift;
            _xScale = xscale;
        }

        public float f(float x, float y, float z)
        {
            return (_vertShift + _amplitude * (float)Math.Sin(x * _xScale)) - y;
        }

        public Vector3 df(float x, float y, float z)
        {
            Vector3 v3 = new Vector3(-1f / (_amplitude * _xScale * (float)Math.Cos(x * _xScale)), 1, 0);
            v3.Normalize();

            return v3;
        }
    }
}
