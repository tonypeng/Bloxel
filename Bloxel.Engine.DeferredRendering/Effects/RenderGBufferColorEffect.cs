/*
 * Bloxel - RenderGBufferColorEffect.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bloxel.Engine.DeferredRendering.Effects
{
    public class RenderGBufferColorEffect : Effect
    {
        public RenderGBufferColorEffect(Effect cloneSource)
            : base(cloneSource)
        { }

        public Matrix World { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public Texture2D Texture { get; set; }

        public float ColorFactor { get; set; }

        protected override void OnApply()
        {
            Matrix wt = Matrix.Invert(World);
            Matrix wit = Matrix.Transpose(wt);

            Parameters["ColorFactor"].SetValue(ColorFactor);
            Parameters["ColorMap"].SetValue(Texture);
            Parameters["WorldViewProjection"].SetValue(World * View * Projection);
            Parameters["WorldInverseTranspose"].SetValue(wit);
        }
    }
}