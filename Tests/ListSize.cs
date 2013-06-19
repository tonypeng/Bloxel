using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tests
{
    public class ListSize : ITest
    {
        public void run()
        {
            for (int i = 0; i < 10; i++)
            {
                List<int> l = new List<int>(i);
                
                //Console.WriteLine("Size of list of length {0}: {1} bytes", i, Marshal.SizeOf(l));
            }
        }
    }
}
