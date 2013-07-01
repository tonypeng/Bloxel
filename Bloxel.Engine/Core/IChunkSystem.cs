/*
 * Bloxel - IChunkSystem.cs
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
    /// <summary>
    /// Represents a system of building and rendering chunks.
    /// Different builders may output different graphical data, in which case an associated
    /// renderer is required.
    /// </summary>
    public interface IChunkSystem
    {
        IChunkBuilder Builder { get; }
        IChunkRenderer Renderer { get; }
    }
}
