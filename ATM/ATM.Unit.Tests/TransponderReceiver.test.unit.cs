using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using ATM.Logger;
using ATM.Render;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    public class TransponderReceiver_test_unit
    {
        readonly double xMin = 10000;
        readonly double xMax = 90000;
        readonly double yMin = 10000;
        readonly double yMax = 90000;
        readonly double zMin = 500;
        readonly double zMax = 20000;
        Airspace airspace;
        IAirspace fakeAirspace;
        IConsoleOutput consoleOutput;
        IFileOutput fileOutput;
        ITransponderReceiver transponderReceiver;
        List<TrackData> tracks;
        

        ATMclass uut;

        [SetUp]
        public void Setup()
        {
            //Setup stuff
            airspace = new Airspace(xMin, xMax, yMin, yMax, zMin, zMax);
            fakeAirspace = Substitute.For<IAirspace>();
            consoleOutput = Substitute.For<IConsoleOutput>();
            fileOutput = Substitute.For<IFileOutput>();
            //Make new fake TransponderReceiver.
            transponderReceiver = Substitute.For<ITransponderReceiver>(); ;
            tracks = new List<TrackData>();
            

            uut = new ATMclass(consoleOutput, fileOutput, fakeAirspace, transponderReceiver);
        }

        #region TransponderReceiver
        [Test]
        public void TransponderReceiver_AttachATM_observersCountIs1()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver, consoleOutput);

            system.Attach(uut);

            Assert.That(() => system.getObserverCount().Equals(1));
        }

        [Test]
        public void TransponderReceiver_AttachATMthenDetach_observersCountIs0()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver, consoleOutput);

            system.Attach(uut);
            system.Detach(uut);

            Assert.That(() => system.getObserverCount().Equals(0));
        }

        [Test]
        public void TransponderReceiver_DetachATMwithoutAttachingIt_observersCountIs0()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver, consoleOutput);

            system.Detach(uut);

            Assert.That(() => system.getObserverCount().Equals(0));
        }

        [Test]
        public void TransponderReceiver_3NewTracksWithDifferentTags_TrackCountIs3()
        {
            //Test inspired by "TransponderReceiverUser" by FRABJ.
            // Make a fake Transponder Data Receiver
            var _fakeTransponderReceiver = Substitute.For<ITransponderReceiver>();
            // Inject the fake TDR
            var transponderReceiver = new TransponderReceiver(_fakeTransponderReceiver, consoleOutput);

            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(consoleOutput, fileOutput, airspace, this.transponderReceiver);

            // Setup test data
            List<string> testData = new List<string>
            {
                "ATR423;39045;12932;14000;20151006213456789",
                "BCD123;10005;85890;12000;20151006213456789",
                "XYZ987;25059;75654;4000;20151006213456789"
            };

            //Attach uut to receiver
            transponderReceiver.Attach(uut);

            // Act: Trigger the fake object to execute event invocation
            _fakeTransponderReceiver.TransponderDataReady
                += Raise.EventWith(this, new RawTransponderDataEventArgs(testData));

            Assert.That(uut._currentTracks.Count.Equals(3));
        }

        [Test]
        public void TransponderReceiver_3NewTracksWithSameTags_TrackCountIs1()
        {
            //Test inspired by "TransponderReceiverUser" by FRABJ.
            // Make a fake Transponder Data Receiver
            var _fakeTransponderReceiver = Substitute.For<ITransponderReceiver>();
            // Inject the fake TDR
            var transponderReceiver = new TransponderReceiver(_fakeTransponderReceiver, consoleOutput);

            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(consoleOutput, fileOutput, airspace, this.transponderReceiver);

            // Setup test data
            List<string> testData = new List<string>
            {
                "ATR423;39045;12932;14000;20151006213456789",
                "ATR423;10005;85890;12000;20151006213456789",
                "ATR423;25059;75654;4000;20151006213456789"
            };

            //Attach uut to receiver
            transponderReceiver.Attach(uut);

            // Act: Trigger the fake object to execute event invocation
            _fakeTransponderReceiver.TransponderDataReady
                += Raise.EventWith(this, new RawTransponderDataEventArgs(testData));

            Assert.That(uut._currentTracks.Count.Equals(1));
        }
        #endregion
    }
}
