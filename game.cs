using System;
using TAS;

namespace Cuphead
    class Program {
        public static void Main(String[] args) {
            Manager.Start();
            for (;;) {
                Console.WriteLine(TAS.Manager.GetInput());
                System.Threading.Thread.Sleep(1000/60);
            }
            Manager.Stop();
        }
    }
}
