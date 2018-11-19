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
    public class Logger_test_unit
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

        #region Logging

        #region ActiveSeparationEvent logging

        /*
        [Test]
        public void active_logging_nothingCalled_MethodHasNotBeenCalled()
        {
            Assert.That(logger.LogActiveSeparationEvent_timesCalled.Equals(0));
            logger.DidNotReceiveWithAnyArgs().LogActiveEvent();
        }
        */

        /*
        [Test]
        public void active_logging_logActiveSeparationEvent_MethodHasBeenCalled()
        {
            TrackData trackData1 = new TrackData("ABC", 10000, 20000, 3000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 10000, 20000, 3000, timestamp, 100, 10);
          
            uut.CheckForSeperationEvent(trackData1,trackData2);
            Assert.That(logger.LogActiveSeparationEvent_timesCalled.Equals(1));
        }
        */  

        /*
        [Test]
        public void logging_logActiveSeparationEvent_Tag1IsSame()
        {
            TrackData trackData1 = new TrackData("ABC", 10000, 20000, 3000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 10000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut.CheckForSeperationEvent(trackData1,trackData2);
            Assert.That(logger.ParametersList[0]._InvolvedTracks[0]._Tag.Equals(seperationEvent._InvolvedTracks[0]._Tag));
        }
        */

        /*
        [Test]
        public void logging_logActiveSeparationEvent_Tag2IsSame()
        {
            TrackData trackData1 = new TrackData("ABC", 10000, 20000, 3000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 10000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut.CheckForSeperationEvent(trackData1, trackData2);
            Assert.That(logger.ParametersList[0]._InvolvedTracks[1]._Tag.Equals(seperationEvent._InvolvedTracks[1]._Tag));
        }
        */

        /*
        [Test]
        public void logging_logActiveSeparationEvent_OccurenteTimeIsSame()
        {
            TrackData trackData1 = new TrackData("ABC", 10000, 20000, 3000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 10000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut.CheckForSeperationEvent(trackData1, trackData2);
            Assert.That(logger.ParametersList[0]._occurrenceTime.Equals(seperationEvent._occurrenceTime));
        }
        */

        /*
        [Test]
        public void logging_logActiveSeparationEvent_RaisedIsSame()
        {
            TrackData trackData1 = new TrackData("ABC", 10000, 20000, 3000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 10000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut.CheckForSeperationEvent(trackData1, trackData2);
            Assert.That(logger.ParametersList[0]._isRaised.Equals(seperationEvent._isRaised));
        }
        */

        #endregion

        //Inactive mangler at blive lavet
        #region InActiveSeparationEvent logging

        /*
        [Test]
        public void logging_nothingCalled_LogInactiveSeperationEventMethodHasNotBeenCalled()
        {
            Assert.That(logger.LogInactiveSeparationEvent_timesCalled.Equals(0));
        }
        */

        /*
        [Test]
        public void logging_LogInactiveSeperationEvent_MethodHasBeenCalled()
        {
            //Set up seperation event, that when checked, should be removed, 
            //since the conditions for a seperation event no longer are true.
            TrackData trackData1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 20000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut._currentSeperationEvents.Add(seperationEvent);

            uut.RemoveSeparationEvents();
            Assert.That(logger.LogInactiveSeparationEvent_timesCalled.Equals(1));
        }
        */
        /*
        [Test]
        public void logging_LogInactiveSeperationEvent_Tag1IsSameAsTrackData1Tag()
        {
            //Set up seperation event, that when checked, should be removed, 
            //since the conditions for a seperation event no longer are true.
            TrackData trackData1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 20000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut._currentSeperationEvents.Add(seperationEvent);

            uut.RemoveSeparationEvents();
            Assert.That(logger.ParametersList[0]._InvolvedTracks[0]._Tag.Equals(trackData1._Tag));
        }
        */
        /*
        [Test]
        public void logging_LogInactiveSeperationEvent_Tag2IsSameAsTrackData2Tag()
        {
            //Set up seperation event, that when checked, should be removed, 
            //since the conditions for a seperation event no longer are true.
            TrackData trackData1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 20000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut._currentSeperationEvents.Add(seperationEvent);

            uut.RemoveSeparationEvents();
            Assert.That(logger.ParametersList[0]._InvolvedTracks[1]._Tag.Equals(trackData2._Tag));
        }
        */

        /*
        [Test]
        public void logging_LogInactiveSeperationEvent_OccurenceTimeIsSameAsForSeperationEvent()
        {
            //Set up seperation event, that when checked, should be removed, 
            //since the conditions for a seperation event no longer are true.
            TrackData trackData1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 20000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut._currentSeperationEvents.Add(seperationEvent);

            uut.RemoveSeparationEvents();
            Assert.That(logger.ParametersList[0]._occurrenceTime.Equals(seperationEvent._occurrenceTime));
        }
        */
        /*
        [Test]
        public void logging_LogInactiveSeperationEvent_IsRaisedIsSameAsForSeperationEvent()
        {
            //Set up seperation event, that when checked, should be removed, 
            //since the conditions for a seperation event no longer are true.
            TrackData trackData1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 20000, 20000, 3000, timestamp, 100, 10);
            List<TrackData> trackDatas = new List<TrackData>
            {
                trackData1,
                trackData2
            };
            SeperationEvent seperationEvent = new SeperationEvent(timestamp, trackDatas, true);

            uut._currentSeperationEvents.Add(seperationEvent);

            uut.RemoveSeparationEvents();
            Assert.That(logger.ParametersList[0]._isRaised.Equals(seperationEvent._isRaised));
        }
        */

        #endregion

        #endregion
    }
}
