/*
 * Bloxel - DirectionalLightEffect.cs
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
    public class DirectionalLightEffect : Effect
    {
        public DirectionalLightEffect(Effect cloneSource)
            : base(cloneSource)
        { }

        public Vector3 LightDirection { get; set; }
        public Vector4 LightColor { get; set; }
        public float LightMaxIntensity { get; set; }

        public Texture2D NormalMap { get; set; }
        public Matrix ViewProjectionInverse { get; set; }

        protected override void OnApply()
        {
            Parameters["NormalMap"].SetValue(NormalMap);
            Parameters["ViewProjectionInverse"].SetValue(ViewProjectionInverse);
            Parameters["LightDirection"].SetValue(LightDirection);
            Parameters["LightColor"].SetValue(LightColor);
            Parameters["LightMaxIntensity"].SetValue(LightMaxIntensity);
        }
    }
}