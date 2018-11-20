using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using ATM.Render;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ATM.Logger;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class EventList_test_unit
    {
        private EventList uut;
        private TrackData td1;
        private TrackData td2;
        private TrackData td3;
        private string timeStamp;
        IConsoleOutput consoleOutput;
        IFileOutput fileOutput;
        ITransponderReceiver transponderReceiver;
        private IRenderer fakeRenderer;
        private Logger.ILogger fakeLogger;

        [SetUp]
        public void setup()
        {
            fakeRenderer = Substitute.For<IRenderer>();
            fakeLogger = Substitute.For<Logger.ILogger>();
            timeStamp = "235928121999";
            uut = new EventList();
			td1=new TrackData("ABC123",10000,10000,1000,timeStamp,100,45, consoleOutput);
            td2 = new TrackData("DEF123", 10000, 10000, 1000, timeStamp, 100, 45, consoleOutput);
            td3 = new TrackData("XYZ123", 10000, 10000, 1000, timeStamp, 100, 45, consoleOutput);
        }

        #region CheckIfSeperationEventExistsFor

		[Test]
        public void CheckIfSeperationEventExistsFor_NoEventExists_ReturnFalse()
        {
			Assert.That(uut.CheckIfSeperationEventExistsFor(td1,td2).Equals(false));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_TrackEnteredEventExists_ReturnFalse()
        {
			TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td1, true, consoleOutput, fileOutput);
			uut.events.Add(tee);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td1, td2).Equals(false));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_TrackLeftEventExists_ReturnFalse()
        {
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td1, true, consoleOutput, fileOutput);
            uut.events.Add(tle);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td1, td2).Equals(false));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_OtherSeperationEventExists_ReturnFalse()
        {
			List<TrackData> tracks = new List<TrackData>();
			tracks.Add(td1);
			tracks.Add(td2);
            SeperationEvent se = new SeperationEvent(timeStamp, tracks, true, consoleOutput, fileOutput);
            uut.events.Add(se);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td3, td2).Equals(false));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_InvolvingSeperationEventExistsTagsMatchRightOrder_ReturnTrue()
        {
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se = new SeperationEvent(timeStamp, tracks, true, consoleOutput, fileOutput);
            uut.events.Add(se);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td1, td2).Equals(true));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_InvolvingSeperationEventExistsTagsMatchWrongOrder_ReturnTrue()
        {
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se = new SeperationEvent(timeStamp, tracks, true, consoleOutput, fileOutput);
            uut.events.Add(se);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td2, td1).Equals(true));
        }


        #endregion

        #region checkIfTrackEnteredEventExistsFor
        [Test]
        public void CheckIfTrackEnteredEventExistsFor_NoEventExists_ReturnFalse()
        {
            Assert.That(uut.checkIfTrackEnteredEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackEnteredEventExistsFor_OtherTrackEnteredEventExists_ReturnFalse()
        {
            TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td2, true, consoleOutput, fileOutput);
            uut.events.Add(tee);
            Assert.That(uut.checkIfTrackEnteredEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackEnteredEventExistsFor_TrackLeftEventExists_ReturnFalse()
        {
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td2, true, consoleOutput, fileOutput);
            uut.events.Add(tle);
            Assert.That(uut.checkIfTrackEnteredEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackEnteredEventExistsFor_InvolvingTrackEnteredEventExists_ReturnTrue()
        {
            TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td1, true, consoleOutput, fileOutput);
            uut.events.Add(tee);
            Assert.That(uut.checkIfTrackEnteredEventExistsFor(td1).Equals(true));
        }
        #endregion

        #region checkIfTrackLeftEventExistsFor
        [Test]
        public void CheckIfTrackLeftEventExistsFor_NoEventExists_ReturnFalse()
        {
            Assert.That(uut.checkIfTrackLeftEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackLeftEventExistsFor_OtherTrackLeftEventExists_ReturnFalse()
        {
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td2, true, consoleOutput, fileOutput);
            uut.events.Add(tle);
            Assert.That(uut.checkIfTrackLeftEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackLeftEventExistsFor_TrackEnteredEventExists_ReturnFalse()
        {
            TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td2, true, consoleOutput, fileOutput);
            uut.events.Add(tee);
            Assert.That(uut.checkIfTrackLeftEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackLeftEventExistsFor_InvolvingTrackLeftEventExists_ReturnTrue()
        {
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td1, true, consoleOutput, fileOutput);
            uut.events.Add(tle);
            Assert.That(uut.checkIfTrackLeftEventExistsFor(td1).Equals(true));
        }
        #endregion

        #region CleanUpEvents
        [Test]
        public void CleanUpEvents_NoEventsInList_DoesntThrowException()
        {
            uut.cleanUpEvents();
        }

        [Test]
        public void CleanUpEvents_3Events2Raised_AfterCleanUpOnly2Remain()
        {
            //Test is done with seperation events, since the _isRaised-attribute of 
            //TrackEnteredEvent and TrackLeftEvent goes false after a set amount of time, introducing possible errors
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se1 = new SeperationEvent(timeStamp, tracks, true, consoleOutput, fileOutput);
            SeperationEvent se2 = new SeperationEvent(timeStamp, tracks, true, consoleOutput, fileOutput);
            SeperationEvent se3 = new SeperationEvent(timeStamp, tracks, false, consoleOutput, fileOutput);

            uut.events.Add(se1);
            uut.events.Add(se2);
            uut.events.Add(se3);

            Assert.That(uut.events.Count.Equals(3));
            uut.cleanUpEvents();
            Assert.That(uut.events.Count.Equals(2));
        }

        [Test]
        public void CleanUpEvents_5Events1Raised_AfterCleanUpOnly1Remain()
        {
            //Test is done with seperation events, since the _isRaised-attribute of 
            //TrackEnteredEvent and TrackLeftEvent goes false after a set amount of time, introducing possible errors
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se1 = new SeperationEvent(timeStamp, tracks, false, consoleOutput, fileOutput);
            SeperationEvent se2 = new SeperationEvent(timeStamp, tracks, false, consoleOutput, fileOutput);
            SeperationEvent se3 = new SeperationEvent(timeStamp, tracks, true, consoleOutput, fileOutput);
            SeperationEvent se4 = new SeperationEvent(timeStamp, tracks, false, consoleOutput, fileOutput);
            SeperationEvent se5 = new SeperationEvent(timeStamp, tracks, false, consoleOutput, fileOutput);

            uut.events.Add(se1);
            uut.events.Add(se2);
            uut.events.Add(se3);
            uut.events.Add(se4);
            uut.events.Add(se5);

            Assert.That(uut.events.Count.Equals(5));
            uut.cleanUpEvents();
            Assert.That(uut.events.Count.Equals(1));

        }

        #endregion
    }
}
