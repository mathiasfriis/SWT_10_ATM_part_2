using ATM.Events;
using ATM.IntervalTimer;
using ATM.Logger;
using ATM.Render;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransponderReceiver;

namespace ATM.Tests.Integration
{
    [TestFixture]
    class IntegrationTest2
    {
        //S's - Stubs
        private IConsoleOutput fakeConsoleOutput;
        private IFileOutput fakeFileOutput;

        //X's - Modules under test
        private TrackData trackData1;
        private TrackData trackData2;
        private TrackData trackData3;
        private TrackData trackData4;

        private Airspace airspace;


        //T's - Modules driven.
        private ATMclass ATM;


        //TransponderReceiver, needed for creating an instance of ATMclass
        ITransponderReceiver fakeTransponderReceiver;

        [SetUp]
        public void setup()
        {
            //Set up S's
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            fakeFileOutput = Substitute.For<IFileOutput>();

            //Set up X's
            airspace = new Airspace(10000, 90000, 10000, 90000, 500, 20000);
            trackData1 = new TrackData("ABC123", 30000, 30000, 3000, "20181224200050123", 100, 45, fakeConsoleOutput);
            trackData2 = new TrackData("DEF123", 30001, 30001, 3001, "20181224200050123", 100, 45, fakeConsoleOutput);

            trackData3 = new TrackData("ABC123", 30000, 30000, 3000, "20181224200050123", 100, 45, fakeConsoleOutput);
            trackData4 = new TrackData("DEF123", 50000, 50000, 5000, "20181224200050123", 100, 45, fakeConsoleOutput);

            //Fake transponderReceiver
            fakeTransponderReceiver = Substitute.For<ITransponderReceiver>();

            //Set up T's
            ATM = new ATMclass(fakeConsoleOutput, fakeFileOutput, airspace, fakeTransponderReceiver);

        }

        #region Renderer
        #region trackEnteredEvent
        [Test]
        public void TrackEnteredEvent_NewTrackInAirspaceAdded_RendererPrintsExpectedString()
        {
            string expectedString = "Track entered airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }

        [Test]
        public void TrackEnteredEvent_TrackEnteredAirspaceLessThan5SecondsAgo_IsStillBeingRendered()
        {
            string expectedString = "Track entered airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            //Sleep for a little less than 5 seconds
            Thread.Sleep(4800);

            //Reset consoleOutput, so calls to print done during the first 5 seconds won't count.
            fakeConsoleOutput.ClearReceivedCalls();

            //Wait for some time, to see if any events are rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

        public void TrackEnteredEvent_TrackEnteredAirspaceMoreThan5SecondsAgo_IsNotBeingRenderedAnymore()
        {

            ATM.HandleNewTrackData(trackData1);

            //Sleep for More than 5 seconds
            Thread.Sleep(5200);

            //Reset consoleOutput, so calls to print done during the first 5 seconds won't count.
            fakeConsoleOutput.ClearReceivedCalls();

            //Wait for some time, to see if any events are rendered
            Thread.Sleep(5200);

            //Console did not receive anything
            fakeConsoleOutput.DidNotReceiveWithAnyArgs().Print("Any string");
        }
        #endregion

        #region trackLeftEvent
        [Test]
        public void TrackLeftEvent_TrackLeftAirspace_RendererPrintsExpectedString()
        {
            string expectedString = "Track left airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            trackData1._CurrentXcord += 100000;

            ATM.HandleNewTrackData(trackData1);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

        [Test]
        public void TrackLeftEvent_TrackLeftAirspaceLessThan5SecondsAgo_IsStillBeingRendered()
        {
            string expectedString = "Track left airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            trackData1._CurrentXcord += 100000;

            ATM.HandleNewTrackData(trackData1);

            //Sleep for a little less than 5 seconds
            Thread.Sleep(4800);

            //Reset consoleOutput, so calls to print done during the first 5 seconds won't count.
            fakeConsoleOutput.ClearReceivedCalls();

            //Wait for some time, to see if any events are rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

        public void TrackLeftEvent_TrackLeftAirspaceMoreThan5SecondsAgo_IsNotBeingRenderedAnymore()
        {

            ATM.HandleNewTrackData(trackData1);

            trackData1._CurrentXcord += 100000;

            ATM.HandleNewTrackData(trackData1);

            //Sleep for a little more than 5 seconds
            Thread.Sleep(5200);

            //Reset consoleOutput, so calls to print done during the first 5 seconds won't count.
            fakeConsoleOutput.ClearReceivedCalls();

            //Wait for some time, to see if any events are rendered
            Thread.Sleep(5200);

            //Console did not receive anything
            fakeConsoleOutput.DidNotReceiveWithAnyArgs().Print("Any string");
        }
        #endregion

        #region seperationEvent
        [Test]
        public void SeperationEvent_CollidingTracksAdded_RendererPrintsExpectedString()
        {
            string expectedString1 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: ABC123, DEF123";
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";

            ATM.HandleNewTrackData(trackData1);
            ATM.HandleNewTrackData(trackData2);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString2));
        }

        [Test]
        public void SeperationEvent_CollidingTracksAddedWaitForALongTime_ConsoleIsStillRendering()
        {
            string expectedString1 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: ABC123, DEF123";
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";

            ATM.HandleNewTrackData(trackData1);
            ATM.HandleNewTrackData(trackData2);

            //Sleep for more than 5 seconds, to make sure that seperation event does not stop being rendered after 5 seconds.
            Thread.Sleep(6000);

            //Reset consoleOutput, so calls to print done during the first 5 seconds won't count.
            fakeConsoleOutput.ClearReceivedCalls();

            //Wait for some time, to see if any events are rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString2));
        }

        [Test]
        public void SeperationEvent_TrackNotInCollissionAnymore_ConsoleIsNotRenderingSeperationEvent()
        {
            string expectedString1 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: ABC123, DEF123";
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";

            //Add tracks, so that seperation Event occurs
            ATM.HandleNewTrackData(trackData1);
            ATM.HandleNewTrackData(trackData2);

            //Change x-coordinate for one track, so that seperation event does not occur anymore
            trackData1._CurrentXcord += 5002;
            ATM.HandleNewTrackData(trackData1);

            //Wait for a little time to make sure that seperation event is cleared from list at next render.
            Thread.Sleep(2200);

            //Reset consoleOutput, so calls to print done during the first 5 seconds won't count.
            fakeConsoleOutput.ClearReceivedCalls();

            //Wait for some time, to see if any events are rendered
            Thread.Sleep(5200);

            //Console did not receive any Prints about seperation event
            fakeConsoleOutput.DidNotReceive().Print(Arg.Is<string>(expectedString2));
            fakeConsoleOutput.DidNotReceive().Print(Arg.Is<string>(expectedString1));
        }
        #endregion 
        #endregion

        #region Logger
        #region trackEnteredEvent

        public void Logger_NothingDone_LoggerReceivedNothing()
        {
            fakeFileOutput.DidNotReceiveWithAnyArgs().Write("Any string");
        }

        [Test]
        public void TrackEnteredEvent_NewTrackInAirspaceAdded_LoggerWritesExpectedString()
        {
            string expectedString = "Track entered airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString));
        }

        [Test]
        public void TrackEnteredEvent_NewTrackInAirspaceAddedWait5sec_LoggerStillOnlyWroteOnce()
        {
            string expectedString = "Track entered airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            //Wait 5 secs
            Thread.Sleep(5000);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString));
        }
        #endregion

        #region trackLeftEvent
        [Test]
        public void TrackLeftEvent_TrackLeftAirspace_LoggerExpectedString()
        {
            string expectedString = "Track left airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            //Update track data so that the new position is out of the monitored airspace
            trackData1._CurrentXcord += 100000;
            ATM.HandleNewTrackData(trackData1);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString));
        }

        [Test]
        public void TrackLeftEvent_TrackLeftAirspace_LoggerStillOnlyWroteOnce()
        {
            string expectedString = "Track left airspace - Occurencetime: 20181224200050123 Involved tracWk: ABC123";

            ATM.HandleNewTrackData(trackData1);

            //Update track data so that the new position is out of the monitored airspace
            trackData1._CurrentXcord += 100000;
            ATM.HandleNewTrackData(trackData1);

            //Wait 5 secs
            Thread.Sleep(5000);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString));
        }
        #endregion

        #region seperationEvent
        [Test]
        public void SeperationEvent_CollidingTracksAdded_LoggerExpectedString()
        {
            string expectedString1 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: ABC123, DEF123";
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";

            ATM.HandleNewTrackData(trackData1);
            ATM.HandleNewTrackData(trackData2);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString2));
        }

        [Test]
        public void SeperationEvent_CollidingTracksAddedWait5sec_LoggerStillOnlyWroteOnce()
        {
            string expectedString1 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: ABC123, DEF123";
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";

            ATM.HandleNewTrackData(trackData1);
            ATM.HandleNewTrackData(trackData2);

            //Wait 5 secs
            Thread.Sleep(5000);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString2));
        }

        #endregion W
        #endregion

        //Check that logger receives calls upon creation of new events(all events)

        /*
        [Test]
        public void summin()
        {
            List<string> stringList = new List<string>
            {
                "Testdata213"
            };
            RawTransponderDataEventArgs testData = new RawTransponderDataEventArgs(stringList);
            ATM._transponderReceiver.ReceiverOnTransponderDataReady(new object(), testData);
        }
        */

    }
}
