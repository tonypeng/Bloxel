/*
 * Bloxel - BoundingBoxRenderer.cs
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

namespace Playground
{
    public static class BoundingBoxRenderer
    {
        static VertexPositionColor[] vertices = new VertexPositionColor[8];

        static short[] indices = new short[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4
        };

        static BasicEffect _effect;

        public static void Render(GraphicsDevice device, BoundingBox aabb, Camera cam) { Render(device, aabb, Color.Gold, cam); }

        public static void Render(GraphicsDevice device, BoundingBox aabb, Color c, Camera cam)
        {
            if (_effect == null)
            {
                _effect = new BasicEffect(device);
                _effect.VertexColorEnabled = true;
                _effect.LightingEnabled = false;
            }

            Vector3[] corners = aabb.GetCorners();

            for (int i = 0; i < 8; i++)
            {
                vertices[i].Position = corners[i];
                vertices[i].Color = c;
            }

            // back up values we're going to change
            BlendState preBlendstate = device.BlendState;
            DepthStencilState preDepthStencilState = device.DepthStencilState;

            // restore stuff that spritebatch messes up
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.RasterizerState = RasterizerState.CullNone;

            _effect.View = cam.View;
            _effect.Projection = cam.Projection;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertices,
                    0,
                    8,
                    indices,
                    0,
                    indices.Length / 2);
            }

            device.BlendState = preBlendstate;
            device.DepthStencilState = preDepthStencilState;
        }
    }
}
