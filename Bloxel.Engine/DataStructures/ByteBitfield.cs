/*
 * Bloxel - ByteBitfield.cs
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
