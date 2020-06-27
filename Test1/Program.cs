using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Test t = new Test();
            t.Run1();
            await t.Run2();
        }
    }

    class Test
    {
        public async void Run1()
        {
            await Task.Delay(2000);
            Console.WriteLine("In run1");
        }

        public async Task Run2()
        {
            await Task.Delay(2000);
            Console.WriteLine("In run2");
        }
    }
}
