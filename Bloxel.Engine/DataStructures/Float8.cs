/*
 * Bloxel - Float8.cs
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
