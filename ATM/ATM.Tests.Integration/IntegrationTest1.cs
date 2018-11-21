using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        //S's - Stubs
        private IConsoleOutput fakeConsoleOutput;
        private IFileOutput fakeFileOutput;
        private IIntervalTimer fakeIntervalTimer;
        private ITransponderReceiver fakeTransponderReceiver;

        //X's - Modules under test
        private TrackData trackData1;
        private TrackData trackData2;
        private List<TrackData> tracks;
        private SeperationEvent seperationEvent;
        private Airspace airspace;
        private TrackEnteredEvent trackEnteredEvent;
        private TrackLeftEvent trackLeftEvent;

        //T's - Modules acted upon.
        private ATMclass atmClass;


        [SetUp]
        public void setup()
        {
            //Set up S's
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            fakeFileOutput = Substitute.For<IFileOutput>();
            fakeIntervalTimer = Substitute.For<IIntervalTimer>();
            fakeTransponderReceiver = Substitute.For<ITransponderReceiver>();

            //Set up X's
            airspace = new Airspace(10000, 90000, 10000, 90000, 500, 20000);

            //Set up T's
            atmClass = new ATMclass(fakeConsoleOutput, fakeFileOutput, airspace, fakeTransponderReceiver);

        }


        #region TrackData

        [Test]
        public void ATMclass_TrackData_AddTrackToCurrentTracks()
        {
            TrackData trackData3 =
                new TrackData("DEF456", 10002, 10002, 1002, "201811071339000", 0, 0, fakeConsoleOutput);
            atmClass.HandleNewTrackData(trackData3);

            string expectedString =
                $"{trackData3._Tag} - ( {trackData3._CurrentXcord}, {trackData3._CurrentYcord}, {trackData3._CurrentZcord}) - Speed: {trackData3._CurrentHorzVel} m/s - Course: {trackData3._CurrentCourse} degrees";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }


       /* [Test]
        public void ATMclass_TrackData_UpdataTrackInCurrentTracks()
        {
            TrackData trackData1 =
                new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);
            
            TrackData trackData2 =
                new TrackData("DEF456", 10002, 10022, 1002, "20181107134000000", 0, 0, fakeConsoleOutput);
            atmClass.HandleNewTrackData(trackData1);

            fakeConsoleOutput.ClearReceivedCalls();
            atmClass.HandleNewTrackData(trackData2);

            string expectedString =
                $"{trackData2._Tag} - ( {trackData2._CurrentXcord}, {trackData2._CurrentYcord}, {trackData2._CurrentZcord}) - Speed: {trackData2._CurrentHorzVel} m/s - Course: {trackData2._CurrentCourse} degrees";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }
        */

        public void ATMclass_TrackData_TrackIsNoLongerInAirspace()
        {
            TrackData trackData1 =
                new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);
            fakeConsoleOutput.ClearReceivedCalls();
            TrackData trackData2 =
                new TrackData("DEF456", 10002, 9002, 1002, "20181107134000000", 0, 0, fakeConsoleOutput);
            atmClass.HandleNewTrackData(trackData1);
            atmClass.HandleNewTrackData(trackData2);

        }

        #endregion
    }
}
