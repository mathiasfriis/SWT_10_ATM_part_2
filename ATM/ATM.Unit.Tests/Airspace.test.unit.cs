using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class Airspace_test_unit
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

        #region Airspace
        [Test]
        public void airspace_coordinateInAirspace_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, 1000).Equals(true));
        }

        #region CoordinatesTooLow
        [Test]
        public void airspace_xCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMin - 1, 50000, 1000).Equals(false));
        }

        [Test]
        public void airspace_yCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMin - 1, 1000).Equals(false));
        }

        [Test]
        public void airspace_zCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMin - 1).Equals(false));
        }
        #endregion

        #region CoordinatesTooHigh
        [Test]
        public void airspace_xCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMax + 1, 50000, 1000).Equals(false));
        }

        [Test]
        public void airspace_yCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMax + 1, 1000).Equals(false));
        }

        [Test]
        public void airspace_zCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMax + 1).Equals(false));
        }
        #endregion

        #region CoordinatesLowerBoundary
        [Test]
        public void airspace_xCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMin, 50000, 1000).Equals(true));
        }

        [Test]
        public void airspace_yCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMin, 1000).Equals(true));
        }

        [Test]
        public void airspace_zCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMin).Equals(true));
        }
        #endregion

        #region CoordinatesUpperBoundary
        [Test]
        public void airspace_xCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMax, 50000, 1000).Equals(true));
        }

        [Test]
        public void airspace_yCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMax, 1000).Equals(true));
        }

        [Test]
        public void airspace_zCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMax).Equals(true));
        }
        #endregion
        #endregion

    }
}
