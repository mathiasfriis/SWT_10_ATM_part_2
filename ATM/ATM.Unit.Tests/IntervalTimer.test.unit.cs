﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using NUnit.Framework;
using TransponderReceiver;
using System.Threading;
using ATM.Logger;
using ATM.Render;
using NSubstitute;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class IntervalTimer_test_unit
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
        List<FlightEvent> seperationEvents;
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
            seperationEvents = new List<FlightEvent>();
            tracks = new List<TrackData>();
        
            uut = new ATMclass(consoleOutput, fileOutput, fakeAirspace, transponderReceiver);
        }



        #region IntervalTimer_TrackEntered
        [Test]
        public void Intervaltimer_trackenteredevent()
        {
            TrackData trackData1 = new TrackData("TEST1", 12000, 12000, 1000, "14322018", 10, 270, consoleOutput);

            string time = trackData1.TimeStamp;

            TrackEnteredEvent TrackEnteredEvent = new TrackEnteredEvent(time, trackData1, true, consoleOutput, fileOutput);

            //Wait 6 seconds to check if isRaised flag has been set to False by the IntervalTimers Start and TimerElapsed function
            Thread.Sleep(6000);

            //Check if isRaised flag has been set to False
            Assert.That(() => TrackEnteredEvent.isRaised.Equals(false));

        }
        #endregion

        #region IntervalTimer_TrackLeft
        [Test]
        public void Intervaltimer_trackleftevent()
        {
            TrackData trackData1 = new TrackData("TEST1", 12000, 12000, 1000, "14322018", 10, 270, consoleOutput);

            string time = trackData1.TimeStamp;

            TrackLeftEvent TrackLeftEvent = new TrackLeftEvent(time, trackData1, true, consoleOutput, fileOutput);

            //Wait 6 seconds to check if isRaised flag has been set to False by the IntervalTimers Start and TimerElapsed function
            Thread.Sleep(6000);

            //Check if isRaised flag has been set to False
            Assert.That(() => TrackLeftEvent.isRaised.Equals(false));

        }
        #endregion


    }
}
