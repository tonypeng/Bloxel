/*
 * Bloxel - VertexPositionNormalColor.cs
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

namespace Bloxel.Engine.DataStructures
{
    public struct VertexPositionNormalColorLight : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;
        public float Light;

        public VertexPositionNormalColorLight(Vector3 position, Vector3 normal, Color color, float light)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
            this.Light = light;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(sizeof(float) * 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 7, VertexElementFormat.Single, VertexElementUsage.Color, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionNormalColorLight.VertexDeclaration; }
        }
    }
}
