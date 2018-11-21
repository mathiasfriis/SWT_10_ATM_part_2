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

        [Test]
        public void TrackEnteredEvent_NewTrackInAirspaceAdded_RendererPrintsExpectedString()
        {
            string expectedString = "Track entered airspace - Occurencetime: 235928121999 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(100);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));

        }

        [Test]
        public void TrackLeftEvent_NewTrackInAirspaceAdded_RendererPrintsExpectedString()
        {
            string expectedString = "Track left airspace - Occurencetime: 235928121999 Involved track: ABC123";

            ATM.HandleNewTrackData(trackData1);

            trackData1._CurrentXcord += 100000;

            ATM.HandleNewTrackData(trackData1);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(100);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

        [Test]
        public void SeperationEvent_CollidingTracksAdded_RendererPrintsExpectedString()
        {
            string expectedString = "Seperation event - Occurencetime: 235928121999 Involved tracks: ABC123, DEF123";

            ATM.HandleNewTrackData(trackData1);
            ATM.HandleNewTrackData(trackData2);

            //Sleep for a bit, to make sure that new event has been rendered
            Thread.Sleep(100);

            fakeConsoleOutput.Received().Print(Arg.Is<string>(expectedString));
        }

    }
}
