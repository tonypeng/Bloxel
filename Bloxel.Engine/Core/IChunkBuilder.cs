/*
 * Bloxel - IChunkBuilder.cs
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

using Bloxel.Engine.DataStructures;

namespace Bloxel.Engine.Core
{
    public interface IChunkBuilder
    {
        bool RequiresPostProcess { get; }

        void Build(Chunk c);
        void PostProcess(Chunk c);
    }
}
