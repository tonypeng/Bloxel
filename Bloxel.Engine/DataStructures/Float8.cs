/*
 * Bloxel - Float8.cs
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
    public struct Float8
    {
        private sbyte _packedValue;

        public Float8(float f)
        {
            if (f > 1.0f) f = 1.0f;
            if (f < -1.0f) f = -1.0f;

            _packedValue = (sbyte)(f * 127);
        }

        public Float8(byte packed)
            : this((sbyte)packed)
        { }

        public Float8(sbyte packed)
        {
            _packedValue = packed;
        }

        public SByte PackedValue { get { return _packedValue; } }

        public float ToSingle() { return _packedValue / 127f; }

        public static Float8 operator +(Float8 f1, Float8 f2)
        {
            return new Float8(f1.ToSingle() + f2.ToSingle());
        }

        public static Float8 operator -(Float8 f1, Float8 f2)
        {
            return new Float8(f1.ToSingle() - f2.ToSingle());
        }

        public static Single operator +(Float8 f8, Single f32)
        {
            return f8.ToSingle() + f32;
        }

        public static Single operator +(Single f32, Float8 f8)
        {
            return f32 + f8.ToSingle();
        }

        public static Single operator -(Float8 f8, Single f32)
        {
            return f8.ToSingle() - f32;
        }

        public static Single operator -(Single f32, Float8 f8)
        {
            return f32 - f8.ToSingle();
        }

        public static Single operator *(Float8 f8, Single f32)
        {
            return f8.ToSingle() * f32;
        }

        public static Single operator *(Single f32, Float8 f8)
        {
            return f8 * f32;
        }

        public static Float8 operator *(Float8 f1, Float8 f2)
        {
            return new Float8(f1.ToSingle() * f2.ToSingle());
        }

        public static Single operator /(Float8 f8, Single f32)
        {
            return f8.ToSingle() / f32;
        }

        public static Single operator /(Single f32, Float8 f8)
        {
            return f32 / f8.ToSingle();
        }

        public static Float8 operator /(Float8 f1, Float8 f2)
        {
            return new Float8(f1.ToSingle() / f2.ToSingle());
        }

        public static implicit operator Float8(Single f32)
        {
            return new Float8(f32);
        }

        public static implicit operator Single(Float8 f8)
        {
            return f8.ToSingle();
        }
    }
}
