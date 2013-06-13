/*
 * Bloxel - Vector3I.cs
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

namespace Bloxel.Engine.DataStructures
{
    public struct Vector3I
    {
        public static Vector3I Zero = new Vector3I(0, 0, 0);

        public static Vector3I XNegative = new Vector3I(-1, 0, 0);
        public static Vector3I XPositive = new Vector3I(1, 0, 0);
        public static Vector3I YNegative = new Vector3I(0, -1, 0);
        public static Vector3I YPositive = new Vector3I(0, 1, 0);
        public static Vector3I ZNegative = new Vector3I(0, 0, -1);
        public static Vector3I ZPositive = new Vector3I(0, 0, 1);

        int _X, _Y, _Z;

        public Vector3I(int xyz)
        {
            _X = _Y = _Z = xyz;
        }

        public Vector3I(int x, int y, int z)
        {
            _X = x;
            _Y = y;
            _Z = z;
        }

        public int X { get { return _X; } set { _X = value; } }
        public int Y { get { return _Y; } set { _Y = value; } }
        public int Z { get { return _Z; } set { _Z = value; } }

        public Microsoft.Xna.Framework.Vector3 ToVector3()
        {
            return new Microsoft.Xna.Framework.Vector3(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() != typeof(Vector3I) && obj.GetType() != typeof(Microsoft.Xna.Framework.Vector3)) return false;

            if (obj.GetType() == typeof(Microsoft.Xna.Framework.Vector3))
            {
                Microsoft.Xna.Framework.Vector3 other = (Microsoft.Xna.Framework.Vector3)obj;

                return (float)_X == other.X && (float)_Y == other.Y && (float)_Z == other.Z;
            }

            // it's another Vector3I
            Vector3I another = (Vector3I)obj;

            return this == another;
        }

        public override int GetHashCode()
        {
            // TO-DO: check me
            int hash = 41;

            unchecked
            {
                hash = hash * 53 + X;
                hash = hash * 53 + Y;
                hash = hash * 53 + Z;
            }

            return hash;
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }

        public static Vector3I Min(Vector3I v1, Vector3I v2)
        {
            int minX = Math.Min(v1.X, v2.X);
            int minY = Math.Min(v1.Y, v2.Y);
            int minZ = Math.Min(v1.Z, v2.Z);

            return new Vector3I(minX, minY, minZ);
        }

        public static Vector3I operator +(Vector3I v1, Vector3I v2)
        {
            return new Vector3I(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3I operator -(Vector3I v1, Vector3I v2)
        {
            return v1 + -1 * v2;
        }

        public static Vector3I operator *(Vector3I v, int i)
        {
            return new Vector3I(v.X * i, v.Y * i, v.Z * i);
        }

        public static Vector3I operator *(int i, Vector3I v)
        {
            return v * i;
        }

        public static bool operator ==(Vector3I a, Vector3I b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3I a, Vector3I b)
        {
            return !(a == b);
        }
    }
}
