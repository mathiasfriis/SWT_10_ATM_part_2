using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ATM;
using ATM.Render;
using ATM.Logger;
using TransponderReceiver; //Needed in order to use the TransponderReceiver dll


namespace ConsoleApplication
{
    class Program
    {
        
        static void Main(string[] args)
        {

            // TEST AF SYSTEM UDEN SEPARATION EVENT
            IConsoleOutput consoleOutput = new ConsoleOutput();
            IFileOutput fileOutput = new FileOutput();
            Airspace airspace = new Airspace(0, 900000, 0, 900000, 0, 200000);

            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver, consoleOutput);

            var atm = new ATMclass(consoleOutput, fileOutput, airspace, receiver);

            system.Attach(atm);


            // CHECK FOR REMOVAL OF EVENTS
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMilliseconds(100);

            var timer = new System.Threading.Timer((e) =>
            {
                //atm.cleanUpEvents();
            }, null, startTimeSpan, periodTimeSpan);


            // RUN INFINITE

            while (true)
                Thread.Sleep(1000);
        }
    }
}
