using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Render
{
    class ConsoleOutput : IConsoleOutput
    {
        public void Print(string toPrint)
        {
            Console.WriteLine(toPrint);
        }
    }
}
