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
        public void CheckIfSeperationEventExistsFor_InvolvingSeperationEventExists_ReturnFalse()
        {
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se = new SeperationEvent(timeStamp, tracks, true, consoleOutput, fileOutput);
            uut.events.Add(se);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td1, td2).Equals(true));
        }


        #endregion

        #region checkIfTrackEnteredEventExistsFor

        #endregion

        #region checkIfTrackLeftEventExistsFor

        #endregion
    }
}
