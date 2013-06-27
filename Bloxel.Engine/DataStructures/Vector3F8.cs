/*
 * Bloxel - Vector3F8.cs
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

namespace Bloxel.Engine.DataStructures
{
    public struct Vector3F8
    {
        public static Vector3F8 Zero = new Vector3F8(0, 0, 0);

        public static Vector3F8 XNegative = new Vector3F8(-1, 0, 0);
        public static Vector3F8 XPositive = new Vector3F8(1, 0, 0);
        public static Vector3F8 YNegative = new Vector3F8(0, -1, 0);
        public static Vector3F8 YPositive = new Vector3F8(0, 1, 0);
        public static Vector3F8 ZNegative = new Vector3F8(0, 0, -1);
        public static Vector3F8 ZPositive = new Vector3F8(0, 0, 1);

        Float8 _X, _Y, _Z; // 3 bytes total

        public Vector3F8(float xyz)
        {
            _X = _Y = _Z = xyz;
        }

        public Vector3F8(Vector3 v3)
            : this(v3.X, v3.Y, v3.Z)
        { }

        public Vector3F8(float x, float y, float z)
        {
            _X = x;
            _Y = y;
            _Z = z;
        }

        public Float8 X { get { return _X; } set { _X = value; } }
        public Float8 Y { get { return _Y; } set { _Y = value; } }
        public Float8 Z { get { return _Z; } set { _Z = value; } }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() != typeof(Vector3F8) && obj.GetType() != typeof(Vector3)) return false;

            if (obj.GetType() == typeof(Vector3))
            {
                Vector3 other = (Vector3)obj;

                return (float)_X == other.X && (float)_Y == other.Y && (float)_Z == other.Z;
            }

            // it's another Vector3F8
            Vector3F8 another = (Vector3F8)obj;

            return this == another;
        }

        public override int GetHashCode()
        {
            // TODO: check me
            int hash = 41;

            unchecked
            {
                hash = hash * 53 + _X.PackedValue;
                hash = hash * 53 + _Y.PackedValue;
                hash = hash * 53 + _Z.PackedValue;
            }

            return hash;
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }

        public static Vector3F8 Min(Vector3F8 v1, Vector3F8 v2)
        {
            float minX = Math.Min(v1.X, v2.X);
            float minY = Math.Min(v1.Y, v2.Y);
            float minZ = Math.Min(v1.Z, v2.Z);

            return new Vector3F8(minX, minY, minZ);
        }

        public static Vector3F8 operator +(Vector3F8 v1, Vector3F8 v2)
        {
            return new Vector3F8(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3F8 operator -(Vector3F8 v1, Vector3F8 v2)
        {
            return v1 + -1 * v2;
        }

        public static Vector3F8 operator *(Vector3F8 v, int i)
        {
            return new Vector3F8(v.X.ToSingle() * i, v.Y.ToSingle() * i, v.Z.ToSingle() * i);
        }

        public static Vector3F8 operator *(int i, Vector3F8 v)
        {
            return v * i;
        }

        public static bool operator ==(Vector3F8 a, Vector3F8 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3F8 a, Vector3F8 b)
        {
            return !(a == b);
        }
    }
}
