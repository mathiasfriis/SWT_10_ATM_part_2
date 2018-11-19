using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM;
using ATM.Events;
using ATM.Logger;
using ATM.Render;
using NSubstitute;
using NUnit.Framework;
//using ATM.Timer;

namespace ATM.Tests.Integration
{
    [TestFixture]
    public class IntegrationTestPart1
    {
        //S's - Stubs
        private IConsoleOutput fakeConsoleOutput;
        private IFileOutput fakeFileOutput;

        //X's - Modules under test
        private FileLogger logger;
        private ConsoleRenderer renderer;
        //private Timer timer;
        
        //T's - Modules acted upon.
        private TrackData trackData1;
        private TrackData trackData2;
        private List<TrackData> tracks;
        private SeperationEvent seperationEvent;
        private TrackEnteredEvent trackEnteredEvent;
        private TrackLeftEvent trackLeftEvent;



        [SetUp]
        public void setup()
        {
            //Set up S's
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            fakeFileOutput = Substitute.For<IFileOutput>();

            //Set up T's
            trackData1 = new TrackData("ABC123",10000,10000,1000,"201811071337000",42,10);
            trackData1 = new TrackData("XYZ987", 10001, 10001, 1001, "201811071338000", 42, 10);
            tracks = new List<TrackData>()
            {
                trackData1,
                trackData2
            };
            seperationEvent = new SeperationEvent("201811071337000",tracks,true);
            trackEnteredEvent = new TrackEnteredEvent(trackData1._TimeStamp,trackData1,true);
            trackLeftEvent = new TrackLeftEvent(trackData1._TimeStamp,trackData1,true);

            //Set up X's
            logger = new FileLogger();
            renderer = new ConsoleRenderer();
            //timer = new Timer();
        }


        #region Renderer

        /*
        public void Renderer_renderTrackData_OutputReceivesCorrectString()
        {
            trackData1.render();
            string Tag = trackData1._Tag;
            double x = trackData1._CurrentXcord;
            double y = trackData1._CurrentYcord;
            double z = trackData1._CurrentZcord;
            double horzVel = trackData1._CurrentHorzVel;
            double course = trackData1._CurrentCourse;

            fakeConsoleOutput.Received(Arg.Any<String>().Contains(Tag + " - " + "(" + x + "," + y + "," + z + ")" + " - " + "Speed: " + horzVel + "m/s - Course: " + course + " degrees."));
        }
        

        #endregion

        #region Logger

        #region SeperationEvent

        public void Logger_logSeperationEvent_OutputReceivesCorrectString()
        {
            string occurrenceTime = seperationEvent._occurrenceTime;
            string tag1 = seperationEvent._InvolvedTracks[0]._Tag;
            string tag2 = seperationEvent._InvolvedTracks[1]._Tag;

            string expectedString = "Some string that describes the seperation event";

            seperationEvent.render();

            fakeConsoleOutput.Received(Arg.Any<String>().Contains(expectedString);
        }

        #endregion

        #region TrackEnteredEvent
        public void Logger_logTrackEnteredEvent_OutputReceivesCorrectString()
        {
            string occurrenceTime = trackEnteredEvent._occurrenceTime;
            string tag = trackEnteredEvent._InvolvedTracks[0]._Tag;

            string expectedString = "Some string that describes the seperation event";

            seperationEvent.render();

            fakeConsoleOutput.Received(Arg.Any<String>().Contains(expectedString);
        }
        #endregion

        #region TrackLeftEvent
        public void Logger_logTrackLeftEvent_OutputReceivesCorrectString()
        {
            string occurrenceTime = trackEnteredEvent._occurrenceTime;
            string tag = trackEnteredEvent._InvolvedTracks[0]._Tag;

            string expectedString = "Some string that describes the seperation event";

            seperationEvent.render();

            fakeConsoleOutput.Received(Arg.Any<String>().Contains(expectedString);
        }
        #endregion
    */
    #endregion
    }
}
