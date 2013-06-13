/*
 * Bloxel - Edge.cs
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

namespace Bloxel.Engine.DataStructures
{
    public struct Edge : IEquatable<Edge>
    {
        private Vector3I _point1;
        private Vector3I _point2;

        public Edge(Vector3I point1, Vector3I point2)
        {
            _point1 = point1;
            _point2 = point2;
        }

        public Vector3I Point1 { get { return _point1; } }
        public Vector3I Point2 { get { return _point2; } }

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
