/*
 * Bloxel - Block.cs
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Bloxel.Engine.DataStructures
{
    public struct GridPoint
    {
        public static GridPoint Full = new GridPoint(0, 1.0f);
        public static GridPoint Empty = new GridPoint(0, -1.0f);

        private byte _material;
        private Density _density;
        private byte[] _metadata;

        private Vector3 _XPositive, _YPositive, _ZPositive;

        public GridPoint(GridPoint other)
            : this(other, other._metadata.Length)
        { }

        public GridPoint(GridPoint other, int metaDataLength)
        {
            _material = other._material;
            _density = other._density;

            _metadata = new byte[metaDataLength];

            for (int i = 0; i < other._metadata.Length && i < metaDataLength; i++)
            {
                _metadata[i] = other._metadata[i];
            }

            _XPositive = other.XPositiveNormal;
            _YPositive = other.YPositiveNormal;
            _ZPositive = other.ZPositiveNormal;
        }

        public GridPoint(byte material, float density)
            : this(material, density, 0)
        { }

        public GridPoint(byte material, float density, int metaDataLength)
        {
            Contract.Assert(metaDataLength >= 0);

            _material = material;
            _density = new Density(density);

            _metadata = new byte[metaDataLength];

            _XPositive = _YPositive = _ZPositive = Vector3.Zero;
        }

        public Vector3 XPositiveNormal { get { return _XPositive; } set { _XPositive = value; } }
        public Vector3 YPositiveNormal { get { return _YPositive; } set { _YPositive = value; } }
        public Vector3 ZPositiveNormal { get { return _ZPositive; } set { _ZPositive = value; } }

        public byte Material { get { return _material; } }

        public float Density
        {
            get { return _density.ToSingle(); }
            set
            {
                Contract.Assert(value >= -1.0f && value <= 1.0f);

                _density.Set(value);
            }
        }

        public byte[] Metadata { get { return _metadata; } }
    }
}
