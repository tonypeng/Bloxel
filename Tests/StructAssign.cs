using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class StructAssign : ITest
    {
        struct foo
        {
            public int i;
        }

        static foo f = new foo();

        public void run()
        {
            foo f2 = f;
            f2.i = 1;

            Console.WriteLine("f: {0}, f2: {1}", f.i, f2.i);
        }
    }
}
