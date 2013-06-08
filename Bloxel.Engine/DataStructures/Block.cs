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

namespace Bloxel.Engine.DataStructures
{
    public struct Block
    {
        private byte _blockType;
        private Density _density;
        private ByteBitfield[] _lightingValues;

        public float Density
        {
            get { return _density.ToSingle(); }
            set
            {
                Contract.Assert(value >= 0.0f && value <= 1.0f);
                
                _density.Set(value);
            }
        }

        public Block(byte blockType, float density)
            : this(blockType, density, new ByteBitfield[8])
        { }

        public Block(byte blockType, float density, ByteBitfield[] b)
        {
            _blockType = blockType;
            _density = new Density(density);
            _lightingValues = b;
        }

        public byte Type { get { return _blockType; } }

        public byte LightAt(BlockVertex vertex)
        {
            ByteBitfield pair = _lightingValues[(byte)vertex / 2];
            int index = ((byte)vertex % 2) * 4;

            return pair.Get(index, 4);
        }

        public void SetLightAt(BlockVertex vertex, byte value)
        {
            if (value > 15)
                throw new ArgumentOutOfRangeException("Light value must be in the range [0, 15]!");

            ByteBitfield pair = _lightingValues[(byte)vertex / 2];
            int index = ((byte)vertex % 2) * 4;

            pair.Set(index, value, 4);
        }
    }
}
