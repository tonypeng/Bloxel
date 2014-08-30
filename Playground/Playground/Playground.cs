/*
 * Bloxel - Playground.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
 */

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

using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysicsDrawer;
using BEPUphysicsDrawer.Models;
using ConversionHelper;

using Bloxel.Engine.Cameras;
using Bloxel.Engine.Core;
using Bloxel.Engine.DataStructures;
using Bloxel.Engine.Input;
using Bloxel.Engine.Utilities;

using Bloxel.Engine.DeferredRendering;

using Playground.Generator;
using BEPUphysics.Entities.Prefabs;

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

        Space _space;
        object _physicsMutex = new object();

        ModelDrawer modelDrawer;

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

            modelDrawer = new InstancedModelDrawer(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            contentLibrary = new ContentLibrary();
            contentLibrary.Load(GraphicsDevice, Content);

            basicEffect = new BasicEffect(GraphicsDevice);

            _space = new Space();
            _space.ForceUpdater.Gravity = MathConverter.Convert(new Vector3(0, -9.81f, 0));

            font = Content.Load<SpriteFont>("Arial");

            _fpsCounter = new FPSCounter(this,  font);
            _fpsCounter.LoadContent();

            camManager = new CameraManager();
            camManager.AddCamera("player", cam);
            camManager.MainCameraName = "player";

            simplexDensityFunction = new SimplexDensityFunction(0.01f, 0.1f);

            EngineConfiguration config = new EngineConfiguration();

            config.ChunkWidth = 16;
            config.ChunkHeight = 16;
            config.ChunkLength = 16;

            config.CPULightingEnabled = true;

            config.RenderDistance = 5;

            int worldChunkHeight = 8;

            SphereDensityFunction sdf = new SphereDensityFunction(new Vector3(8, 8, 8), 7.5f);
            SineWaveDensityFunction swdf = new SineWaveDensityFunction(5, 10, 0.25f);

            DualContourIslandChunkGenerator icg = new DualContourIslandChunkGenerator(worldChunkHeight * config.ChunkHeight, new SimplexNoiseGenerator(Environment.TickCount));
            DualContourFlatLandGenerator flg = new DualContourFlatLandGenerator();

            DensityChunkGenerator dcg = new DensityChunkGenerator(swdf);
            DebuggerGenerator dg = new DebuggerGenerator();

            IChunkGenerator generator;
            ITerrainGradientFunction grad;

            generator = icg;
            grad = icg;

            _world = new World(config, camManager);
            _chunkManager = new StaticThreadedChunkManager(config, _world, 5, worldChunkHeight, 5);
            _chunkManager.ChunkSystem = new DualContourColoredChunkSystem(GraphicsDevice, contentLibrary, _chunkManager, camManager, _world, grad, 0.0f);
            _chunkManager.ChunkSystem.Builder.MeshBuilt += new MeshBuildEventHandler(BuildPhysicsMesh);
            _chunkManager.ChunkGenerator = generator;
            _chunkManager.LightManager = new FloodfillLightManager(_chunkManager, config, worldChunkHeight);

            _world.ChunkManager = _chunkManager;

            _chunkManager.GenerateChunks();
            _chunkManager.BuildAllChunks();
        }

        protected override void UnloadContent()
        {
        }

        private void BuildPhysicsMesh(object o, MeshBuildEventArgs e)
        {
            Chunk chunk = (Chunk)o;

            if (e.Vertices.Length <= 0) return;

            lock (chunk.TagSync)
            {
                if (chunk.Tag != null && chunk.Tag is BEPUChunkTag)
                {
                    BEPUChunkTag tag = (BEPUChunkTag)chunk.Tag;

                    lock (_physicsMutex)
                    {
                        _space.Remove(tag.Mesh);
                        //modelDrawer.Remove(tag.Mesh.Mesh);
                    }
                }
                else
                {
                    chunk.Tag = new BEPUChunkTag();
                }

                StaticMesh mesh = new StaticMesh(MathConverter.Convert(e.Vertices), e.Indices);

                lock (_physicsMutex)
                {
                    _space.Add(mesh);
                    //modelDrawer.Add(mesh.Mesh);
                    ((BEPUChunkTag)chunk.Tag).Mesh = mesh;
                }
            }
        }

        float timeout = 0;
        Vector3I pickedPos = Vector3I.One * -1;
        Vector3I sidePickedPos = Vector3I.One * -1;

        bool drawPhysics = false;
        bool wireFrame = false;

        protected override void Update(GameTime gameTime)
        {
            Input.Get().Update();

            if (Input.Get().IsKeyDown(Keys.Escape, true))
                _paused = !_paused;

            if (Input.Get().IsKeyDown(Keys.F2, true))
            {
                _chunkManager.ChunkSystem.Renderer.ToggleDebugMode(ChunkRendererDebugOptions.DEBUG_DRAW_WIREFRAME);
                wireFrame = !wireFrame;
            }

            if (Input.Get().IsKeyDown(Keys.F3, true)) _chunkManager.ChunkSystem.Renderer.ToggleDebugMode(ChunkRendererDebugOptions.DEBUG_DRAW_NORMALS);
            if (Input.Get().IsKeyDown(Keys.F4, true)) _chunkManager.ChunkSystem.Renderer.ToggleDebugMode(ChunkRendererDebugOptions.DEBUG_DRAW_RENDERTARGETS);
            if (Input.Get().IsKeyDown(Keys.F5, true)) drawPhysics = !drawPhysics;

            if (Input.Get().IsKeyDown(Keys.F5, true))
            {
                if (pickedPos != Vector3I.One * -1)
                {
                    Chunk c = _world.ChunkAt(pickedPos.X, pickedPos.Y, pickedPos.Z);

                    _chunkManager.EnqueueChunkForBuild(c);
                }
            }
            
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
            if (!Input.Get().IsKeyDown(Keys.Tab))
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

            pickedPos = Vector3I.One * -1;
            sidePickedPos = Vector3I.One * -1;

            Vector3I lastEmptyBlock = new Vector3I((int)(cam.Position.X + 0.5f), (int)(cam.Position.Y + 0.5f), (int)(cam.Position.Z + 0.5f));

            // block picking
            for (float f = 0; f < 32f; f += 0.1f)
            {
                Vector3 pos = cam.Position + cam.Forward * f;

                int rx = (int)(pos.X + 0.5f);
                int ry = (int)(pos.Y + 0.5f);
                int rz = (int)(pos.Z + 0.5f);

                GridPoint gridPoint = _world.PointAt(rx, ry, rz);

                if (gridPoint.Density >= 0.0f)
                {
                    pickedPos = new Vector3I(rx, ry, rz);
                    sidePickedPos = lastEmptyBlock;
                    break;
                }
                else
                {
                    lastEmptyBlock = new Vector3I(rx, ry, rz);
                }
            }

            if (pickedPos == Vector3I.One * -1)
                sidePickedPos = pickedPos;

            if ((timeout -= (float)gameTime.ElapsedGameTime.TotalSeconds) <= 0 && Input.Get().IsLeftMouseButtonDown() && pickedPos != Vector3I.One * -1)
            {
                int rx = pickedPos.X;
                int ry = pickedPos.Y;
                int rz = pickedPos.Z;

                GridPoint gridPoint = _world.PointAt(rx, ry, rz);

                if (gridPoint.Density >= 0.0f)
                {
                    pickedPos = new Vector3I(rx, ry, rz);

                    GridPoint selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                    // modify the normals and density
                    selected.Density = -1.0f;
                    // make the vectors point towards the point
                    selected.XPositiveNormal = new Vector3(-1, 0, 0);
                    selected.YPositiveNormal = new Vector3(0, -1, 0);
                    selected.ZPositiveNormal = new Vector3(0, 0, -1);

                    _world.SetPoint(rx, ry, rz, selected);

                    gridPoint = _world.PointAt(rx, ry - 1, rz);

                    if (gridPoint.Density >= 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = 1.0f;
                        selected.YPositiveNormal = new Vector3(0, 1, 0);

                        _world.SetPoint(rx, ry - 1, rz, selected);
                    }

                    gridPoint = _world.PointAt(rx - 1, ry, rz);

                    if (gridPoint.Density >= 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = 1.0f;
                        selected.XPositiveNormal = new Vector3(1, 0, 0);

                        _world.SetPoint(rx - 1, ry, rz, selected);
                    }

                    gridPoint = _world.PointAt(rx, ry, rz - 1);

                    if (gridPoint.Density >= 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = 1.0f;
                        selected.ZPositiveNormal = new Vector3(0, 0, 1);

                        _world.SetPoint(rx, ry, rz - 1, selected);
                    }

                    gridPoint = _world.PointAt(rx, ry, rz + 1);

                    if (gridPoint.Density >= 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = 1.0f;

                        _world.SetPoint(rx, ry, rz + 1, selected);
                    }

                    gridPoint = _world.PointAt(rx, ry + 1, rz);

                    if (gridPoint.Density >= 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = 1.0f;

                        _world.SetPoint(rx, ry + 1, rz, selected);
                    }

                    gridPoint = _world.PointAt(rx + 1, ry, rz);

                    if (gridPoint.Density >= 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = 1.0f;

                        _world.SetPoint(rx + 1, ry, rz, selected);
                    }

                    timeout = 0.2f;

                    if (Input.Get().IsKeyDown(Keys.Space))
                        timeout = 0.01f;
                }
            }

            if (timeout <= 0 && Input.Get().IsRightMouseButtonDown() && sidePickedPos != Vector3I.One * -1)
            {
                int rx = sidePickedPos.X;
                int ry = sidePickedPos.Y;
                int rz = sidePickedPos.Z;

                GridPoint gridPoint = _world.PointAt(rx, ry, rz);

                if (gridPoint.Density < 0.0f)
                {
                    pickedPos = new Vector3I(rx, ry, rz);

                    GridPoint selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                    // modify the normals and density
                    selected.Density = 1.0f;
                    // make the vectors point away from the point
                    selected.XPositiveNormal = new Vector3(1, 0, 0);
                    selected.YPositiveNormal = new Vector3(0, 1, 0);
                    selected.ZPositiveNormal = new Vector3(0, 0, 1);

                    _world.SetPoint(rx, ry, rz, selected);

                    gridPoint = _world.PointAt(rx, ry - 1, rz);

                    if (gridPoint.Density < 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = -1.0f;
                        selected.YPositiveNormal = new Vector3(0, -1, 0);

                        _world.SetPoint(rx, ry - 1, rz, selected);
                    }

                    gridPoint = _world.PointAt(rx - 1, ry, rz);

                    if (gridPoint.Density < 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = -1.0f;
                        selected.XPositiveNormal = new Vector3(-1, 0, 0);

                        _world.SetPoint(rx - 1, ry, rz, selected);
                    }

                    gridPoint = _world.PointAt(rx, ry, rz - 1);

                    if (gridPoint.Density < 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = -1.0f;
                        selected.ZPositiveNormal = new Vector3(0, 0, -1);

                        _world.SetPoint(rx, ry, rz - 1, selected);
                    }

                    gridPoint = _world.PointAt(rx, ry, rz + 1);

                    if (gridPoint.Density < 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = -1.0f;

                        _world.SetPoint(rx, ry, rz + 1, selected);
                    }

                    gridPoint = _world.PointAt(rx, ry + 1, rz);

                    if (gridPoint.Density < 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = -1.0f;

                        _world.SetPoint(rx, ry + 1, rz, selected);
                    }

                    gridPoint = _world.PointAt(rx + 1, ry, rz);

                    if (gridPoint.Density < 0)
                    {
                        selected = new GridPoint(gridPoint, (int)DualContouringMetadataIndex.Length);
                        selected.Density = -1.0f;

                        _world.SetPoint(rx + 1, ry, rz, selected);
                    }

                    timeout = 0.2f;

                    if (Input.Get().IsKeyDown(Keys.Space))
                        timeout = 0.01f;
                }
            }

            if (Input.Get().IsKeyDown(Keys.B, true))
            {
                Sphere sphere = new Sphere(MathConverter.Convert(cam.Position + cam.Forward * 2), 1.0f, 1);
                sphere.LinearVelocity = MathConverter.Convert(cam.Forward * 5);

                lock (_physicsMutex)
                {
                    _space.Add(sphere);
                    modelDrawer.Add(sphere);
                }
            }

            _world.Update(gameTime);

            lock (_physicsMutex)
            {
                _space.Update();

                if (drawPhysics)
                {
                    modelDrawer.IsWireframe = wireFrame;
                    modelDrawer.Update();
                }
            }

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _chunkManager.ChunkSystem.Renderer.RenderAll();

            lock (_physicsMutex)
            {
                if (drawPhysics)
                    modelDrawer.Draw(MathConverter.Convert(cam.View), MathConverter.Convert(cam.Projection));
            }

            if (pickedPos != Vector3I.One * -1)
            {
                BoundingBox picked = new BoundingBox(pickedPos.ToVector3() - 0.55f * Vector3.One, pickedPos.ToVector3() + 0.55f * Vector3.One);
                BoundingBoxRenderer.Render(GraphicsDevice, picked, camManager.MainCamera);

                BoundingBox sidePicked = new BoundingBox(sidePickedPos.ToVector3() - 0.55f * Vector3.One, sidePickedPos.ToVector3() + 0.55f * Vector3.One);
                BoundingBoxRenderer.Render(GraphicsDevice, sidePicked, Color.Blue, camManager.MainCamera);

                BoundingBox b1 = new BoundingBox(pickedPos.ToVector3(), pickedPos.ToVector3() + Vector3.One);
                BoundingBox b2 = new BoundingBox(pickedPos.ToVector3() + Vector3.Left, pickedPos.ToVector3() + Vector3.Up + Vector3.Backward);
                BoundingBox b3 = new BoundingBox(pickedPos.ToVector3() + Vector3.Forward, pickedPos.ToVector3() + Vector3.Up + Vector3.Right);
                BoundingBox b4 = new BoundingBox(pickedPos.ToVector3() + Vector3.Left + Vector3.Forward, pickedPos.ToVector3() + Vector3.Up);

                BoundingBox b5 = new BoundingBox(pickedPos.ToVector3() + Vector3.Down, pickedPos.ToVector3() + Vector3.Right + Vector3.Backward);
                BoundingBox b6 = new BoundingBox(pickedPos.ToVector3() + Vector3.Left + Vector3.Down, pickedPos.ToVector3() + Vector3.Backward);
                BoundingBox b7 = new BoundingBox(pickedPos.ToVector3() + Vector3.Forward + Vector3.Down, pickedPos.ToVector3() + Vector3.Right);
                BoundingBox b8 = new BoundingBox(pickedPos.ToVector3() + Vector3.Left + Vector3.Forward + Vector3.Down, pickedPos.ToVector3());
            }

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Target: " + cam.Target, Vector2.One * 2f, Color.White);
            spriteBatch.DrawString(font, "Position: " + cam.Position, new Vector2(2, 2 + font.LineSpacing + 2), Color.White);
            spriteBatch.DrawString(font, "Selected: " + ((pickedPos == Vector3I.One * -1) ? "None" : pickedPos.ToString()), new Vector2(2, 2 + 2 * font.LineSpacing + 2), Color.White);

            if (_paused)
            {
                Vector2 dim = font.MeasureString("Paused");

                spriteBatch.DrawString(font, "Paused", new Vector2(GraphicsDevice.Viewport.Width - dim.X - 2, 2), Color.White);
            }

            spriteBatch.Draw(contentLibrary.DummyPixel, new Rectangle(GraphicsDevice.Viewport.Width / 2 - 1, GraphicsDevice.Viewport.Height / 2 - 1, 2, 2), Color.White);

            spriteBatch.End();

            _fpsCounter.Draw();

            base.Draw(gameTime);
        }
    }
}
