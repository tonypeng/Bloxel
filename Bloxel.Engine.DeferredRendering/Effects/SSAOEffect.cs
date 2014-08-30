/*
 * Bloxel - SSAOEffect.cs
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bloxel.Engine.DeferredRendering.Effects
{
    public class SSAOEffect : Effect
    {
        public float Intensity { get; set; }

        public SSAOEffect(Effect cloneSource)
            : base(cloneSource)
        { }

        public Texture2D NormalMap { get; set; }
        public Texture2D DepthMap { get; set; }
        public Matrix ViewProjectionInverse { get; set; }

        public Matrix ViewProjection { get; set; }

        public Texture2D RandomMap { get; set; }

        public Vector2 NoiseOffset { get; set; }

        public float Size { get; set; }

        protected override void OnApply()
        {
            Parameters["Size"].SetValue(Size);
            Parameters["NoiseOffset"].SetValue(NoiseOffset);
            Parameters["Intensity"].SetValue(Intensity);
            Parameters["RandomMap"].SetValue(RandomMap);
            Parameters["NormalMap"].SetValue(NormalMap);
            Parameters["DepthMap"].SetValue(DepthMap);
            Parameters["ViewProjectionInverse"].SetValue(ViewProjectionInverse);
            Parameters["ViewProjection"].SetValue(ViewProjection);
        }
    }
}
