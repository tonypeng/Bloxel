/*
 * Bloxel - ColoredChunkRenderer.cs
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
using Bloxel.Engine.DataStructures;

namespace Bloxel.Engine.Core
{
    public class ColoredChunkRenderer : IChunkRenderer
    {
        private BasicEffect basicEffect;
        private GraphicsDevice GraphicsDevice;

        private CameraManager _camManager;
        private IChunkManager _chunks;

        public ColoredChunkRenderer(EngineConfiguration config, ContentLibrary contentLibrary, GraphicsDevice device, CameraManager cameraManager, IChunkManager chunkManager)
        {
            GraphicsDevice = device;

            _camManager = cameraManager;
            _chunks = chunkManager;

            basicEffect = contentLibrary.BasicEffect;
        }

        public void RenderAll()
        {
            for (int x = _chunks.MinimumX; x <= _chunks.MaximumX; x++)
            {
                for (int y = _chunks.MinimumY; y <= _chunks.MaximumY; y++)
                {
                    for (int z = _chunks.MinimumZ; z <= _chunks.MaximumZ; z++)
                    {
                        Chunk c = _chunks[x, y, z];

                        // do we even need to render this?
                        if (!_camManager.MainCamera.ViewFrustrum.Intersects(c.BoundingBox))
                            continue;

                        if (c.VertexBuffer == null || c.IndexBuffer == null)
                            continue;

                        Render(c);
                    }
                }
            }
        }

        public void Render(Chunk c)
        {
            // back up values we're going to change
            BlendState preBlendstate = GraphicsDevice.BlendState;
            DepthStencilState preDepthStencilState = GraphicsDevice.DepthStencilState;

            // restore stuff that spritebatch messes up
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None, FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame };
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace };

            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = _camManager.MainCamera.View;
            basicEffect.Projection = _camManager.MainCamera.Projection;
            basicEffect.EnableDefaultLighting();
            basicEffect.PreferPerPixelLighting = true;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.SetVertexBuffer(c.VertexBuffer);
                GraphicsDevice.Indices = c.IndexBuffer;

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.VertexBuffer.VertexCount, 0, c.IndexBuffer.IndexCount / 3);
            }

            basicEffect.LightingEnabled = false;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.SetVertexBuffer(c.NormalsVertexBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, c.NormalsVertexBuffer.VertexCount / 2);
            }

            GraphicsDevice.BlendState = preBlendstate;
            GraphicsDevice.DepthStencilState = preDepthStencilState;
        }
    }
}
