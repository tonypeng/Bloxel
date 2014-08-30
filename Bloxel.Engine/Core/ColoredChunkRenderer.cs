/*
 * Bloxel - ColoredChunkRenderer.cs
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
using Bloxel.Engine.DataStructures;

namespace Bloxel.Engine.Core
{
    public class ColoredChunkRenderer : IChunkRenderer
    {
        private EngineConfiguration _config;

        private BasicEffect basicEffect;
        private Effect _terrainColorEffect;
        private GraphicsDevice _device;

        private CameraManager _camManager;
        private IChunkManager _chunks;

        private ChunkRendererDebugOptions _debugOptions;

        private RasterizerState _debugRasterizerState;
        private RasterizerState _rasterizerState;

        public ColoredChunkRenderer(EngineConfiguration config, ContentLibrary contentLibrary, GraphicsDevice device, CameraManager cameraManager, IChunkManager chunkManager)
        {
            _config = config;

            _device = device;

            _terrainColorEffect = contentLibrary.TerrainColorEffect;

            _camManager = cameraManager;
            _chunks = chunkManager;

            _debugOptions = ChunkRendererDebugOptions.NONE;

            basicEffect = contentLibrary.BasicEffect;

            _debugRasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None, FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame };
            _rasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace };
        }

        public void ToggleDebugMode(ChunkRendererDebugOptions debugFlags)
        {
            _debugOptions ^= debugFlags;
        }

        public void RenderAll()
        {
            // back up values we're going to change
            BlendState preBlendstate = _device.BlendState;
            DepthStencilState preDepthStencilState = _device.DepthStencilState;

            // restore stuff that spritebatch messes up
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            _device.RasterizerState = _debugOptions.HasFlag(ChunkRendererDebugOptions.DEBUG_DRAW_WIREFRAME) ? _debugRasterizerState : _rasterizerState;

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

            _terrainColorEffect.Parameters["CPULightingEnabled"].SetValue(_config.CPULightingEnabled);

            foreach (EffectPass pass in _terrainColorEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                for (int x = _chunks.MinimumX; x <= _chunks.MaximumX; x++)
                {
                    for (int y = _chunks.MinimumY; y <= _chunks.MaximumY; y++)
                    {
                        for (int z = _chunks.MinimumZ; z <= _chunks.MaximumZ; z++)
                        {
                            Chunk c = _chunks[x, y, z];

                            if (c.VertexBuffer == null || c.IndexBuffer == null)
                                continue;

                            // do we even need to render this?
                            if (!_camManager.MainCamera.ViewFrustrum.Intersects(c.BoundingBox))
                                continue;

                            _device.SetVertexBuffer(c.VertexBuffer);
                            _device.Indices = c.IndexBuffer;

                            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.VertexBuffer.VertexCount, 0, c.IndexBuffer.IndexCount / 3);
                        }
                    }
                }
            }

            if (_debugOptions.HasFlag(ChunkRendererDebugOptions.DEBUG_DRAW_NORMALS))
            {
                for (int x = _chunks.MinimumX; x <= _chunks.MaximumX; x++)
                {
                    for (int y = _chunks.MinimumY; y <= _chunks.MaximumY; y++)
                    {
                        for (int z = _chunks.MinimumZ; z <= _chunks.MaximumZ; z++)
                        {
                            Chunk c = _chunks[x, y, z];

                            if (c.NormalsVertexBuffer == null) continue;

                            // do we even need to render this?
                            if (!_camManager.MainCamera.ViewFrustrum.Intersects(c.BoundingBox))
                                continue;

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
                            }
                        }
                    }
                }
            }

            _device.BlendState = preBlendstate;
            _device.DepthStencilState = preDepthStencilState;
        }

        public void Render(Chunk c)
        {
            if (c.VertexBuffer == null || c.IndexBuffer == null) return;

            // back up values we're going to change
            BlendState preBlendstate = _device.BlendState;
            DepthStencilState preDepthStencilState = _device.DepthStencilState;

            // restore stuff that spritebatch messes up
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            _device.RasterizerState = _debugOptions.HasFlag(ChunkRendererDebugOptions.DEBUG_DRAW_WIREFRAME) ? _debugRasterizerState : _rasterizerState;

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
