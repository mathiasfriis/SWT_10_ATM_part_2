﻿using ATM.Events;
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
    class IntegrationTest3
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
        TransponderReceiver transponderReceiver;

        [SetUp]
        public void Setup()
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

            //Create new ATM.TransponderReceiver for simulating inputs from the TransponderReceiver from the dll.
            transponderReceiver = new TransponderReceiver(fakeTransponderReceiver, fakeConsoleOutput);

            //Set up T's
            ATM = new ATMclass(fakeConsoleOutput, fakeFileOutput, airspace, fakeTransponderReceiver);

            //Attach ATM, so that updates to the transponderReceiver updates data in the ATM
            transponderReceiver.Attach(ATM);

        }

        #region Events

        #region Renderer
        #region trackEnteredEvent
        [Test]
        public void TrackEnteredEvent_NewTrackInAirspaceAdded_RendererPrintsExpectedString()
        {
            string expectedString = "Track entered airspace - Occurencetime: 20181224200050123 Involved track: ABC123";
            
            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }

        [Test]
        public void TrackEnteredEvent_TrackEnteredAirspaceLessThan5SecondsAgo_IsStillBeingRendered()
        {
            string expectedString = "Track entered airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

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

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

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

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Same track not in airspace
            List<string> data2 = new List<string>
            {
                "ABC123;1030000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent2 = new RawTransponderDataEventArgs(data2);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent2);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

        [Test]
        public void TrackLeftEvent_TrackLeftAirspaceLessThan5SecondsAgo_IsStillBeingRendered()
        {
            string expectedString = "Track left airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Same track not in airspace
            List<string> data2 = new List<string>
            {
                "ABC123;1030000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent2 = new RawTransponderDataEventArgs(data2);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent2);

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

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Same track not in airspace
            List<string> data2 = new List<string>
            {
                "ABC123;1030000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent2 = new RawTransponderDataEventArgs(data2);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent2);

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
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";
            
            //Same track not in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123",
                "DEF123;30001;30001;3001;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString2));
        }

        [Test]
        public void SeperationEvent_CollidingTracksAddedWaitForALongTime_ConsoleIsStillRendering()
        {
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";

            //Same track not in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123",
                "DEF123;30001;30001;3001;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

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

            //Same track not in airspace
            List<string> data1 = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123",
                "DEF123;30001;30001;3001;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent1 = new RawTransponderDataEventArgs(data1);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent1);

            //Update X-coordinate of one track, so that seperation event conditions are no longer met.
            List<string> data2 = new List<string>
            {
                "ABC123;50000;30000;3000;20181224200050123",
            };

            //Create new event
            RawTransponderDataEventArgs newEvent2 = new RawTransponderDataEventArgs(data2);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent2);

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

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString));
        }

        [Test]
        public void TrackEnteredEvent_NewTrackInAirspaceAddedWait5sec_LoggerStillOnlyWroteOnce()
        {
            string expectedString = "Track entered airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

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

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Same track not in airspace
            List<string> data2 = new List<string>
            {
                "ABC123;1030000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent2 = new RawTransponderDataEventArgs(data2);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent2);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString));
        }

        [Test]
        public void TrackLeftEvent_TrackLeftAirspace_LoggerStillOnlyWroteOnce()
        {
            string expectedString = "Track left airspace - Occurencetime: 20181224200050123 Involved track: ABC123";

            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Same track not in airspace
            List<string> data2 = new List<string>
            {
                "ABC123;1030000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent2 = new RawTransponderDataEventArgs(data2);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent2);

            //Wait 5 secs
            Thread.Sleep(5000);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString));
        }
        #endregion

        #region seperationEvent
        [Test]
        public void SeperationEvent_CollidingTracksAdded_LoggerExpectedString()
        {
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";
            //Same track not in airspace
            List<string> data1 = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123",
                "DEF123;30001;30001;3001;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent1 = new RawTransponderDataEventArgs(data1);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent1);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString2));
        }

        [Test]
        public void SeperationEvent_CollidingTracksAddedWait5sec_LoggerStillOnlyWroteOnce()
        {
            string expectedString2 = "Separation event - Occurencetime: 20181224200050123 Involved tracks: DEF123, ABC123";

            //Same track not in airspace
            List<string> data1 = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123",
                "DEF123;30001;30001;3001;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent1 = new RawTransponderDataEventArgs(data1);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent1);

            //Wait 5 secs
            Thread.Sleep(5000);

            fakeFileOutput.Received(1).Write(Arg.Is<string>(expectedString2));
        }

        #endregion
        #endregion

        #endregion

        #region Tracks

        #region TrackData

        [Test]
        public void ATMclass_TrackData_AddNewTrack()
        {
            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Course and speed = 0, because we need to observe two instances of trackData for the same track, to calculate speed and course
            string expectedString =
                "ABC123 - ( 30000, 30000, 3000) - Speed: 0 m/s - Course: 0 degrees";

            //Sleep for a bit to make sure that the render function has been called
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

        [Test]
        public void ATMclass_TrackData_NewTrackOutOfAirSpace()
        {
            //Track not in airspace
            List<string> data = new List<string>
            {
                "ABC123;1030000;30000;3000;20181224200050123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            fakeConsoleOutput.Received(0).Print(Arg.Is<string>(str => str.Contains("Track")));
        }

        [Test]
        public void ATMclass_TrackData_TrackUpdatedCorrectly()
        {
            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Same Track - moved 30m in x-direction over 1 second
            List<string> dataUpdated = new List<string>
            {
                "ABC123;30030;30000;3000;20181224200051123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);
            RawTransponderDataEventArgs newEventUpdated = new RawTransponderDataEventArgs(dataUpdated);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEventUpdated);

            //ClearReceivedCalls() Da vi ikke er interesseret i trackData1 bliver renderet (Testest i testen forinden)
            fakeConsoleOutput.ClearReceivedCalls();
            
            //30 m/s because it moved 30m in one direction over 1 sec, and course = 0, because it only moved in x-direction
            string expectedString =
                "ABC123 - ( 30030, 30000, 3000) - Speed: 30 m/s - Course: 0 degrees";

            //Sleep for a bit to make sure that the render function has been called
            Thread.Sleep(200);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }


        [Test]
        public void ATMclass_TrackData_TrackIsNoLongerInAirspace()
        {
            //Track in airspace
            List<string> data = new List<string>
            {
                "ABC123;30000;30000;3000;20181224200050123"
            };

            //Same Track - moved 30m in x-direction over 1 second
            List<string> dataUpdated = new List<string>
            {
                "ABC123;103000;30000;3000;20181224200051123"
            };

            //Create new event
            RawTransponderDataEventArgs newEvent = new RawTransponderDataEventArgs(data);
            RawTransponderDataEventArgs newEventUpdated = new RawTransponderDataEventArgs(dataUpdated);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEvent);

            //Give new event to transponderReceiver
            transponderReceiver.ReceiverOnTransponderDataReady(new object(), newEventUpdated);
            
            //Sleep for more than 5 seconds, so TrackLeftEvent is no longer rendered
            Thread.Sleep(5200);

            fakeConsoleOutput.ClearReceivedCalls();

            //Sleep for a bit to make sure renderer has been caleld
            Thread.Sleep(200);

            fakeConsoleOutput.Received(0).Print(Arg.Is<string>(str => str.Contains("Track")));
        }

        #endregion

        #endregion
    }
}
