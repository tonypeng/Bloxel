/*
 * Bloxel - Direction.cs
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
    public enum Direction : byte
    {
        XIncreasing = 0,
        XDecreasing = 1,
        ZIncreasing = 2,
        ZDecreasing = 3,
        YIncreasing = 4,
        YDecreasing = 5
    }
}
