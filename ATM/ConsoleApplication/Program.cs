﻿using System;
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
            ConsoleRenderer consolerender = new ConsoleRenderer();
            Airspace airspace = new Airspace(10000, 90000, 10000, 90000, 500, 20000);

            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver);

            ATMclass atm = new ATMclass(consoleOutput, fileOutput, airspace);
            system.Attach(atm);

            // TEST AF SYSTEM MED SEPARATION EVENTS
            //TrackData trackData1 = new TrackData("TEST1", 12000, 12000, 1000, "14322018", 10, 270);
            //TrackData trackData2 = new TrackData("TEST2", 12000, 12000, 1000, "14322018", 10, 270);

            //atm._currentTracks.Add(trackData1);
            //atm._currentTracks.Add(trackData2);

            //atm.CheckForSeperationEvents(trackData2);

            // TEST AF SYSTEM MED LOGGER
            TrackData trackData4 = new TrackData("DEF456", 50002, 50002, 5002, "20181107134000000", 0, 0, consoleOutput);

            TrackData trackData3 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, consoleOutput);
            atm.AddTrack(trackData3);
            Thread.Sleep(500);
            atm.HandleNewTrackData(trackData4);
            
            while (true)
                Thread.Sleep(3000);
        }
    }
}
