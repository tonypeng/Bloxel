/*
 * Bloxel - QuadRenderer.cs
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

namespace Bloxel.Engine.DeferredRendering
{
    public class QuadRenderer
    {
        public QuadRenderer(GraphicsDevice device)
        {
            this.device = device;

            vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(0,0,0),new Vector2(1,1)),
                new VertexPositionTexture(new Vector3(0,0,0),new Vector2(0,1)),
                new VertexPositionTexture(new Vector3(0,0,0),new Vector2(0,0)),
                new VertexPositionTexture(new Vector3(0,0,0),new Vector2(1,0))
            };

            indexBuffer = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        public void Draw(Vector2 v1, Vector2 v2)
        {
            vertices[0].Position.X = v2.X;
            vertices[0].Position.Y = v1.Y;

            vertices[1].Position.X = v1.X;
            vertices[1].Position.Y = v1.Y;

            vertices[2].Position.X = v1.X;
            vertices[2].Position.Y = v2.Y;

            vertices[3].Position.X = v2.X;
            vertices[3].Position.Y = v2.Y;

            device.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, 4, indexBuffer, 0, 2);
        }

        public void DrawFullScreenQuad()
        {
            Draw(Vector2.One * -1, Vector2.One);
        }

        #region FieldsAndProperties
        private VertexPositionTexture[] vertices = null;
        private short[] indexBuffer = null;
        private GraphicsDevice device;
        #endregion
    }
}
