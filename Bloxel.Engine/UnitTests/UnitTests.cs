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
using System.Linq;
using System.Text;

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

        public static void Main(String[] args)
        {
            TestSimplexNoise();
        }
    }
}
#endif