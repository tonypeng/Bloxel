﻿/*
 * Bloxel - Edge.cs
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

namespace Bloxel.Engine.DataStructures
{
    public struct Edge : IEquatable<Edge>
    {
        private Vector3I _point1;
        private Vector3I _point2;

        private Direction _direction;

        public Edge(Vector3I point1, Vector3I point2, Direction direction)
        {
            _point1 = point1;
            _point2 = point2;

            _direction = direction;
        }

        public Vector3I Point1 { get { return _point1; } }
        public Vector3I Point2 { get { return _point2; } }

        /// <summary>
        /// Gets the direction that the face constructed from this edge should face.
        /// </summary>
        public Direction FaceDirection { get { return _direction; } }

        public bool Equals(Edge other)
        {
            return (other._point1 == this._point1 && other._point2 == this._point2) ||
                (other._point1 == this._point2 && other._point2 == this._point1);
        }

        /// <summary>
        /// Returns the positions of the four cubes that share this edge.
        /// </summary>
        /// <returns>The positions of the four cubes that share this edge.</returns>
        public Vector3I[] GetCubePositions()
        {
            Vector3I[] ret = new Vector3I[4];

            Vector3I dir = (_point2 - _point1);

            int dx = dir.X;
            int dy = dir.Y;
            int dz = dir.Z;

            // assuming that coordinates are all increments of 1
            Contract.Assert(Math.Abs(dx + dy + dz) == 1); // only one coordinate should change

            Vector3I min = Vector3I.Min(_point1, _point2);

            ret[0] = min;

            Vector3I bothComponents  = Vector3I.Zero;

            int i = 1;

            if (dx == 0)
            {
                bothComponents.X = -1;
                ret[i++] = min - new Vector3I(1, 0, 0);
            }

            if (dy == 0)
            {
                bothComponents.Y = -1;
                ret[i++] = min - new Vector3I(0, 1, 0);
            }

            if (dz == 0)
            {
                bothComponents.Z = -1;
                ret[i++] = min - new Vector3I(0, 0, 1);
            }

            ret[3] = min + bothComponents;

            return ret;
        }
    }
}
