/*
 * Bloxel - ContentLibrary.cs
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

namespace Bloxel.Engine.Core
{
    public class ContentLibrary
    {
        private readonly string BL_DIR = "Bloxel";

        // effects
        public BasicEffect BasicEffect;
        public Effect TerrainColorEffect;
        public Effect ClearEffect;
        public Effect DirectionalLightEffect;
        public Effect PointLightEffect;
        public Effect RenderCombineEffect;
        public Effect RenderGBufferColorEffect;
        public Effect SSAOEffect;

        // textures
        public Texture2D DummyPixel;

        // fonts
        public SpriteFont UIFontTiny;

        public void Load(GraphicsDevice device, ContentManager Content)
        {
            BasicEffect = new BasicEffect(device);

            TerrainColorEffect = Content.Load<Effect>(BL_DIR + "/Effects/TerrainColor");

            DummyPixel = new Texture2D(device, 1, 1);
            DummyPixel.SetData<Color>(new Color[] { Color.White });

            ClearEffect = Content.Load<Effect>(BL_DIR + "/Effects/Deferred/Clear");
            DirectionalLightEffect = Content.Load<Effect>(BL_DIR + "/Effects/Deferred/DirectionalLight");
            PointLightEffect = Content.Load<Effect>(BL_DIR + "/Effects/Deferred/PointLight");
            RenderCombineEffect = Content.Load<Effect>(BL_DIR + "/Effects/Deferred/RenderCombine");
            RenderGBufferColorEffect = Content.Load<Effect>(BL_DIR + "/Effects/Deferred/RenderGBufferColor");
            SSAOEffect = Content.Load<Effect>(BL_DIR + "/Effects/Deferred/SSAO");

            UIFontTiny = Content.Load<SpriteFont>(BL_DIR + "/Fonts/UIFontTiny");
        }
    }
}
