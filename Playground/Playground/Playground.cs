using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Bloxel.Engine.Input;

namespace Playground
{
    public class Playground : BloxelHost
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera cam;
        Chunk chunk;

        SpriteFont font;
        BasicEffect basicEffect;
        SphereDensityFunction sdf;

        FPSCounter _fpsCounter;

        bool _paused = false;

        public Playground()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            cam = new FreeCamera();

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            cam.Initialize(Vector3.Up, (float)(70f * Math.PI / 180), GraphicsDevice.Viewport.AspectRatio, 0.01f, 140 * 5,
                Vector3.Zero, Vector3.Forward);

            Input.Create(this);
            Input.Get().Update();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            basicEffect = new BasicEffect(GraphicsDevice);

            font = Content.Load<SpriteFont>("Arial");

            _fpsCounter = new FPSCounter(this, font);
            _fpsCounter.LoadContent();

            Console.WriteLine("Creating chunk...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            chunk = new Chunk(null, Vector3I.Zero, 16, 128, 16);
            sdf = new SphereDensityFunction(new Vector3(8, 8, 8), 6f);
            DensityChunkGenerator dcg = new DensityChunkGenerator(sdf);
            dcg.Generate(chunk);
            sw.Stop();
            Console.WriteLine("Done in {0}ms.", sw.ElapsedMilliseconds);
            Console.WriteLine("Density at (7, 7, 7): {0}", chunk.PointAt(7, 7, 7).Density);
            Console.WriteLine("Building chunk mesh...");
            DualContourChunkBuilder dccb = new DualContourChunkBuilder(GraphicsDevice, sdf, 0.0f);
            sw.Restart();
            dccb.Build(chunk);
            sw.Stop();
            Console.WriteLine("Done in {0}ms.", sw.Elapsed.TotalMilliseconds);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Get().Update();

            if (Input.Get().IsKeyDown(Keys.Escape, true))
                _paused = !_paused;

            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            if (!_paused)
            {
                cam.Yaw += -(Input.Get().MouseXCoordinate() - centerX) * cam.RotateSpeed;
                cam.Pitch += -(Input.Get().MouseYCoordinate() - centerY) * cam.RotateSpeed;

                Mouse.SetPosition(centerX, centerY);
            }

            Vector3 moveVector = Vector3.Zero;

            if (Input.Get().IsKeyDown(Keys.LeftControl, true))
            {
                cam.Position = new Vector3((int)cam.Position.X, (int)cam.Position.Y, (int)cam.Position.Z);
                cam.Yaw = (int)(cam.Yaw / MathHelper.PiOver2) * MathHelper.PiOver2;
                cam.Pitch = (int)(cam.Pitch / MathHelper.PiOver2) * MathHelper.PiOver2;
            }

            bool shouldBeNew = Input.Get().IsKeyDown(Keys.LeftControl);

            if (Input.Get().IsKeyDown(Keys.W, shouldBeNew))
            {
                moveVector += cam.Forward * 0.5f;
            }
            if (Input.Get().IsKeyDown(Keys.S, shouldBeNew))
            {
                moveVector -= cam.Forward * 0.5f;
            }
            if (Input.Get().IsKeyDown(Keys.D, shouldBeNew))
            {
                moveVector += cam.Right * 0.5f;
            }
            if (Input.Get().IsKeyDown(Keys.A, shouldBeNew))
            {
                moveVector -= cam.Right * 0.5f;
            }
            if (Input.Get().IsKeyDown(Keys.E, shouldBeNew))
            {
                moveVector += Vector3.Up * 0.5f;
            }
            if (Input.Get().IsKeyDown(Keys.Q, shouldBeNew))
            {
                moveVector -= Vector3.Up * 0.5f;
            }
            if (Input.Get().IsKeyDown(Keys.Tab))
            {
                moveVector *= 0.25f;
            }

            moveVector *= (float)gameTime.ElapsedGameTime.TotalSeconds / (1f / 60f);

            if (Input.Get().IsKeyDown(Keys.Right, true))
                cam.Yaw -= MathHelper.PiOver2;
            if (Input.Get().IsKeyDown(Keys.Left, true))
                cam.Yaw += MathHelper.PiOver2;

            cam.Position += moveVector;

            cam.Update();

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // back up values we're going to change
            BlendState preBlendstate = GraphicsDevice.BlendState;
            DepthStencilState preDepthStencilState = GraphicsDevice.DepthStencilState;

            // restore stuff that spritebatch messes up
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None, FillMode = Microsoft.Xna.Framework.Graphics.FillMode.WireFrame };
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace };

            basicEffect.VertexColorEnabled = true;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = cam.View;
            basicEffect.Projection = cam.Projection;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                GraphicsDevice.Indices = chunk.IndexBuffer;

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.VertexBuffer.VertexCount, 0, chunk.IndexBuffer.IndexCount / 3);

                GraphicsDevice.SetVertexBuffer(chunk.NormalsVertexBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, chunk.NormalsVertexBuffer.VertexCount / 2);
            }

            for (int x = 5; x < 11; x++)
            {
                for (int y = 5; y < 11; y++)
                {
                    for (int z = 5; z < 11; z++)
                    {
                            BoundingBoxRenderer.Render(GraphicsDevice, new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1)), Color.Gold, cam);
                    }
                }
            }

            GraphicsDevice.BlendState = preBlendstate;
            GraphicsDevice.DepthStencilState = preDepthStencilState;

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Target: " + cam.Target, Vector2.One * 2f, Color.White);
            spriteBatch.DrawString(font, "Position: " + cam.Position, new Vector2(2, 2 + font.LineSpacing + 2), Color.White);

            if (_paused)
            {
                Vector2 dim = font.MeasureString("Paused");

                spriteBatch.DrawString(font, "Paused", new Vector2(GraphicsDevice.Viewport.Width - dim.X - 2, 2), Color.White);
            }

            spriteBatch.End();

            _fpsCounter.Draw();

            base.Draw(gameTime);
        }
    }
}
