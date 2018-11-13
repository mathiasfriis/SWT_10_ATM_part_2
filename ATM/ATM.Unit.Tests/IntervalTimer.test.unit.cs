using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using NUnit.Framework;
using TransponderReceiver;

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
        FakeAirspace fakeAirspace;
        FakeLogger logger;
        FakeRenderer renderer;
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
            fakeAirspace = new FakeAirspace(xMin, xMax, yMin, yMax, zMin, zMax);
            logger = new FakeLogger();
            renderer = new FakeRenderer();
            //Make new fake TransponderReceiver.
            seperationEvents = new List<Event>();
            tracks = new List<TrackData>();
            timestamp = "235928121999";

            uut = new ATMclass(logger, renderer, fakeAirspace);
        }


        #region IntervalTimer


        #region IntervalTimer_TrackEntered
        #endregion

        #region IntervalTimer_TrackLeft
        #endregion

        #endregion

    }
}
