/*
 * Bloxel - GridPoint.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
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
            : this(material, density, 1)
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

        /// <summary>
        /// Gets the array of metadata for this gridpoint.
        /// </summary>
        /// <remarks>When Metadata is initialized with a non-zero length and CPU lighting is enabled, index 0 is reserved for lighting.</remarks>
        public byte[] Metadata { get { return _metadata; } }
    }
}
