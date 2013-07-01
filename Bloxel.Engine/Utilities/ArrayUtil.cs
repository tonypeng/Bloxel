/*
 * Bloxel - ArrayUtil.cs
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

namespace Bloxel.Engine.Utilities
{
    public static class ArrayUtil
    {
        public static int Convert3DTo1D(int x, int y, int z, int length, int height)
        {
            return x * length * height + z * height + y;
        }

        public static Tuple<int, int, int> Convert1DTo3D(int index, int length, int height)
        {
            int x = index / (length * height);
            int remainder1 = index % (length * height);
            int z = remainder1 / height;
            int y = remainder1 % height;

            return new Tuple<int, int, int>(x, y, z);
        }
    }
}
