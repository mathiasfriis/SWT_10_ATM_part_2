using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using ATM.Logger;
using ATM.Render;
using NSubstitute;
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
        IAirspace fakeAirspace;
        IConsoleOutput consoleOutput;
        IFileOutput fileOutput;
        ITransponderReceiver transponderReceiver;
        List<Event> seperationEvents;
        List<TrackData> tracks;
        string timestamp;

        ATMclass uut;

        [SetUp]
        public void setup()
        {
            //Setup stuff
            airspace = new Airspace(xMin, xMax, yMin, yMax, zMin, zMax);
            fakeAirspace = Substitute.For<IAirspace>();
            consoleOutput = Substitute.For<IConsoleOutput>();
            fileOutput = Substitute.For<IFileOutput>();
            //Make new fake TransponderReceiver.
            transponderReceiver = Substitute.For<ITransponderReceiver>();
            seperationEvents = new List<Event>();
            tracks = new List<TrackData>();
            timestamp = "235928121999";

            uut = new ATMclass(consoleOutput, fileOutput, fakeAirspace, transponderReceiver);
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
