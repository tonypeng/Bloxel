/*
 * Bloxel - UnitTests.cs
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

#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Bloxel.Engine.Async;
using Bloxel.Engine.DataStructures;
using Bloxel.Engine.Utilities;

namespace Bloxel.Engine.UnitTests
{
    public class UnitTests
    {
        static void TestBitfieldSet()
        {
            Console.WriteLine("sizeof(ByteBitfield)={0}", System.Runtime.InteropServices.Marshal.SizeOf(typeof(ByteBitfield)));

            ByteBitfield field = new ByteBitfield(0);

            while (true)
            {
                for (int i = 7; i >= 0; i--)
                {
                    Console.Write(field.Get(i, 1));
                }
                Console.WriteLine();

                Console.WriteLine("Enter a number:");
                byte num = Byte.Parse(Console.ReadLine());
                Console.WriteLine("Enter an index:");
                int index = Int32.Parse(Console.ReadLine());

                int size = (int)Math.Log(num, 2) + 1;

                Console.WriteLine("Size: {0}", size);

                field.Set(index, num, size);
            }
        }

        static void TestBitfieldGet()
        {
            ByteBitfield field = new ByteBitfield(0);
            field.Set(4, 15, 4);
            field.Set(0, 2, 2);

            while (true)
            {
                for (int i = 7; i >= 0; i--)
                {
                    Console.Write(field.Get(i, 1));
                }
                Console.WriteLine();

                Console.WriteLine("Enter an index:");
                int index = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Enter a length:");
                int length = Int32.Parse(Console.ReadLine());

                Console.WriteLine("Value: {0}", field.Get(index, length));
            }
        }

        static void TestPriorityScheduler()
        {
            IScheduler scheduler = new PriorityScheduler(2);
            scheduler.Start();

            Console.WriteLine("Adding task 1");
            scheduler.Schedule(() => System.Threading.Thread.Sleep(1000), 2.0f);
            Console.WriteLine("Adding task 2");
            scheduler.Schedule(() => System.Threading.Thread.Sleep(1000), 2.0f);
            Console.WriteLine("Adding task 3");
            scheduler.Schedule(() => Console.WriteLine("Hello, world! (Lower priority)"), 2.0f);
            Console.WriteLine("Adding task 4");
            scheduler.Schedule(() => Console.WriteLine("Hello, world! (Higher priority)"), 1.0f);

            Console.ReadKey();
        }

        static void TestSimplexNoise()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 128; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        SimplexNoise.GetNoise(x, y, z);
                    }
                }
            }
            sw.Stop();

            Console.WriteLine("Done in {0}ms.", sw.ElapsedMilliseconds);

            Console.ReadKey();
        }

        static void TestSchmitzVertexFromHermiteData()
        {
            while (true)
            {
                HermiteData hermite = new HermiteData(new List<Microsoft.Xna.Framework.Vector3>(), new List<Microsoft.Xna.Framework.Vector3>());

                Console.WriteLine("Enter position 1:");
                string pos1str = Console.ReadLine();
                Console.WriteLine("Enter normal 1:");
                string normal1str = Console.ReadLine();
                Console.WriteLine("Enter position 2:");
                string pos2str = Console.ReadLine();
                Console.WriteLine("Enter normal 2:");
                string normal2str = Console.ReadLine();

                string[] pos1parts = pos1str.Split(',');
                string[] normal1parts = normal1str.Split(',');
                string[] pos2parts = pos2str.Split(',');
                string[] normal2parts = normal2str.Split(',');

                Vector3 pos1 = new Vector3(Single.Parse(pos1parts[0].Trim()), Single.Parse(pos1parts[1].Trim()), Single.Parse(pos1parts[2].Trim()));
                Vector3 normal1 = new Vector3(Single.Parse(normal1parts[0].Trim()), Single.Parse(normal1parts[1].Trim()), Single.Parse(normal1parts[2].Trim()));
                Vector3 pos2 = new Vector3(Single.Parse(pos2parts[0].Trim()), Single.Parse(pos2parts[1].Trim()), Single.Parse(pos2parts[2].Trim()));
                Vector3 normal2 = new Vector3(Single.Parse(normal2parts[0].Trim()), Single.Parse(normal2parts[1].Trim()), Single.Parse(normal2parts[2].Trim()));
                normal1.Normalize();
                normal2.Normalize();

                Console.WriteLine("Position 1: {0}", pos1);
                Console.WriteLine("Normal 1: {0}", normal1);
                Console.WriteLine("Position 2: {0}", pos2);
                Console.WriteLine("Normal 2: {0}", normal2);

                hermite.Add(pos1, normal1);
                hermite.Add(pos2, normal2);

                Stopwatch sw = new Stopwatch();

                sw.Start();
                Vector3 pos = DualContouring.SchmitzVertexFromHermiteData(hermite, 0.001f, 25);
                sw.Stop();

                Console.WriteLine("Out: {0}", pos);
                Console.WriteLine("Took {0}ms", sw.ElapsedMilliseconds);
            }
        }

        static void TestEdges()
        {
            while (true)
            {
                Console.WriteLine("Enter position 1:");
                string pos1str = Console.ReadLine();
                Console.WriteLine("Enter position 2:");
                string pos2str = Console.ReadLine();

                string[] pos1parts = pos1str.Split(',');
                string[] pos2parts = pos2str.Split(',');

                Vector3I pos1 = new Vector3I(Int32.Parse(pos1parts[0].Trim()), Int32.Parse(pos1parts[1].Trim()), Int32.Parse(pos1parts[2].Trim()));
                Vector3I pos2 = new Vector3I(Int32.Parse(pos2parts[0].Trim()), Int32.Parse(pos2parts[1].Trim()), Int32.Parse(pos2parts[2].Trim()));

                Edge e = new Edge(pos1, pos2);

                Vector3I[] surrounding = e.GetCubePositions();

                for (int i = 0; i < surrounding.Length; i++)
                    Console.WriteLine(surrounding[i]);
            }
        }

        public static void Main(String[] args)
        {
            TestEdges();
        }
    }
}
#endif