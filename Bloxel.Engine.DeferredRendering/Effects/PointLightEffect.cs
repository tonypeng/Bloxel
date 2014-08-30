/*
 * Bloxel - PointLightEffect.cs
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
    public class PointLightEffect : Effect
    {
        public PointLightEffect(Effect cloneSource)
            : base(cloneSource)
        { }

        public Matrix ModelViewProjection { get; set; }
        public Matrix ViewProjectionInverse { get; set; }

        public Vector2 HalfPixel { get; set; }
        public Vector3 LightPosition { get; set; }
        public Vector4 LightColor { get; set; }
        public float LightIntensity { get; set; }
        public float LightRange { get; set; }

        public Texture2D NormalMap { get; set; }
        public Texture2D DepthMap { get; set; }

        protected override void OnApply()
        {
            Parameters["HalfPixel"].SetValue(HalfPixel);
            Parameters["ModelViewProjection"].SetValue(ModelViewProjection);
            Parameters["ViewProjectionInverse"].SetValue(ViewProjectionInverse);
            Parameters["DepthMap"].SetValue(DepthMap);
            Parameters["NormalMap"].SetValue(NormalMap);
            Parameters["LightPosition"].SetValue(LightPosition);
            Parameters["LightColor"].SetValue(LightColor);
            Parameters["LightIntensity"].SetValue(LightIntensity);
            Parameters["LightRange"].SetValue(LightRange);
        }
    }
}