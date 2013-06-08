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

        public int X { get { return _X; } }
        public int Y { get { return _Y; } }
        public int Z { get { return _Z; } }

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

            return another.X == _X && another.Y == _Y && another.Z == _Z;
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
    }
}
