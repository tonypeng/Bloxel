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
        private Effect _terrainColorEffect;
        private GraphicsDevice _device;

        private CameraManager _camManager;
        private IChunkManager _chunks;

        public ColoredChunkRenderer(EngineConfiguration config, ContentLibrary contentLibrary, GraphicsDevice device, CameraManager cameraManager, IChunkManager chunkManager)
        {
            _device = device;

            _terrainColorEffect = contentLibrary.TerrainColorEffect;

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
                        //if (!_camManager.MainCamera.ViewFrustrum.Intersects(c.BoundingBox))
                        //    continue;

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
            BlendState preBlendstate = _device.BlendState;
            DepthStencilState preDepthStencilState = _device.DepthStencilState;

            // restore stuff that spritebatch messes up
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            _device.RasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None, FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame };
            _device.RasterizerState = RasterizerState.CullNone;
            _device.RasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace };

            _terrainColorEffect.CurrentTechnique = _terrainColorEffect.Techniques["Solid"];

            _terrainColorEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            _terrainColorEffect.Parameters["xView"].SetValue(_camManager.MainCamera.View);
            _terrainColorEffect.Parameters["xProjection"].SetValue(_camManager.MainCamera.Projection);
            _terrainColorEffect.Parameters["CameraPosition"].SetValue(_camManager.MainCamera.Position);
            
            _terrainColorEffect.Parameters["FogBegin"].SetValue(48f);
            _terrainColorEffect.Parameters["FogEnd"].SetValue(160);
            _terrainColorEffect.Parameters["FogColor"].SetValue(Color.LightGray.ToVector4());

            _terrainColorEffect.Parameters["LightDirection"].SetValue(new Vector3(0.5f, -1f, 0.5f));
            _terrainColorEffect.Parameters["LightDirection2"].SetValue(new Vector3(-0.5f, -1f, -0.5f));

            foreach (EffectPass pass in _terrainColorEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _device.SetVertexBuffer(c.VertexBuffer);
                _device.Indices = c.IndexBuffer;

                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.VertexBuffer.VertexCount, 0, c.IndexBuffer.IndexCount / 3);
            }

            /*
            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = _camManager.MainCamera.View;
            basicEffect.Projection = _camManager.MainCamera.Projection;
            basicEffect.LightingEnabled = false;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _device.SetVertexBuffer(c.NormalsVertexBuffer);
                _device.DrawPrimitives(PrimitiveType.LineList, 0, c.NormalsVertexBuffer.VertexCount / 2);
            }*/

            _device.BlendState = preBlendstate;
            _device.DepthStencilState = preDepthStencilState;
        }
    }
}
