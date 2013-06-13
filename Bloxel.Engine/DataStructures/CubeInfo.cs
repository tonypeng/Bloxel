/*
 * Bloxel - QEFVertex.cs
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
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Bloxel.Engine.DataStructures
{
    public struct CubeInfo
    {
        private Vector3 _vertexPos;
        private ShortBitfield _edgeChanges;

        public CubeInfo(Vector3 pos, params int[] edgeChanges)
        {
            _vertexPos = pos;

            _edgeChanges = new ShortBitfield(0);

            for (int i = 0; i < edgeChanges.Length; i++)
            {
                _edgeChanges.Set(edgeChanges[i], 1, 1);
            }
        }

        public Vector3 VertexPosition { get { return _vertexPos; } set { _vertexPos = value; } }
        public ShortBitfield EdgeChanges { get { return _edgeChanges; } }
    }
}
