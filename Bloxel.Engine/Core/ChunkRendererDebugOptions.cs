/*
 * Bloxel - ChunkRendererDebugOptions.cs
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

namespace Bloxel.Engine.Core
{
    [Flags]
    public enum ChunkRendererDebugOptions
    {
        NONE = 0,
        DEBUG_DRAW_WIREFRAME = 1,
        DEBUG_DRAW_NORMALS = 2,
        DEBUG_DRAW_RENDERTARGETS = 4,
    }
}
