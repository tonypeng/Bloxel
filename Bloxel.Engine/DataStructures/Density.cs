/*
 * Bloxel - Density.cs
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

namespace Bloxel.Engine.DataStructures
{
    /// <summary>
    /// Provides a one-byte density value, in increments of (1/255), or ~0.004
    /// </summary>
    public struct Density
    {
        private short _density;

        public Density(float density)
            : this((short)(density * 32767f))
        { }

        public Density(short density)
        {
            _density = density;
        }

        public void Set(float f)
        {
            _density = (short)(f * 32767f);
        }

        public float ToSingle()
        {
            return _density / 32767f;
        }

        public short PackedDensity
        {
            get { return _density; }
            set { _density = value; }
        }
    }
}
