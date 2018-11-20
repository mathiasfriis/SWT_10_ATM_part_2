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
using System.Threading;
using TransponderReceiver;

namespace ATM.Tests.Integration
{
    [TestFixture]
    class IntegrationTestPart2
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
            tracks = new List<TrackData>()
            {
                trackData1,
                trackData2
            };
            //seperationEvent = new SeperationEvent("201811071337000", tracks, true);
            //trackEnteredEvent = new TrackEnteredEvent(trackData1._TimeStamp, trackData1, true);
            //trackLeftEvent = new TrackLeftEvent(trackData1._TimeStamp, trackData1, true);
            airspace = new Airspace(10000, 90000, 10000, 90000, 500, 20000);

            //Set up T's
            atmClass = new ATMclass(fakeConsoleOutput, fakeFileOutput, airspace, fakeTransponderReceiver);

        }


        #region TrackData

        [Test]
        public void ATMclass_TrackData_AddTrackToCurrentTracks()
        {
            TrackData trackData3 = new TrackData("DEF456", 10002, 10002, 1002, "201811071339000", 0, 0, fakeConsoleOutput);
            atmClass.HandleNewTrackData(trackData3);

            string expectedString =
                $"{trackData3._Tag} - ( {trackData3._CurrentXcord}, {trackData3._CurrentYcord}, {trackData3._CurrentZcord}) - Speed: {trackData3._CurrentHorzVel} m/s - Course: {trackData3._CurrentCourse} degrees";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }


        [Test]
        public void ATMclass_TrackData_UpdataTrackInCurrentTracks()
        {
            TrackData trackData1 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);
            TrackData trackData2 = new TrackData("DEF456", 10002, 10002, 1002, "20181107134000000", 0, 0, fakeConsoleOutput);
            atmClass.HandleNewTrackData(trackData1);
            atmClass.HandleNewTrackData(trackData2);

            string expectedString =
                $"{trackData2._Tag} - ( {trackData2._CurrentXcord}, {trackData2._CurrentYcord}, {trackData2._CurrentZcord}) - Speed: {trackData2._CurrentHorzVel} m/s - Course: {trackData2._CurrentCourse} degrees";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }

        #endregion


        #region Event

        [Test]
        public void ATMclass_Event_TrackEnteredAirspaceEvent()
        {
            TrackData trackData1 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData1);

            string expectedString =
                $"Track entered airspace - Occurencetime: {trackData1._TimeStamp} Involved track: {trackData1._Tag}";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }
        /* TrackLeftAirspace event endnu ikke implementeret i atm
        [Test]
        public void ATMclass_Event_TrackleftAirspaceEvent()
        {
            TrackData trackData1 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData1);

            TrackData trackData2 = new TrackData("DEF456", 9000, 900, 400, "20181107134000000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData2);

            string expectedString =
                $"Track left airspace - Occurencetime: {trackData2._TimeStamp} Involved track: {trackData2._Tag}";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }*/
        /*
        [Test]
        public void ATMclass_Event_TwoTracksRaiseASeperationEvent()
        {
            TrackData trackData1 = new TrackData("DEF456", 11002, 11002, 1202, "20181107133900000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData1);

            TrackData trackData2 = new TrackData("ABC123", 10002, 10002, 1001, "20181107134000000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData2);

            string expectedString =
                $"Separation event - Occurencetime: {trackData2._TimeStamp} Involved tracks: {trackData1._Tag}, {trackData2._Tag}";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }
        */
        #endregion


        #region Airspace
        [Test]
        public void ATMclass_Airspace_NewTrackIsInsideAirspace()
        {
            TrackData trackData1 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData1);

            Assert.That(() => atmClass._currentTracks.Exists(x => x._Tag == trackData1._Tag) == true);
        }
        [Test]
        public void Airspace_HandleNewTrackData_TrackIsNoLongerInsideAirspace()
        {
            TrackData trackData1 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData1);

            TrackData trackData2 = new TrackData("DEF456", 9000, 900, 400, "20181107134000000", 0, 0, fakeConsoleOutput);

            atmClass.HandleNewTrackData(trackData2);

            Assert.That(() => atmClass._currentTracks.Exists(x => x._Tag == trackData2._Tag) == false);
        }

        #endregion


        #region TimedEvent



        #endregion
    }

}
