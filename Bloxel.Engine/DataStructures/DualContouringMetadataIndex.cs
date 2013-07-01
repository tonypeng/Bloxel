/*
 * Bloxel - DualContouringMetadataIndex.cs
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
    public enum DualContouringMetadataIndex : byte
    {
        // Lighting = 0
        Normal_XPositive_X = 1,
        Normal_XPositive_Y = 2,
        Normal_XPositive_Z = 3,
        Normal_YPositive_X = 4,
        Normal_YPositive_Y = 5,
        Normal_YPositive_Z = 6,
        Normal_ZPositive_X = 7,
        Normal_ZPositive_Y = 8,
        Normal_ZPositive_Z = 9,
        Length = 10
    }
}
