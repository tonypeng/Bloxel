/*
 * Bloxel - CubeInfo.cs
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
