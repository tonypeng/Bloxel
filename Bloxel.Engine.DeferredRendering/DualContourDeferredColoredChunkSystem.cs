/*
 * Bloxel - DualContourDeferredColoredChunkSystem.cs
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Bloxel.Engine.Cameras;
using Bloxel.Engine.Core;

namespace Bloxel.Engine.DeferredRendering
{
    public class DualContourDeferredColoredChunkSystem : IChunkSystem
    {
        private IChunkBuilder _builder;
        private IChunkRenderer _renderer;

        public IChunkBuilder Builder { get { return _builder; } }
        public IChunkRenderer Renderer { get { return _renderer; } }

        public DualContourDeferredColoredChunkSystem(GraphicsDevice device, ContentLibrary contentLibrary, IChunkManager chunkManager, CameraManager cameraManager, World world, ITerrainGradientFunction densityGradientFunction, float minimumSolidDensity)
        {
            _builder = new DualContourChunkBuilder(device, world, densityGradientFunction, minimumSolidDensity);
            _renderer = new DeferredColoredChunkRenderer(world.EngineConfiguration, contentLibrary, device, cameraManager, chunkManager);
        }
    }
}
