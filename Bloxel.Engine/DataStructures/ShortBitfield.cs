/*
 * Bloxel - ShortBitfield.cs
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
