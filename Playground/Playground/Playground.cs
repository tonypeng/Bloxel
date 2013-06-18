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
using Bloxel.Engine.Utilities;

namespace Playground
{
    public class Playground : BloxelHost
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        CameraManager camManager;
        Camera cam;
        Chunk chunk;

        SpriteFont font;
        BasicEffect basicEffect;
        SphereDensityFunction sdf;
        SimplexDensityFunction simplexDensityFunction;

        FPSCounter _fpsCounter;

        IChunkManager _chunkManager;
        World _world;

        ContentLibrary contentLibrary;

        bool _paused = false;

        public Playground()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

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

            contentLibrary = new ContentLibrary();
            contentLibrary.Load(GraphicsDevice, Content);

            basicEffect = new BasicEffect(GraphicsDevice);

            font = Content.Load<SpriteFont>("Arial");

            _fpsCounter = new FPSCounter(this,  font);
            _fpsCounter.LoadContent();

            camManager = new CameraManager();
            camManager.AddCamera("player", cam);
            camManager.MainCameraName = "player";

            /*
            Console.WriteLine("Creating chunk...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            chunk = new Chunk(null, Vector3I.Zero, 16, 16, 16);
            sdf = new SphereDensityFunction(new Vector3(8, 8, 8), 6f);
            
            DensityChunkGenerator dcg = new DensityChunkGenerator(simplexDensityFunction);
            dcg.Generate(chunk);
            sw.Stop();
            Console.WriteLine("Done in {0}ms.", sw.ElapsedMilliseconds);
            Console.WriteLine("Density at (7, 7, 7): {0}", chunk.PointAt(7, 7, 7).Density);
            Console.WriteLine("Building chunk mesh...");
            DualContourChunkBuilder dccb = new DualContourChunkBuilder(GraphicsDevice, simplexDensityFunction, 0.6f);
            sw.Restart();
            dccb.Build(chunk);
            sw.Stop();
            Console.WriteLine("Done in {0}ms.", sw.Elapsed.TotalMilliseconds);*/

            simplexDensityFunction = new SimplexDensityFunction(0.01f, 0.1f);

            EngineConfiguration config = new EngineConfiguration();

            config.ChunkWidth = 16;
            config.ChunkHeight = 16;
            config.ChunkLength = 16;

            config.RenderDistance = 5;

            int worldChunkHeight = 8;

            // 113188818 - test seed
            IslandChunkGenerator icg = new IslandChunkGenerator(worldChunkHeight * config.ChunkHeight, new SimplexNoiseGenerator(Environment.TickCount));

            _world = new World(config);
            _chunkManager = new StaticThreadedChunkManager(_world, 5, worldChunkHeight, 5);
            _chunkManager.ChunkSystem = new DualContourColoredChunkSystem(GraphicsDevice, contentLibrary, _chunkManager, camManager, _world, icg, 0.0f);
            _chunkManager.ChunkGenerator = icg;

            _world.ChunkManager = _chunkManager;

            _chunkManager.GenerateChunks();
            _chunkManager.BuildAllChunks();
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
                moveVector *= 0.125f;
            }

            moveVector *= (float)gameTime.ElapsedGameTime.TotalSeconds / (1f / 60f);

            float mult = 1 / 2048f * (float)gameTime.ElapsedGameTime.TotalSeconds / (1f / 1000f);

            if (Input.Get().IsKeyDown(Keys.LeftShift))
                mult *= 2;

            if (Input.Get().IsKeyDown(Keys.Right))
                cam.Yaw -= MathHelper.Pi * mult;
            if (Input.Get().IsKeyDown(Keys.Left))
                cam.Yaw += MathHelper.Pi * mult;
            if (Input.Get().IsKeyDown(Keys.Up))
                cam.Pitch -= MathHelper.Pi * mult;
            if (Input.Get().IsKeyDown(Keys.Down))
                cam.Pitch += MathHelper.Pi * mult;

            cam.Position += moveVector;

            cam.Update();

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _chunkManager.ChunkSystem.Renderer.RenderAll();

            /*
            for (int x = 5; x < 11; x++)
            {
                for (int y = 5; y < 11; y++)
                {
                    for (int z = 5; z < 11; z++)
                    {
                            BoundingBoxRenderer.Render(GraphicsDevice, new BoundingBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1)), Color.Gold, cam);
                    }
                }
            }*/

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
