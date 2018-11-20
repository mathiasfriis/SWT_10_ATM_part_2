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

namespace ATM.Tests.Integration
{
    [TestFixture]
    class IntegrationTestPart2
    {
        //S's - Stubs
        private IConsoleOutput fakeConsoleOutput;
        private IFileOutput fakeFileOutput;
        private IIntervalTimer fakeIntervalTimer;

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
            
            //Set up X's
            //trackData1 = new TrackData("ABC123", 10000, 10000, 1000, "201811071337000", 42, 10);
            //trackData2 = new TrackData("XYZ987", 10001, 10001, 1001, "201811071338000", 42, 10);
            tracks = new List<TrackData>()
            {
                trackData1,
                trackData2
            };
            //seperationEvent = new SeperationEvent("201811071337000", tracks, true);
            //trackEnteredEvent = new TrackEnteredEvent(trackData1._TimeStamp, trackData1, true);
            //trackLeftEvent = new TrackLeftEvent(trackData1._TimeStamp, trackData1, true);
            airspace = new Airspace(0, 13000, 0, 13000, 500, 2000);

            //Set up T's
            //atmClass = new ATMclass(fakeConsoleOutput, fakeFileOutput, airspace);

        }
        
        
        #region TrackData
        /*
        [Test]
        public void TrackData_AddTrackdata_AddTrackToCurrentTracks()
        {
            TrackData trackData3 = new TrackData("DEF456", 10002, 10002, 1002, "201811071339000", 0, 0,fakeConsoleOutput);
            atmClass.AddTrack(trackData3);
            atmClass.RenderTracks();

            string expectedString =
                $"{trackData3._Tag} - ( {trackData3._CurrentXcord}, {trackData3._CurrentYcord}, {trackData3._CurrentZcord}) - Speed: {trackData3._CurrentHorzVel} m/s - Course: {trackData3._CurrentCourse} degrees";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }
        */
        /*
        [Test]
        public void TrackData_UpdataTrackData_UpdataTrackInCurrentTracks()
        {
            TrackData trackData3 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);
            TrackData trackData4 = new TrackData("DEF456", 10002, 10002, 1002, "20181107133900000", 0, 0, fakeConsoleOutput);
            atmClass.AddTrack(trackData3);
            
            atmClass.UpdateTrackData(trackData4);
            atmClass.RenderTracks();

            string expectedString =
                $"{trackData4._Tag} - ( {trackData4._CurrentXcord}, {trackData4._CurrentYcord}, {trackData4._CurrentZcord}) - Speed: {trackData4._CurrentHorzVel} m/s - Course: {trackData4._CurrentCourse} degrees";

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }
        */
        #endregion


        #region Event



        #endregion


        #region Airspace



        #endregion


        #region TimedEvent



        #endregion
    }

}
