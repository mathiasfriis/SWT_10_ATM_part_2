﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ATM.Events;
using ATM.IntervalTimer;
using ATM.Logger;
using ATM.Render;
using NSubstitute;
using NUnit.Framework;
using TransponderReceiver;

namespace ATM.Tests.Integration
{
    [TestFixture]
    class IntegrationTest1
    {
        // Vi laver en fake af vores TransponderReceiver
        private ITransponderReceiver fakeTransponderReceiver;

        //S's - Stubs
        private IConsoleOutput fakeConsoleOutput;
        private IFileOutput fakeFileOutput;


        //X's - Modules under test
        private Airspace airspace;

        //T's - Modules acted upon.
        private ATMclass atmClass;

        [SetUp]
        public void setup()
        {
            //fakes
            fakeTransponderReceiver = Substitute.For<ITransponderReceiver>();
            
            //Set up S's
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            fakeFileOutput = Substitute.For<IFileOutput>();

            //Set up X's
            airspace = new Airspace(10000, 90000, 10000, 90000, 500, 20000);

            //Set up T's
            atmClass = new ATMclass(fakeConsoleOutput, fakeFileOutput, airspace, fakeTransponderReceiver);

        }


        #region TrackData

        [Test]
        public void ATMclass_TrackData_AddNewTrack()
        {
            //Opretter et track som er inden for vores Airspace
            TrackData trackData =
                new TrackData("DEF456", 10002, 10002, 1002, "201811071339000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData);

            string expectedString =
                $"{trackData.Tag} - ( {trackData.CurrentXcord}, {trackData.CurrentYcord}, {trackData.CurrentZcord}) - Speed: {trackData.CurrentHorzVel} m/s - Course: {trackData.CurrentCourse} degrees";

            //Sleep for a bit to make sure that the render function has been called
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

        [Test]
        public void ATMclass_TrackData_NewTrackOutOfAirSpace()
        {
            //Opretter et track som ikke er inden for vores Airspace
            TrackData trackData =
                new TrackData("DEF456", 10002, 9002, 1002, "201811071339000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData);

            fakeConsoleOutput.Received(0).Print(Arg.Is<string>(str => str.Contains("Track")));
        }

        [Test]
        public void ATMclass_TrackData_UpdataTrackInCurrentTracks()
        {
            //Opretter et track som er inden for vores Airspace
            TrackData trackData1 =
                new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);
            
            //Opretter et nyt track med samme tag som trackData1, stadig inden for airspace
            TrackData trackData2 =
                new TrackData("DEF456", 10002, 10002, 1002, "20181107134000000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData1);

            //ClearReceivedCalls() Da vi ikke er interesseret i trackData1 bliver renderet (Testest i testen forinden)
            fakeConsoleOutput.ClearReceivedCalls();

            atmClass.HandleNewTrackData(trackData2);
            string expectedString =
                $"{trackData2.Tag} - ( {trackData2.CurrentXcord}, {trackData2.CurrentYcord}, {trackData2.CurrentZcord}) - Speed: {trackData2.CurrentHorzVel} m/s - Course: {trackData2.CurrentCourse} degrees";

            //Sleep for a bit to make sure that the render function has been called
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }
        

        [Test]
        public void ATMclass_TrackData_TrackIsNoLongerInAirspace()
        {
            //Opretter et track som er inden for vores Airspace
            TrackData trackData1 =
                new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);

            //Opretter et track som har samme tag som trackData1 men som ikke længere er inden for vores Airspace
            TrackData trackData2 =
                new TrackData("DEF456", 10002, 9002, 1002, "20181107134000000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData1);
            
            //Track updated so it leaves airspace
            atmClass.HandleNewTrackData(trackData2);

            string expectedString =
                $"Track left airspace - Occurencetime: {trackData2.TimeStamp} Involved track: {trackData2.Tag}";

            //Sleep for more than 5 seconds, so TrackLeftEvent is no longer rendered
            Thread.Sleep(5200);

            fakeConsoleOutput.ClearReceivedCalls();

            //Sleep for a bit to make sure renderer has been caleld
            Thread.Sleep(200);

            fakeConsoleOutput.Received(0).Print(Arg.Is<string>(str => str.Contains("Track")));
        }

        #endregion
    }
}
