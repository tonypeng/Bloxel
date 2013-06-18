/*
 * Bloxel - DualContourColoredChunkSystem.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Bloxel.Engine.Cameras;

namespace Bloxel.Engine.Core
{
    public class DualContourColoredChunkSystem : IChunkSystem
    {
        private IChunkBuilder _builder;
        private IChunkRenderer _renderer;

        public IChunkBuilder Builder { get { return _builder; } }
        public IChunkRenderer Renderer { get { return _renderer; } }

        public DualContourColoredChunkSystem(GraphicsDevice device, ContentLibrary contentLibrary, IChunkManager chunkManager, CameraManager cameraManager, World world, ITerrainGradientFunction densityGradientFunction, float minimumSolidDensity)
        {
            _builder = new DualContourChunkBuilder(device, world, densityGradientFunction, minimumSolidDensity);
            _renderer = new ColoredChunkRenderer(world.EngineConfiguration, contentLibrary, device, cameraManager, chunkManager);
        }
    }
}
