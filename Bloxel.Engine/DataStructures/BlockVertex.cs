/*
 * Bloxel - BlockVertex.cs
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
    public enum BlockVertex : byte
    {
        XYZPositive = 0,
        XYZ = 1,
        XYPositiveZ = 2,
        XYPositiveZPositive = 3,
        XPositiveYZPositive = 4,
        XPositiveYZ = 5,
        XPositiveYPositiveZ = 6,
        XPositiveYPositiveZPositive = 7,
        TotalVertices = 8,
    }
}
