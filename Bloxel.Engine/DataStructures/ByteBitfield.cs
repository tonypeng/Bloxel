/*
 * Bloxel - ByteBitfield.cs
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

 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.

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
    public struct ByteBitfield
    {
        private byte _bitfield;

        public ByteBitfield(byte bitfield)
        {
            _bitfield = bitfield;
        }

        public void Set(int index, byte b, int length)
        {
            byte resetMask = 0xff;
            resetMask = (byte)((byte)(resetMask << (8 - length)) >> (8 - length - index));
            resetMask = (byte)~resetMask;

            _bitfield &= resetMask;

            b <<= 8 - length;
            b >>= 8 - length - index;

            _bitfield |= b;
        }

        public byte Get(int index, int length)
        {
            byte mask = 0xff;
            mask = (byte)((byte)(mask << (8 - length)) >> (8 - length - index));

            return (byte)((byte)(_bitfield & mask) >> (index));
        }

        public byte Bitfield { get { return _bitfield; } set { _bitfield = value; } }

        public static implicit operator ByteBitfield(byte b)
        {
            return new ByteBitfield(b);
        }

        public static implicit operator Byte(ByteBitfield b)
        {
            return b._bitfield;
        }
    }
}
