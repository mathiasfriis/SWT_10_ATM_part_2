using System;
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

        double xMin = 10000;
        double xMax = 90000;
        double yMin = 10000;
        double yMax = 90000;
        double zMin = 500;
        double zMax = 20000;
        Airspace airspace;
        IAirspace fakeAirspace;
        ILogger logger;
        IRenderer renderer;
        ITransponderReceiver TransponderReceiver;
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
            logger = Substitute.For<ILogger>();
            renderer = Substitute.For<IRenderer>();
            //Make new fake TransponderReceiver.
            seperationEvents = new List<Event>();
            tracks = new List<TrackData>();
            timestamp = "235928121999";
        
            uut = new ATMclass(logger, renderer, fakeAirspace);
        }



        #region IntervalTimer_TrackEntered
        [Test]
        public void intervaltimer_trackenteredevent()
        {
            TrackData trackData1 = new TrackData("TEST1", 12000, 12000, 1000, "14322018", 10, 270);

            string time = trackData1._TimeStamp;

            TrackEnteredEvent TrackEnteredEvent = new TrackEnteredEvent(time, trackData1, true, renderer, logger);
            
            //Wait 6 seconds to check if isRaised flag has been set to False by the IntervalTimers Start and TimerElapsed function
            Thread.Sleep(6);

            //Check if isRaised flag has been set to False
            Assert.That(() => TrackEnteredEvent._isRaised.Equals(false));

        }
        #endregion

        #region IntervalTimer_TrackLeft
        [Test]
        public void intervaltimer_trackleftevent()
        {
            TrackData trackData1 = new TrackData("TEST1", 12000, 12000, 1000, "14322018", 10, 270);

            string time = trackData1._TimeStamp;

            TrackLeftEvent TrackLeftEvent = new TrackLeftEvent(time, trackData1, true, renderer, logger);

            //Wait 6 seconds to check if isRaised flag has been set to False by the IntervalTimers Start and TimerElapsed function
            Thread.Sleep(6);

            //Check if isRaised flag has been set to False
            Assert.That(() => TrackLeftEvent._isRaised.Equals(false));

        }
        #endregion


    }
}
