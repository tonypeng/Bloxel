/*
 * Bloxel - RenderCombineEffect.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
 */

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bloxel.Engine.DeferredRendering.Effects
{
    public class RenderCombineEffect : Effect
    {
        public RenderCombineEffect(Effect cloneSource)
            : base(cloneSource)
        { }
    }
}