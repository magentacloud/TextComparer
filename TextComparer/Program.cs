using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            Comparer comparer = new Comparer();
            comparer.Process();

            Console.ReadLine();
        }
        
    }
}
