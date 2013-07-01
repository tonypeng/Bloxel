/*
 * Bloxel - DualContourModification.cs
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
    public enum DualContourModification : byte
    {
        /// <summary>
        /// Describes a natural modification.  The gradient vector should be calculated with a suitable ITerrainGradientFunction.
        /// </summary>
        NATURAL = 0,
        /// <summary>
        /// Describes a modification that is intended to create a cube.
        /// </summary>
        CUBE = 1,
        SLANT_15 = 2,
        SLANT_30 = 3,
        SLANT_45 = 4,
        SLANT_60 = 5,
        SLANT_75 = 6,
        SPHERE = 7,
    }
}
