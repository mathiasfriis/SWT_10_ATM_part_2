﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class Renderer_test_unit
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

        #region Rendering
        #region renderSeperationEvent
        [Test]
        public void rendering_nothingCalled_RenderSeperationEventHasNotBeenCalled()
        {
            Assert.That(() => renderer.RenderSeperationEvent_TimesCalled.Equals(0));
        }

        [Test]
        public void rendering_RenderSeperationEventCalledWithNoEventsInList_MethodHasNotBeenCalled()
        {
            uut.RenderSeperationEvents();
            Assert.That(() => renderer.RenderSeperationEvent_TimesCalled.Equals(0));
        }

        [Test]
        public void rendering_RenderSeperationEventCalledWith2EventsInList_MethodHasBeenCalled2Times()
        {
            List<TrackData> trackDatas = new List<TrackData>()
            {
                new TrackData("ABC",1,2,3,"time",5,6),
                new TrackData("ABC",1,2,3,"time",5,6)
            };
            SeperationEvent seperationEvent1 = new SeperationEvent("time", trackDatas, true);
            uut._currentSeperationEvents.Add(seperationEvent1);
            uut._currentSeperationEvents.Add(seperationEvent1);

            uut.RenderSeperationEvents();
            Assert.That(() => renderer.RenderSeperationEvent_TimesCalled.Equals(2));
        }

        #endregion

        #region renderTrack
        [Test]
        public void rendering_nothingCalled_RenderTrackHasNotBeenCalled()
        {
            Assert.That(() => renderer.RenderTrackData_TimesCalled.Equals(0));
        }

        [Test]
        public void rendering_RenderTracksCalledWithNoTracksInList_RenderTrackHasNotBeenCalled()
        {
            uut.RenderTracks();
            Assert.That(() => renderer.RenderTrackData_TimesCalled.Equals(0));
        }

        [Test]
        public void rendering_RenderTracksCalledWith2TracksInList_RenderTrackHasNotCalled2Times()
        {
            uut._currentTracks.Add(new TrackData("ABC", 1, 2, 3, "time", 1, 2));
            uut._currentTracks.Add(new TrackData("DEF", 1, 2, 3, "time", 1, 2));
            uut.RenderTracks();
            Assert.That(() => renderer.RenderTrackData_TimesCalled.Equals(2));
        }
        #endregion
        #endregion
    }
}
