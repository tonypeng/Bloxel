/*
 * Bloxel - DualContourChunkBuilder.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Bloxel.Engine.DataStructures;
using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.Core
{
    public class DualContourChunkBuilder : IChunkBuilder
    {
        Random rand = new Random();

        // edge table
        private int[] _edgeTable = new int[] {
            0x0  , 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
            0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
            0x190, 0x99 , 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
            0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
            0x230, 0x339, 0x33 , 0x13a, 0x636, 0x73f, 0x435, 0x53c,
            0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
            0x3a0, 0x2a9, 0x1a3, 0xaa , 0x7a6, 0x6af, 0x5a5, 0x4ac,
            0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
            0x460, 0x569, 0x663, 0x76a, 0x66 , 0x16f, 0x265, 0x36c,
            0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
            0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff , 0x3f5, 0x2fc,
            0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
            0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55 , 0x15c,
            0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
            0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc ,
            0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
            0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
            0xcc , 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
            0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
            0x15c, 0x55 , 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
            0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
            0x2fc, 0x3f5, 0xff , 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
            0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
            0x36c, 0x265, 0x16f, 0x66 , 0x76a, 0x663, 0x569, 0x460,
            0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
            0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa , 0x1a3, 0x2a9, 0x3a0,
            0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
            0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33 , 0x339, 0x230,
            0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
            0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99 , 0x190,
            0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
            0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0 };

        // intersections
        private int[][][] _intersections = new int[][][]
            { new int[][] { 
                new int[] {0,0,0}, new int[] {1,0,0}
              },
              new int[][] {
                new int[] {1,0,0}, new int[] {1,1,0}
              },
              new int[][] {
                new int[] {1,1,0}, new int[] {0,1,0}
              },
              new int[][] {
                new int[] {0,1,0}, new int[] {0,0,0}
              },
              new int[][] {
                new int[] {0,0,1}, new int[] {1,0,1}
              },
              new int[][] {
                new int[] {1,0,1}, new int[] {1,1,1}
              },
              new int[][] {
                new int[] {1,1,1}, new int[] {0,1,1}
              },
              new int[][] {
                new int[] {0,1,1}, new int[] {0,0,1}
              },
              new int[][] {
                new int[] {0,0,0}, new int[] {0,0,1}
              },
              new int[][] {
                new int[] {1,0,0}, new int[] {1,0,1}
              },
              new int[][] {
                new int[] {1,1,0}, new int[] {1,1,1}
              },
              new int[][] {
                new int[] {0,1,0}, new int[] {0,1,1}
              }
            };

        //               2
        //     -------------------------
        //    |\                       |\
        //    | \                      | \
        //    |  \  11                 |  \ 10
        //    |   \                    |   \
        //    |    \             6     |    \
        //   3|     \========================\
        //    |     ||                 |     ||
        //    |     ||                 |     ||
        //    |     ||                1|     ||
        //    |     ||      0          |     ||
        //    \-----||------------------\    ||
        //     \    ||                   \   || 5
        //      \   || 7                9 \  ||
        //    8  \  ||                     \ ||
        //        \ ||                      \||
        //         \||========================
        //                      4

        private GraphicsDevice _device;

        private World _world;

        private Dictionary<Chunk, List<VertexPositionNormalColor>> _vertices;
        private Dictionary<Chunk, List<VertexPositionColor>> _normalVertices;
        private Dictionary<Chunk, Dictionary<Vector3I, short>> _positionToIndex;
        private Dictionary<Chunk, List<short>> _indices;
        private Dictionary<Chunk, HashSet<Edge>> _intersectingEdges;
        private Dictionary<Chunk, HashSet<Edge>> _borderEdges;
        private Dictionary<Chunk, HashSet<Vector3I>> _neededBorderPositions;

        private float _minimumSolidDensity;

        private ITerrainGradientFunction _densityGradientFunction;

        private int _triangles = 0;

        /// <summary>
        /// All chunks must generate QEF data so that neighbor chunks can use them.
        /// </summary>
        public bool RequiresPostProcess { get { return true; } }

        public DualContourChunkBuilder(GraphicsDevice device, World world, ITerrainGradientFunction densityGradientFunction, float minimumSolidDensity)
        {
            _device = device;

            _world = world;

            _vertices = new Dictionary<Chunk, List<VertexPositionNormalColor>>();
            _indices = new Dictionary<Chunk, List<short>>();
            _positionToIndex = new Dictionary<Chunk, Dictionary<Vector3I, short>>();
            _intersectingEdges = new Dictionary<Chunk, HashSet<Edge>>();
            _normalVertices = new Dictionary<Chunk, List<VertexPositionColor>>();
            _borderEdges = new Dictionary<Chunk, HashSet<Edge>>();
            _neededBorderPositions = new Dictionary<Chunk, HashSet<Vector3I>>();

            _densityGradientFunction = densityGradientFunction;

            _minimumSolidDensity = minimumSolidDensity;
        }

        public void Build(Chunk c)
        {
            if (!_vertices.ContainsKey(c))
            {
                _vertices.Add(c, new List<VertexPositionNormalColor>());
                _indices.Add(c, new List<short>());
                _positionToIndex.Add(c, new Dictionary<Vector3I, short>());
                _intersectingEdges.Add(c, new HashSet<Edge>());
                _normalVertices.Add(c, new List<VertexPositionColor>());
                _borderEdges.Add(c, new HashSet<Edge>());
                _neededBorderPositions.Add(c, new HashSet<Vector3I>());
            }

            _triangles = 0;

            BuildVertices(c);

            Console.WriteLine("{0} triangles.", _triangles);
        }

        public void PostProcess(Chunk c)
        {
            // after we build the chunk's vertices, we need to stitch the borders.

            // each chunk only stiches borders with its XPositive, YPositive, and ZPositive neighbor chunks. (if they aren't null)
            // this ensures that we don't create more quads than necessary.

            List<VertexPositionNormalColor> vertices = _vertices[c];
            Dictionary<Vector3I, short> positionToIndex = _positionToIndex[c];
            HashSet<Vector3I> neededBorderPositions = _neededBorderPositions[c];

            foreach (Vector3I pos in neededBorderPositions)
            {
                Chunk otherChunk = _world.ChunkAt(c.Position.X + pos.X, c.Position.Y + pos.Y, c.Position.Z + pos.Z);

                Vector3I otherChunkLocalPos = new Vector3I(pos.X % otherChunk.Width, pos.Y % otherChunk.Height, pos.Z % otherChunk.Length);

                vertices.Add(_vertices[otherChunk][_positionToIndex[otherChunk][otherChunkLocalPos]]);
                positionToIndex.Add(pos, (short)(vertices.Count - 1));
            }

            foreach (Edge e in _borderEdges[c])
            {
                Direction d = e.FaceDirection;

                short[] indices = new short[6];

                Vector3I[] cubes = e.GetCubePositions();

                bool invalid = false;

                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].X < 0 || cubes[i].Y < 0 || cubes[i].Z < 0)
                    {
                        invalid = true;
                        break;
                    }
                }

                if (invalid) continue;

                // TODO: possible degenerate triangles (0 area)
                switch (d)
                {
                    case Direction.XDecreasing:
                    case Direction.ZDecreasing:
                    case Direction.YIncreasing:
                        indices[0] = positionToIndex[cubes[0]];
                        indices[1] = positionToIndex[cubes[1]];
                        indices[2] = positionToIndex[cubes[3]];
                        indices[3] = positionToIndex[cubes[0]];
                        indices[4] = positionToIndex[cubes[3]];
                        indices[5] = positionToIndex[cubes[2]];
                        break;
                    case Direction.XIncreasing:
                    case Direction.ZIncreasing:
                    case Direction.YDecreasing:
                        indices[0] = positionToIndex[cubes[0]];
                        indices[1] = positionToIndex[cubes[3]];
                        indices[2] = positionToIndex[cubes[1]];
                        indices[3] = positionToIndex[cubes[0]];
                        indices[4] = positionToIndex[cubes[2]];
                        indices[5] = positionToIndex[cubes[3]];
                        break;
                }

                short index0 = positionToIndex[cubes[0]];
                short index1 = positionToIndex[cubes[1]];
                short index2 = positionToIndex[cubes[2]];
                short index3 = positionToIndex[cubes[3]];

                // triangle 1
                Vector3 point0 = vertices[indices[0]].Position;
                Vector3 point1 = vertices[indices[1]].Position;
                Vector3 point2 = vertices[indices[2]].Position;

                Vector3 v01 = point1 - point0;
                Vector3 v12 = point2 - point1;

                Vector3 normal1 = Vector3.Cross(v12, v01);
                normal1.Normalize();

                if (Single.IsNaN(normal1.X) || Single.IsNaN(normal1.Y) || Single.IsNaN(normal1.Z))
                    normal1 = Vector3.Zero; // TODO: this is a hack; think it through later

                // triangle 2
                Vector3 point3 = vertices[indices[3]].Position;
                Vector3 point4 = vertices[indices[4]].Position;
                Vector3 point5 = vertices[indices[5]].Position;

                Vector3 v34 = point4 - point3;
                Vector3 v45 = point5 - point4;

                Vector3 normal2 = Vector3.Cross(v45, v34);
                normal2.Normalize();

                if (Single.IsNaN(normal2.X) || Single.IsNaN(normal2.Y) || Single.IsNaN(normal2.Z))
                    normal2 = Vector3.Zero; // TODO: this is a hack; think it through later

                _triangles += 2;

                VertexPositionNormalColor vertex0 = vertices[indices[0]];
                VertexPositionNormalColor vertex1 = vertices[indices[1]];
                VertexPositionNormalColor vertex2 = vertices[indices[2]];

                vertex0.Normal += normal1;
                vertex1.Normal += normal1;
                vertex2.Normal += normal1;

                vertices[indices[0]] = vertex0;
                vertices[indices[1]] = vertex1;
                vertices[indices[2]] = vertex2;

                VertexPositionNormalColor vertex3 = vertices[indices[3]];
                VertexPositionNormalColor vertex4 = vertices[indices[4]];
                VertexPositionNormalColor vertex5 = vertices[indices[5]];

                vertex3.Normal += normal2;
                vertex4.Normal += normal2;
                vertex5.Normal += normal2;

                vertices[indices[3]] = vertex3;
                vertices[indices[4]] = vertex4;
                vertices[indices[5]] = vertex5;

                AddIndices(c, indices);
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            BuildBuffers(c);
            sw.Stop();
            Console.WriteLine("BuildBuffers(): {0}ms", sw.ElapsedMilliseconds);
        }

        private void BuildVertices(Chunk c)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            CreateMinimizingVertices(c);
            sw.Stop();
            Console.WriteLine("CreateMinimizingVertices(): {0}ms", sw.ElapsedMilliseconds);

            sw.Restart();
            ConnectMinimizingVertices(c);
            sw.Stop();
            Console.WriteLine("ConnectMinimizingVertices(): {0}ms", sw.ElapsedMilliseconds);
        }

        private void CreateMinimizingVertices(Chunk c)
        {
            HashSet<Edge> intersectingEdges = _intersectingEdges[c];
            List<VertexPositionNormalColor> vertices = _vertices[c];
            Dictionary<Vector3I, short> positionToIndex = _positionToIndex[c];

            for (int x = 0; x < c.Width; x++)
            {
                for (int z = 0; z < c.Length; z++)
                {
                    for (int y = 0; y < c.Height; y++)
                    {
                        if (x == 1 && y == 0 && z == 15)
                            Console.WriteLine();

                        Vector3I chunkPos = new Vector3I(x, y, z);
                        ProcessBlock(intersectingEdges, vertices, positionToIndex, c, c.PointAt(x, y, z), chunkPos + c.Position, chunkPos);
                    }
                }
            }
        }

        private void ConnectMinimizingVertices(Chunk c)
        {
            Dictionary<Vector3I, short> positionToIndex = _positionToIndex[c];
            List<VertexPositionNormalColor> vertices = _vertices[c];

            foreach (Edge e in _intersectingEdges[c])
            {
                Direction d = e.FaceDirection;

                short[] indices = new short[6];

                Vector3I[] cubes = e.GetCubePositions();

                bool invalid = false;

                HashSet<Edge> borderEdges = _borderEdges[c];
                HashSet<Vector3I> neededBorderPositions = _neededBorderPositions[c];

                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].X < 0 || cubes[i].Y < 0 || cubes[i].Z < 0)
                    {
                        invalid = true;
                        break;
                    }

                    if (cubes[i].X > c.Width - 1 || cubes[i].Y > c.Height - 1 || cubes[i].Z > c.Length - 1)
                    {
                        borderEdges.Add(e);

                        neededBorderPositions.Add(cubes[i]);

                        invalid = true;
                    }
                }

                if (invalid) continue;

                // TODO: possible degenerate triangles (0 area)
                switch (d)
                {
                    case Direction.XDecreasing:
                    case Direction.ZDecreasing:
                    case Direction.YIncreasing:
                        indices[0] = positionToIndex[cubes[0]];
                        indices[1] = positionToIndex[cubes[1]];
                        indices[2] = positionToIndex[cubes[3]];
                        indices[3] = positionToIndex[cubes[0]];
                        indices[4] = positionToIndex[cubes[3]];
                        indices[5] = positionToIndex[cubes[2]];
                        break;
                    case Direction.XIncreasing:
                    case Direction.ZIncreasing:
                    case Direction.YDecreasing:
                        indices[0] = positionToIndex[cubes[0]];
                        indices[1] = positionToIndex[cubes[3]];
                        indices[2] = positionToIndex[cubes[1]];
                        indices[3] = positionToIndex[cubes[0]];
                        indices[4] = positionToIndex[cubes[2]];
                        indices[5] = positionToIndex[cubes[3]];
                        break;
                }

                short index0 = positionToIndex[cubes[0]];
                short index1 = positionToIndex[cubes[1]];
                short index2 = positionToIndex[cubes[2]];
                short index3 = positionToIndex[cubes[3]];

                // triangle 1
                Vector3 point0 = vertices[indices[0]].Position;
                Vector3 point1 = vertices[indices[1]].Position;
                Vector3 point2 = vertices[indices[2]].Position;

                Vector3 v01 = point1 - point0;
                Vector3 v12 = point2 - point1;

                Vector3 normal1 = Vector3.Cross(v12, v01);
                normal1.Normalize();

                if (Single.IsNaN(normal1.X) || Single.IsNaN(normal1.Y) || Single.IsNaN(normal1.Z))
                    normal1 = Vector3.Zero; // TODO: this is a hack; think it through later

                // triangle 2
                Vector3 point3 = vertices[indices[3]].Position;
                Vector3 point4 = vertices[indices[4]].Position;
                Vector3 point5 = vertices[indices[5]].Position;

                Vector3 v34 = point4 - point3;
                Vector3 v45 = point5 - point4;

                Vector3 normal2 = Vector3.Cross(v45, v34);
                normal2.Normalize();

                if (Single.IsNaN(normal2.X) || Single.IsNaN(normal2.Y) || Single.IsNaN(normal2.Z))
                    normal2 = Vector3.Zero; // TODO: this is a hack; think it through later

                //_normalVertices.Add(new VertexPositionColor(point1, Color.White));
                //_normalVertices.Add(new VertexPositionColor(point1 + normal1, Color.White));
                //_normalVertices.Add(new VertexPositionColor(point2, Color.Black));
                //_normalVertices.Add(new VertexPositionColor(point2 + normal2, Color.Black));

                _triangles += 2;

                VertexPositionNormalColor vertex0 = vertices[indices[0]];
                VertexPositionNormalColor vertex1 = vertices[indices[1]];
                VertexPositionNormalColor vertex2 = vertices[indices[2]];

                vertex0.Normal += normal1;
                vertex1.Normal += normal1;
                vertex2.Normal += normal1;

                vertices[indices[0]] = vertex0;
                vertices[indices[1]] = vertex1;
                vertices[indices[2]] = vertex2;

                VertexPositionNormalColor vertex3 = vertices[indices[3]];
                VertexPositionNormalColor vertex4 = vertices[indices[4]];
                VertexPositionNormalColor vertex5 = vertices[indices[5]];

                vertex3.Normal += normal2;
                vertex4.Normal += normal2;
                vertex5.Normal += normal2;

                vertices[indices[3]] = vertex3;
                vertices[indices[4]] = vertex4;
                vertices[indices[5]] = vertex5;

                AddIndices(c, indices);
            }
        }

        private void ProcessBlock(HashSet<Edge> intersectingEdges, List<VertexPositionNormalColor> vertices, Dictionary<Vector3I, short> positionToIndex, Chunk c, GridPoint min, Vector3I worldPosition, Vector3I localPosition)
        {
            int lX = localPosition.X;
            int lY = localPosition.Y;
            int lZ = localPosition.Z;

            GridPoint XYZ = min;
            GridPoint XMaxYZ = c.PointAt(lX + 1, lY, lZ);
            GridPoint XYMaxZ = c.PointAt(lX, lY + 1, lZ);
            GridPoint XYZMax = c.PointAt(lX, lY, lZ + 1);
            GridPoint XMaxYMaxZ = c.PointAt(lX + 1, lY + 1, lZ);
            GridPoint XMaxYZMax = c.PointAt(lX + 1, lY, lZ + 1);
            GridPoint XYMaxZMax = c.PointAt(lX, lY + 1, lZ + 1);
            GridPoint XMaxYMaxZMax = c.PointAt(lX + 1, lY + 1, lZ + 1);

            Tuple<HermiteData, Edge[]> data = CubeInfo(localPosition, XYZ, XMaxYZ, XYMaxZ, XYZMax, XMaxYMaxZ, XMaxYZMax, XYMaxZMax, XMaxYMaxZMax);

            HermiteData hermite = data.Item1;

            // If there are no intersections, this is an interior point.  Therefore, don't build any vertices.
            if (hermite.IntersectionPoints.Count == 0)
                return;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // investigate possible optimization by using AddRange, since it re-allocates the list only once.
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < data.Item2.Length; i++)
            {
                if (!intersectingEdges.Contains(data.Item2[i]))
                {
                    edges.Add(data.Item2[i]);
                }
            }

            intersectingEdges.UnionWith(edges);

            Vector3 minimizingVertex = DualContouring.SchmitzVertexFromHermiteData(hermite, 0.01f, 25);

            vertices.Add(new VertexPositionNormalColor(c.Position.ToVector3() + minimizingVertex, Vector3.Zero, Color.Green));
            positionToIndex.Add(localPosition, (short)(_vertices[c].Count - 1));

            //_positionToQEF.Add(localPosition, new CubeInfo(minimizingVertex, data.Item2));
        }

        private Tuple<HermiteData, Edge[]> CubeInfo(Vector3I min, GridPoint XYZ, GridPoint XMaxYZ, GridPoint XYMaxZ, GridPoint XYZMax, GridPoint XMaxYMaxZ, GridPoint XMaxYZMax, GridPoint XYMaxZMax, GridPoint XMaxYMaxZMax)
        {
            HermiteData hermiteData = new HermiteData(new List<Vector3>(), new List<Vector3>());

            // get edge table index
            int cubeIndex = 0;

            if (XYZ.Density < _minimumSolidDensity)
                cubeIndex += 1;
            if (XMaxYZ.Density < _minimumSolidDensity)
                cubeIndex += 2;
            if (XMaxYMaxZ.Density < _minimumSolidDensity)
                cubeIndex += 4;
            if (XYMaxZ.Density < _minimumSolidDensity)
                cubeIndex += 8;
            if (XYZMax.Density < _minimumSolidDensity)
                cubeIndex += 16;
            if (XMaxYZMax.Density < _minimumSolidDensity)
                cubeIndex += 32;
            if (XMaxYMaxZMax.Density < _minimumSolidDensity)
                cubeIndex += 64;
            if (XYMaxZMax.Density < _minimumSolidDensity)
                cubeIndex += 128;

            GridPoint[][][] lookupTable = new GridPoint[][][]
            {
                new GridPoint[][]
                {
                    new GridPoint[]
                    {
                        XYZ,
                        XYZMax,
                    },
                    new GridPoint[]
                    {
                        XYMaxZ,
                        XYMaxZMax,
                    }
                },
                new GridPoint[][]
                {
                    new GridPoint[]
                    {
                        XMaxYZ,
                        XMaxYZMax,
                    },
                    new GridPoint[]
                    {
                        XMaxYMaxZ,
                        XMaxYMaxZMax,
                    }
                },
            };

            int edge = _edgeTable[cubeIndex];

            List<Edge> xEdges = new List<Edge>();

            // loop through all the edges
            for (int i = 0; i < 12; i++)
            {
                // if this edge contains no intersection, skip it
                if ((edge & (1 << i)) == 0)
                    continue;

                int[] corner1Offset = _intersections[i][0];
                int[] corner2Offset = _intersections[i][1];

                Vector3I corner1 = min;
                Vector3I corner2 = min;

                corner1.X += corner1Offset[0];
                corner1.Y += corner1Offset[1];
                corner1.Z += corner1Offset[2];

                corner2.X += corner2Offset[0];
                corner2.Y += corner2Offset[1];
                corner2.Z += corner2Offset[2];

                GridPoint corner1Point = lookupTable[corner1Offset[0]][corner1Offset[1]][corner1Offset[2]];
                GridPoint corner2Point = lookupTable[corner2Offset[0]][corner2Offset[1]][corner2Offset[2]];

                Vector3I delta = (corner2 - corner1) * (corner1Point.Density < _minimumSolidDensity ? -1 : 1); // negate if corner1 is less than the isovalue because we assume corner2 at first.
                Direction dir = GetDirectionFromDelta(delta);

                xEdges.Add(new Edge(corner1, corner2, dir));

                Vector3 intersectionPoint = DualContouring.InterpolateIntersectionPoint(_minimumSolidDensity, corner1.ToVector3(), corner2.ToVector3(),
                    corner1Point.Density,
                    corner2Point.Density);

                Vector3 normal = _densityGradientFunction.df(intersectionPoint.X, intersectionPoint.Y, intersectionPoint.Z);

                hermiteData.Add(intersectionPoint, normal);
            }

            Tuple<HermiteData, Edge[]> ret = new Tuple<HermiteData, Edge[]>(hermiteData, xEdges.ToArray());

            return ret;
        }

        private Direction GetDirectionFromDelta(Vector3I delta)
        {
            int dx = delta.X;
            int dy = delta.Y;
            int dz = delta.Z;

            Contract.Assert(Math.Abs(dx + dy + dz) == 1);

            if (dx == -1)
            {
                return Direction.XDecreasing;
            }
            else if (dx == 1)
            {
                return Direction.XIncreasing;
            }
            else if (dy == -1)
            {
                return Direction.YDecreasing;
            }
            else if (dy == 1)
            {
                return Direction.YIncreasing;
            }
            else if (dz == -1)
            {
                return Direction.ZDecreasing;
            }
            else if (dz == 1)
            {
                return Direction.ZIncreasing;
            }

            throw new Exception("Impossible!");
        }

        private void BuildBuffers(Chunk c)
        {
            if (_vertices[c].Count <= 0 || _indices[c].Count <= 0)
                return;

            VertexPositionNormalColor[] vertices = new VertexPositionNormalColor[_vertices[c].Count];
            short[] indices = new short[_indices[c].Count];

            _vertices[c].CopyTo(vertices);
            _indices[c].CopyTo(indices);
            
            NormalizeNormals(c, vertices);

            VertexPositionColor[] normal = new VertexPositionColor[_normalVertices[c].Count];
            _normalVertices[c].CopyTo(normal);

            lock (c.GraphicsSync)
            {
                if(c.VertexBuffer != null)
                    c.VertexBuffer.Dispose();
                if(c.IndexBuffer != null)
                    c.IndexBuffer.Dispose();
                if (c.NormalsVertexBuffer != null)
                    c.NormalsVertexBuffer.Dispose();

                c.VertexBuffer = new DynamicVertexBuffer(_device, typeof(VertexPositionNormalColor), vertices.Length, BufferUsage.WriteOnly);
                c.IndexBuffer = new DynamicIndexBuffer(_device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
                c.NormalsVertexBuffer = new DynamicVertexBuffer(_device, typeof(VertexPositionColor), normal.Length, BufferUsage.WriteOnly);

                c.VertexBuffer.SetData<VertexPositionNormalColor>(vertices);
                c.IndexBuffer.SetData<short>(indices);
                c.NormalsVertexBuffer.SetData<VertexPositionColor>(normal);
            }

            //_vertices[c].Clear();
            _normalVertices[c].Clear();
            _indices[c].Clear();
            _intersectingEdges[c].Clear();
            //_positionToIndex[c].Clear();
        }

        private void NormalizeNormals(Chunk c, VertexPositionNormalColor[] normals)
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i].Normal.Normalize();

                if (Single.IsNaN(normals[i].Normal.X) || Single.IsNaN(normals[i].Normal.Y) || Single.IsNaN(normals[i].Normal.Z))
                    normals[i].Normal = Vector3.Zero;

                _normalVertices[c].Add(new VertexPositionColor(normals[i].Position, Color.Gray));
                _normalVertices[c].Add(new VertexPositionColor(normals[i].Position + normals[i].Normal, Color.Gray));
            }
        }

        private void AddIndices(Chunk c, params short[] indices)
        {
            _indices[c].AddRange(indices);
        }
    }
}