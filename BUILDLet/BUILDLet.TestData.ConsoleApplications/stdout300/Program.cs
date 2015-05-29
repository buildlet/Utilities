using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;


namespace stdout300
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i <= 300; i++)
            {
                Thread.Sleep(1);

                Console.WriteLine(i);
            }
        }
    }
}
