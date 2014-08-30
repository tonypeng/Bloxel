/*
 * Bloxel - DeferredColoredChunkRenderer.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Bloxel.Engine.Cameras;
using Bloxel.Engine.Core;
using Bloxel.Engine.DataStructures;

using Bloxel.Engine.DeferredRendering.Effects;

namespace Bloxel.Engine.DeferredRendering
{
    public class DeferredColoredChunkRenderer : IChunkRenderer
    {
        private QuadRenderer _quadRenderer;

        private Vector2 _halfPixel;

        private BasicEffect basicEffect;
        private ClearEffect _clearEffect;
        private RenderGBufferColorEffect _renderGBufferEffect;
        private RenderCombineEffect _renderCombineEffect;
        private DirectionalLightEffect _directionalLightEffect;
        private PointLightEffect _pointLightEffect;
        private SSAOEffect _ssaoEffect;

        private SpriteFont _uiFontTiny;

        private RenderTarget2D _albedoTarget;
        private RenderTarget2D _lightTarget;
        private RenderTarget2D _normalTarget;
        private RenderTarget2D _depthTarget;

        private Texture2D _randomMap;

        private GraphicsDevice _device;
        private SpriteBatch _sb;

        private CameraManager _camManager;
        private IChunkManager _chunks;

        private ChunkRendererDebugOptions _debugOptions;

        private RasterizerState _debugRasterizerState;
        private RasterizerState _rasterizerState;

        public DeferredColoredChunkRenderer(EngineConfiguration config, ContentLibrary contentLibrary, GraphicsDevice device, CameraManager cameraManager, IChunkManager chunkManager)
        {
            _device = device;
            _sb = new SpriteBatch(_device);

            _halfPixel = new Vector2(0.5f / _device.PresentationParameters.BackBufferWidth, 0.5f / _device.PresentationParameters.BackBufferHeight);

            _quadRenderer = new QuadRenderer(_device);

            _clearEffect = new ClearEffect(contentLibrary.ClearEffect);
            _renderGBufferEffect = new RenderGBufferColorEffect(contentLibrary.RenderGBufferColorEffect);
            _renderCombineEffect = new RenderCombineEffect(contentLibrary.RenderCombineEffect);
            _directionalLightEffect = new DirectionalLightEffect(contentLibrary.DirectionalLightEffect);
            _pointLightEffect = new PointLightEffect(contentLibrary.PointLightEffect);
            _ssaoEffect = new SSAOEffect(contentLibrary.SSAOEffect);

            _camManager = cameraManager;
            _chunks = chunkManager;

            _debugOptions = ChunkRendererDebugOptions.NONE;

            basicEffect = contentLibrary.BasicEffect;

            _uiFontTiny = contentLibrary.UIFontTiny;

            _albedoTarget = new RenderTarget2D(_device, _device.PresentationParameters.BackBufferWidth, _device.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _lightTarget = new RenderTarget2D(_device, _device.PresentationParameters.BackBufferWidth, _device.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            _normalTarget = new RenderTarget2D(_device, _device.PresentationParameters.BackBufferWidth, _device.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            _depthTarget = new RenderTarget2D(_device, _device.PresentationParameters.BackBufferWidth, _device.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
            
            _randomMap = new Texture2D(_device, _device.PresentationParameters.BackBufferWidth, _device.PresentationParameters.BackBufferHeight);
            CreateRandomNormalTexture(_randomMap);

            _debugRasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None, FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame };
            _rasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace };
        }

        private void CreateRandomNormalTexture(Texture2D tex)
        {
            Random rand = new Random();

            Color[] result = new Color[tex.Width * tex.Height];
            Vector3 offset = Vector3.One * 0.5f;

            for (int i = 0; i < result.Length; i++)
            {
                Vector3 v3 = Vector3.Zero;
                // generate a random Z value in the interval [-1.0, 1.0)
                double z = rand.NextDouble() * 2.0 - 1.0;
                // subtract z^2 from the hypotenuse
                double r = Math.Sqrt(1.0 - z * z);
                // generate a random angle
                double angle = rand.NextDouble() * MathHelper.TwoPi;
                // elementary trig
                v3.X = (float)(r * Math.Cos(angle));
                v3.Y = (float)(r * Math.Sin(angle));
                v3.Z = (float)z;

                v3 += offset;
                v3 *= 0.5f;

                result[i] = new Color(v3);
            }

            tex.SetData<Color>(result);
        }

        public void ToggleDebugMode(ChunkRendererDebugOptions debugFlags)
        {
            _debugOptions ^= debugFlags;
        }

        private void ClearGBuffer()
        {
            _device.RasterizerState = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.Solid };

            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.None;

            _clearEffect.CurrentTechnique.Passes[0].Apply();

            _quadRenderer.DrawFullScreenQuad();
        }

        private void DrawGeometry()
        {
            _renderGBufferEffect.ColorFactor = 1.0f;
            _renderGBufferEffect.World = Matrix.Identity;
            _renderGBufferEffect.View = _camManager.MainCamera.View;
            _renderGBufferEffect.Projection = _camManager.MainCamera.Projection;

            _renderGBufferEffect.CurrentTechnique = _renderGBufferEffect.Techniques["RenderGBufferColor"];

            foreach (EffectPass pass in _renderGBufferEffect.CurrentTechnique.Passes)
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
        }

        private void DrawSSAO(float size, float intensity)
        {
            _device.RasterizerState = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.Solid };
            _device.BlendState = new BlendState() { ColorBlendFunction = BlendFunction.Add, ColorSourceBlend = Blend.One, ColorDestinationBlend = Blend.One };
            _device.DepthStencilState = DepthStencilState.None;
            
            Matrix viewProjection = _camManager.MainCamera.View * _camManager.MainCamera.Projection;

            _ssaoEffect.NoiseOffset = Vector2.Zero;
            _ssaoEffect.Intensity = intensity;
            _ssaoEffect.RandomMap = _randomMap;
            _ssaoEffect.NormalMap = _normalTarget;
            _ssaoEffect.DepthMap = _depthTarget;
            _ssaoEffect.ViewProjection = viewProjection;
            _ssaoEffect.ViewProjectionInverse = Matrix.Invert(viewProjection);

            _ssaoEffect.CurrentTechnique.Passes[0].Apply();
            _quadRenderer.DrawFullScreenQuad();
        }

        private void DrawDirectionalLight(Vector3 direction, Color c, float intensity)
        {
            Vector4 color4 = c.ToVector4();

            _device.RasterizerState = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.Solid };
            _device.BlendState = new BlendState() { ColorBlendFunction = BlendFunction.Add, ColorSourceBlend = Blend.One, ColorDestinationBlend = Blend.One };

            _device.DepthStencilState = DepthStencilState.None;

            _directionalLightEffect.LightDirection = direction;
            _directionalLightEffect.LightColor = color4;
            _directionalLightEffect.LightMaxIntensity = intensity;

            _directionalLightEffect.NormalMap = _normalTarget;
            _directionalLightEffect.ViewProjectionInverse = Matrix.Invert(_camManager.MainCamera.View * _camManager.MainCamera.Projection);

            _directionalLightEffect.CurrentTechnique.Passes[0].Apply();
            _quadRenderer.DrawFullScreenQuad();
        }

        private void DrawCombine()
        {
            _device.RasterizerState = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.Solid };

            _renderCombineEffect.Parameters["ColorMap"].SetValue(_albedoTarget);
            _renderCombineEffect.Parameters["LightMap"].SetValue(_lightTarget);

            _renderCombineEffect.CurrentTechnique.Passes[0].Apply();

            _device.BlendState = BlendState.Opaque;
            _quadRenderer.DrawFullScreenQuad();
        }

        private void DrawDebugRenderTargets()
        {
            _sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);

            _sb.Draw(_albedoTarget, new Rectangle(0, 0, _albedoTarget.Width / 5, _albedoTarget.Height / 5), Color.White);
            _sb.Draw(_lightTarget, new Rectangle(_albedoTarget.Width / 5, 0, _lightTarget.Width / 5, _lightTarget.Height / 5), Color.White);
            _sb.Draw(_normalTarget, new Rectangle(_albedoTarget.Width / 5 + _lightTarget.Width / 5, 0, _normalTarget.Width / 5, _normalTarget.Height / 5), Color.White);
            _sb.Draw(_randomMap, new Rectangle(_albedoTarget.Width / 5 + _lightTarget.Width / 5 + _normalTarget.Width / 5, 0, _randomMap.Width / 5, _randomMap.Height / 5), Color.White);

            if (Input.Input.Get().IsKeyDown(Keys.F5, true))
            {
                System.IO.FileStream fs = System.IO.File.Create("pic.png");

                _lightTarget.SaveAsPng(fs, _lightTarget.Width, _lightTarget.Height);

                fs.Flush();
                fs.Close();
                fs.Dispose();
            }

            _sb.DrawString(_uiFontTiny, "Albedo", new Vector2(2, 2), Color.White);
            _sb.DrawString(_uiFontTiny, "Light", new Vector2(_albedoTarget.Width / 5 + 2, 2), Color.White);
            _sb.DrawString(_uiFontTiny, "Normal", new Vector2(_albedoTarget.Width / 5 + _lightTarget.Width / 5 + 2, 2), Color.White);
            _sb.DrawString(_uiFontTiny, "Depth", new Vector2(_albedoTarget.Width / 5 + _lightTarget.Width / 5 + _normalTarget.Width / 5 + 2, 2), Color.White);

            _sb.End();
        }
        
        public void RenderAll()
        {
            _device.SetRenderTargets(_albedoTarget, _normalTarget, _depthTarget);

            ClearGBuffer();

            // back up values we're going to change
            BlendState preBlendstate = _device.BlendState;
            DepthStencilState preDepthStencilState = _device.DepthStencilState;

            // restore stuff that spritebatch messes up
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            _device.RasterizerState = _debugOptions.HasFlag(ChunkRendererDebugOptions.DEBUG_DRAW_WIREFRAME) ? _debugRasterizerState : _rasterizerState;

            DrawGeometry();

            _device.SetRenderTarget(_lightTarget);
            _device.Clear(new Color(0.0f, 0.0f, 0.0f, 0.0f));

            for (int i = 0; i < 1; i++)
            {
                DrawSSAO(1f, 0.25f);
            }

            //DrawDirectionalLight(new Vector3(1, 2, -3), Color.White, 0.75f);
            //DrawDirectionalLight(new Vector3(-1, -1, -4), Color.White, 0.5f);

            _device.SetRenderTarget(null);

            DrawCombine();

            // restore stuff
            _device.BlendState = BlendState.Opaque;
            _device.DepthStencilState = DepthStencilState.Default;
            _device.RasterizerState = _debugOptions.HasFlag(ChunkRendererDebugOptions.DEBUG_DRAW_WIREFRAME) ? _debugRasterizerState : _rasterizerState;

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

            if (_debugOptions.HasFlag(ChunkRendererDebugOptions.DEBUG_DRAW_RENDERTARGETS))
                DrawDebugRenderTargets();
        }

        public void Render(Chunk c)
        {
            if (c.VertexBuffer == null || c.IndexBuffer == null) return;

            /*
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

            // /*
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
            }//

            _device.BlendState = preBlendstate;
            _device.DepthStencilState = preDepthStencilState;*/
        }
    }
}
