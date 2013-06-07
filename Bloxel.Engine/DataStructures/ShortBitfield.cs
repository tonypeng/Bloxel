/*
 * Bloxel - ShortBitfield.cs
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
    public struct ShortBitfield
    {
        private ushort _bitfield;

        public ShortBitfield(ushort bitfield)
        {
            _bitfield = bitfield;
        }

        public void Set(int index, ushort s, int length)
        {
            ushort resetMask = 0xffff;
            resetMask = (ushort)((ushort)(resetMask << (16 - length)) >> (16 - length - index));
            resetMask = (ushort)~resetMask;

            _bitfield &= resetMask;

            s <<= 16 - length;
            s >>= 16 - length - index;

            _bitfield |= s;
        }

        public ushort Get(int index, int length)
        {
            ushort mask = 0xffff;
            mask = (ushort)((ushort)(mask << (16 - length)) >> (16 - length - index));

            return (ushort)((ushort)(_bitfield & mask) >> (index));
        }

        public ushort Bitfield { get { return _bitfield; } set { _bitfield = value; } }

        public static implicit operator ShortBitfield(ushort s)
        {
            return new ShortBitfield(s);
        }

        public static implicit operator UInt16(ShortBitfield s)
        {
            return s._bitfield;
        }
    }
}
