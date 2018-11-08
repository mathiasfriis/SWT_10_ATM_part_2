using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    public class TransponderReceiver_test_unit
    {
        double xMin = 10000;
        double xMax = 90000;
        double yMin = 10000;
        double yMax = 90000;
        double zMin = 500;
        double zMax = 20000;
        Airspace airspace;
        FakeAirspace fakeAirspace;
        FakeLogger logger;
        FakeRenderer renderer;
        ITransponderReceiver TransponderReceiver;
        List<SeperationEvent> seperationEvents;
        List<TrackData> tracks;
        string timestamp;

        ATMclass uut;

        [SetUp]
        public void setup()
        {
            //Setup stuff
            airspace = new Airspace(xMin, xMax, yMin, yMax, zMin, zMax);
            fakeAirspace = new FakeAirspace(xMin, xMax, yMin, yMax, zMin, zMax);
            logger = new FakeLogger();
            renderer = new FakeRenderer();
            //Make new fake TransponderReceiver.
            seperationEvents = new List<SeperationEvent>();
            tracks = new List<TrackData>();
            timestamp = "235928121999";

            uut = new ATMclass(logger, renderer, fakeAirspace);
        }

        #region TransponderReceiver
        [Test]
        public void TransponderReceiver_AttachATM_observersCountIs1()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver);

            system.Attach(uut);

            Assert.That(() => system.getObserverCount().Equals(1));
        }

        [Test]
        public void TransponderReceiver_AttachATMthenDetach_observersCountIs0()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver);

            system.Attach(uut);
            system.Detach(uut);

            Assert.That(() => system.getObserverCount().Equals(0));
        }

        [Test]
        public void TransponderReceiver_DetachATMwithoutAttachingIt_observersCountIs0()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver);

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
            var transponderReceiver = new TransponderReceiver(_fakeTransponderReceiver);

            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(logger, renderer, airspace);

            // Setup test data
            List<string> testData = new List<string>();
            testData.Add("ATR423;39045;12932;14000;20151006213456789");
            testData.Add("BCD123;10005;85890;12000;20151006213456789");
            testData.Add("XYZ987;25059;75654;4000;20151006213456789");

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
            var transponderReceiver = new TransponderReceiver(_fakeTransponderReceiver);

            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(logger, renderer, airspace);

            // Setup test data
            List<string> testData = new List<string>();
            testData.Add("ATR423;39045;12932;14000;20151006213456789");
            testData.Add("ATR423;10005;85890;12000;20151006213456789");
            testData.Add("ATR423;25059;75654;4000;20151006213456789");

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
