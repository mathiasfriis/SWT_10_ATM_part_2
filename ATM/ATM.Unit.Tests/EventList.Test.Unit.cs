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
        private IFileOutput fakeFileOutput;
        private IConsoleOutput fakeConsoleOutput;

        [SetUp]
        public void setup()
        {
            fakeFileOutput = Substitute.For<IFileOutput>();
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            timeStamp = "235928121999";
            uut = new EventList();
			td1=new TrackData("ABC123",10000,10000,1000,timeStamp,100,45, fakeConsoleOutput);
            td2 = new TrackData("DEF123", 10000, 10000, 1000, timeStamp, 100, 45, fakeConsoleOutput);
            td3 = new TrackData("XYZ123", 10000, 10000, 1000, timeStamp, 100, 45, fakeConsoleOutput);
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
			TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
			uut.events.Add(tee);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td1, td2).Equals(false));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_TrackLeftEventExists_ReturnFalse()
        {
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(tle);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td1, td2).Equals(false));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_OtherSeperationEventExists_ReturnFalse()
        {
			List<TrackData> tracks = new List<TrackData>();
			tracks.Add(td1);
			tracks.Add(td2);
            SeperationEvent se = new SeperationEvent(timeStamp, tracks, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(se);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td3, td2).Equals(false));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_InvolvingSeperationEventExistsTagsMatchRightOrder_ReturnTrue()
        {
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se = new SeperationEvent(timeStamp, tracks, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(se);
            Assert.That(uut.CheckIfSeperationEventExistsFor(td1, td2).Equals(true));
        }

        [Test]
        public void CheckIfSeperationEventExistsFor_InvolvingSeperationEventExistsTagsMatchWrongOrder_ReturnTrue()
        {
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se = new SeperationEvent(timeStamp, tracks, true, fakeConsoleOutput, fakeFileOutput);
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
            TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td2, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(tee);
            Assert.That(uut.checkIfTrackEnteredEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackEnteredEventExistsFor_TrackLeftEventExists_ReturnFalse()
        {
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td2, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(tle);
            Assert.That(uut.checkIfTrackEnteredEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackEnteredEventExistsFor_InvolvingTrackEnteredEventExists_ReturnTrue()
        {
            TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
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
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td2, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(tle);
            Assert.That(uut.checkIfTrackLeftEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackLeftEventExistsFor_TrackEnteredEventExists_ReturnFalse()
        {
            TrackEnteredEvent tee = new TrackEnteredEvent(timeStamp, td2, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(tee);
            Assert.That(uut.checkIfTrackLeftEventExistsFor(td1).Equals(false));
        }

        [Test]
        public void CheckIfTrackLeftEventExistsFor_InvolvingTrackLeftEventExists_ReturnTrue()
        {
            TrackLeftEvent tle = new TrackLeftEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
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
            SeperationEvent se1 = new SeperationEvent(timeStamp, tracks, true, fakeConsoleOutput, fakeFileOutput);
            SeperationEvent se2 = new SeperationEvent(timeStamp, tracks, true, fakeConsoleOutput, fakeFileOutput);
            SeperationEvent se3 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);

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
            SeperationEvent se1 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);
            SeperationEvent se2 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);
            SeperationEvent se3 = new SeperationEvent(timeStamp, tracks, true, fakeConsoleOutput, fakeFileOutput);
            SeperationEvent se4 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);
            SeperationEvent se5 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);

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

        #region AddEvents
        [Test]
        public void AddTrackEnteredEvent_Adding1TrackEntered_1TrackEnteredAdded()
        {
            //List is empty before adding
            Assert.That(uut.events.Count.Equals(0));


            uut.AddTrackEnteredEventFor(td1,fakeFileOutput);

            //List has one event after adding
            Assert.That(uut.events.Count.Equals(1));
            //Event is of type TrackEnteredEvent
            Assert.That(uut.events[0] is TrackEnteredEvent);
        }

        [Test]
        public void AddTrackLeftEvent_Adding1TrackLeft_1TrackLeftAdded()
        {
            //List is empty before adding
            Assert.That(uut.events.Count.Equals(0));


            uut.AddTrackLeftEventFor(td1, fakeFileOutput);

            //List has one event after adding
            Assert.That(uut.events.Count.Equals(1));
            //Event is of type TrackEnteredEvent
            Assert.That(uut.events[0] is TrackLeftEvent);
        }

        [Test]
        public void AddSeperationEvent_Adding1SeperationEvent_1SeperationEventAdded()
        {
            //List is empty before adding
            Assert.That(uut.events.Count.Equals(0));


            uut.AddSeperationEventFor(td1, td2, fakeFileOutput);

            //List has one event after adding
            Assert.That(uut.events.Count.Equals(1));
            //Event is of type TrackEnteredEvent
            Assert.That(uut.events[0] is SeperationEvent);
        }

        [Test]
        public void AddEvents_AddOneOfEachTypeEvent_CountIs3TypesAreCorrect()
        {
            //List is empty before adding
            Assert.That(uut.events.Count.Equals(0));

            uut.AddTrackEnteredEventFor(td1, fakeFileOutput);
            uut.AddTrackLeftEventFor(td1, fakeFileOutput);
            uut.AddSeperationEventFor(td1, td2, fakeFileOutput);

            //List has one event after adding
            Assert.That(uut.events.Count.Equals(3));
            //Event is of type TrackEnteredEvent
            Assert.That(uut.events[0] is TrackEnteredEvent);
            Assert.That(uut.events[1] is TrackLeftEvent);
            Assert.That(uut.events[2] is SeperationEvent);
        }
        #endregion

        #region RenderEvents
        [Test]
        public void RenderEvents_OneEventInList_PrintCalledOnce()
        {
            //Test is done with seperation events, since the _isRaised-attribute of 
            //TrackEnteredEvent and TrackLeftEvent goes false after a set amount of time, introducing possible errors
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se1 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);
            
            uut.events.Add(se1);

            string expectedString = se1.FormatData();

            //Console output did not receive anything before rendering
            fakeConsoleOutput.DidNotReceiveWithAnyArgs().Print("Any string");

            uut.RenderEvents();
            
            //Check that Print was called only once
            fakeConsoleOutput.ReceivedWithAnyArgs().Print("Any string");
        }

        [Test]
        public void RenderEvents_SeperationEventInList_CorrectStringPrinted()
        {
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se1 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);

            uut.events.Add(se1);

            string expectedString = se1.FormatData();

            //Console output did not receive anything before rendering
            fakeConsoleOutput.DidNotReceiveWithAnyArgs().Print("Any string");

            uut.RenderEvents();

            //Console output received the expected string
            fakeConsoleOutput.Received().Print(Arg.Is<string>(str => str.Contains(expectedString)));
        }

        [Test]
        public void RenderEvents_TrackEnteredEventInList_CorrectStringPrinted()
        {
            TrackEnteredEvent se1 = new TrackEnteredEvent(timeStamp, td1, false, fakeConsoleOutput, fakeFileOutput);

            uut.events.Add(se1);

            string expectedString = se1.FormatData();

            //Console output did not receive anything before rendering
            fakeConsoleOutput.DidNotReceiveWithAnyArgs().Print("Any string");

            uut.RenderEvents();

            //Console output received the expected string
            fakeConsoleOutput.Received().Print(Arg.Is<string>(str => str.Contains(expectedString)));
        }

        [Test]
        public void RenderEvents_TrackLeftEventInList_CorrectStringPrinted()
        {
            TrackLeftEvent se1 = new TrackLeftEvent(timeStamp, td1, false, fakeConsoleOutput, fakeFileOutput);

            uut.events.Add(se1);

            string expectedString = se1.FormatData();

            //Console output did not receive anything before rendering
            fakeConsoleOutput.DidNotReceiveWithAnyArgs().Print("Any string");

            uut.RenderEvents();

            //Console output received the expected string
            fakeConsoleOutput.Received().Print(Arg.Is<string>(str => str.Contains(expectedString)));
        }

        [Test]
        public void RenderEvents_ThreeEventsDifferentTypesInList_PrintCalledThreeTimes()
        {
            //Test is done with seperation events, since the _isRaised-attribute of 
            //TrackEnteredEvent and TrackLeftEvent goes false after a set amount of time, introducing possible errors
            List<TrackData> tracks = new List<TrackData>();
            tracks.Add(td1);
            tracks.Add(td2);
            SeperationEvent se1 = new SeperationEvent(timeStamp, tracks, false, fakeConsoleOutput, fakeFileOutput);
            TrackEnteredEvent tee1 = new TrackEnteredEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
            TrackLeftEvent tle1 = new TrackLeftEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
            uut.events.Add(se1);
            uut.events.Add(tee1);
            uut.events.Add(tle1);

            //Console output did not receive anything before rendering
            fakeConsoleOutput.DidNotReceiveWithAnyArgs().Print("Any string");

            uut.RenderEvents();

            //Check that Print was called only once
            fakeConsoleOutput.ReceivedWithAnyArgs(3).Print("Any string");
        }
        #endregion
    }
}
