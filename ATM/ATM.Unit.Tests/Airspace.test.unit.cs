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
        List<Event> seperationEvents;
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
            transponderReceiver = Substitute.For<ITransponderReceiver>();
            seperationEvents = new List<Event>();
            tracks = new List<TrackData>();

            uut = new ATMclass(consoleOutput, fileOutput, fakeAirspace, transponderReceiver);
        }

        #region Airspace
        [Test]
        public void Airspace_coordinateInAirspace_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, 1000).Equals(true));
        }

        #region CoordinatesTooLow
        [Test]
        public void Airspace_xCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMin - 1, 50000, 1000).Equals(false));
        }

        [Test]
        public void Airspace_yCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMin - 1, 1000).Equals(false));
        }

        [Test]
        public void Airspace_zCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMin - 1).Equals(false));
        }
        #endregion

        #region CoordinatesTooHigh
        [Test]
        public void Airspace_xCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMax + 1, 50000, 1000).Equals(false));
        }

        [Test]
        public void Airspace_yCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMax + 1, 1000).Equals(false));
        }

        [Test]
        public void Airspace_zCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMax + 1).Equals(false));
        }
        #endregion

        #region CoordinatesLowerBoundary
        [Test]
        public void Airspace_xCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMin, 50000, 1000).Equals(true));
        }

        [Test]
        public void Airspace_yCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMin, 1000).Equals(true));
        }

        [Test]
        public void Airspace_zCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMin).Equals(true));
        }
        #endregion

        #region CoordinatesUpperBoundary
        [Test]
        public void Airspace_xCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMax, 50000, 1000).Equals(true));
        }

        [Test]
        public void Airspace_yCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMax, 1000).Equals(true));
        }

        [Test]
        public void Airspace_zCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMax).Equals(true));
        }
        #endregion
        #endregion

    }
}
