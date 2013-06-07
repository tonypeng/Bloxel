using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter test name:");
                string name = Console.ReadLine();

                Type type = Type.GetType(name, true);

                ITest test = (ITest)Activator.CreateInstance(type);

                test.run();
            }
        }
    }
}
