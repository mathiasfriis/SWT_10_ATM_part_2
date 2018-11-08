using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class ATMclass_test_unit
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

        #region ATMclass
        [Test]
        public void ATMclass_NothingCalled_IAirspaceCheckIfInMonitoredAreaIsCalledIs0()
        {
            Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(0));
        }

        [Test]
        public void ATMclass_HandleNewTrackDataCalledWithNoCurrentTracks_IAirspaceCheckIfInMonitoredAreaIsCalledIs1()
        {
            TrackData trackData = new TrackData("ABC", 100, 200, 300, "180320180854", 100, 100);
            uut.HandleNewTrackData(trackData);

            Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(1));
        }

        [Test]
        public void ATMclass_HandleNewTrackDataCalledTwiceWithSameTag_IAirspaceCheckIfInMonitoredAreaIsCalledIs2()
        {
            TrackData trackData1 = new TrackData("ABC", 100, 200, 300, "20181108102045200", 100, 100);
            TrackData trackData2 = new TrackData("ABC", 200, 300, 400, "20181108102050100", 200, 200);

            uut.HandleNewTrackData(trackData1);
            uut.HandleNewTrackData(trackData2);

            Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(2));
        }

        [Test]
        public void ATMclass_HandleNewTrackDataCalledTwiceWithDifferentTags_IAirspaceCheckIfInMonitoredAreaIsCalledIs2()
        {
            TrackData trackData1 = new TrackData("ABC", 100, 200, 300, "180320180854", 100, 100);
            TrackData trackData2 = new TrackData("DEF", 200, 300, 400, "180320180954", 200, 200);

            uut.HandleNewTrackData(trackData1);
            uut.HandleNewTrackData(trackData2);

            Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(2));
        }

        [Test]
        public void ATMclass_HandleNewTrackDataTrackDataInRange_CurrentTracksCountIs1()
        {
            //Track data with coordinates inside airspace.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "180320180954", 200, 200);

            uut.HandleNewTrackData(trackData1);

            Assert.That(uut._currentTracks.Count.Equals(1));
        }

        [Test]
        public void ATMclass_HandleNewTrackDataNewTrackDataComesOutOfRange_CurrentTracksCountIs0()
        {
            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(logger, renderer, airspace);
            //Track data with coordinates inside airspace.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "20181108102045200", 200, 200);

            //Track data with same tag, butf coordinates outside airspace.
            TrackData trackData2 = new TrackData("ABC", xMin - 1, yMin - 1, zMin - 1, "20181108102045200", 200, 200);

            uut.HandleNewTrackData(trackData1);
            uut.HandleNewTrackData(trackData2);

            Assert.That(uut._currentTracks.Count.Equals(0));
        }

        [Test]
        public void ATMclass_TrackDatasAreInvolvedInSeperationEvent_IsInvolvedInSeperationEventReturnsTrue()
        {
            //Create 2 trackDatas that are in seperation event.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "180320180954", 200, 200);
            TrackData trackData2 = new TrackData("DEF", xMin + 2, yMin + 2, zMin + 2, "180320180954", 200, 200);

            List<TrackData> trackDatas = new List<TrackData>()
            {
                trackData1,
                trackData2
            };

            //Create seperation event from the two trackDatas and add to current seperation events.
            SeperationEvent seperationEvent = new SeperationEvent(trackData1._TimeStamp, trackDatas, true);
            uut._currentSeperationEvents.Add(seperationEvent);

            Assert.That(() => uut.CheckIfSeperationEventExistsFor(trackData1, trackData2).Equals(true));
        }

        [Test]
        public void ATMclass_TrackDatasAreNotInvolvedInSeperationEvent_IsInvolvedInSeperationEventReturnsFalse()
        {
            //Create 2 trackDatas that are in seperation event.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "180320180954", 200, 200);
            TrackData trackData2 = new TrackData("DEF", xMin + 2, yMin + 2, zMin + 2, "180320180954", 200, 200);

            //No current seperation events.

            Assert.That(() => uut.CheckIfSeperationEventExistsFor(trackData1, trackData2).Equals(false));
        }
        #endregion

        #region AddTrack
        [Test]
        public void AddTrack_NoTracksAdded_CountIs0()
        {
            Assert.That(() => uut._currentTracks.Count.Equals(0));
        }

        [Test]
        public void AddTrack_TrackAdded_CountIs1()
        {
            uut.AddTrack(new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10));
            Assert.That(() => uut._currentTracks.Count.Equals(1));
        }

        [Test]
        public void AddTrack_10TracksAdded_CountIs10()
        {
            for (int i = 0; i < 10; i++)
            {
                uut.AddTrack(new TrackData("ABC" + i, 10000, 10000, 1000, timestamp, 100, 10));
            }

            Assert.That(() => uut._currentTracks.Count.Equals(10));
        }

        [Test]
        public void AddTrack_TrackAdded_TagInFirstListObjectMatchesTagOfAddedTrack()
        {
            TrackData testTrack = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            uut.AddTrack(testTrack);
            Assert.That(() => uut._currentTracks[0]._Tag.Equals(testTrack._Tag));
        }

        [Test]
        public void AddTrack_AddTrackThenAddTrackWithSameTag_CountIs1()
        {
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            TrackData testTrack2 = new TrackData("ABC", 20000, 10000, 1000, timestamp, 100, 10);
            uut.AddTrack(testTrack1);
            uut.AddTrack(testTrack2);
            Assert.That(() => uut._currentTracks.Count.Equals(1));
        }

        [Test]
        public void AddTrack_AddTrackThenAddTrackWithSameTag_XPositionOfObjectInListMatchesXPositionOfLastAddedTrack()
        {
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10);
            TrackData testTrack2 = new TrackData("ABC", 20000, 10000, 1000, timestamp, 100, 10);
            uut.AddTrack(testTrack1);
            uut.AddTrack(testTrack2);
            Assert.That(() => uut._currentTracks[0]._CurrentXcord.Equals(testTrack2._CurrentXcord));
        }
        #endregion

        #region RemoveTrack
        [Test]
        public void RemoveTrack_Add3TracksRemove1TrackWIthValidTag_CountIs2()
        {
            uut.AddTrack(new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10));
            uut.AddTrack(new TrackData("DEF", 10000, 10000, 1000, timestamp, 100, 10));
            uut.AddTrack(new TrackData("GHI", 10000, 10000, 1000, timestamp, 100, 10));

            uut.RemoveTrack("ABC");

            Assert.That(() => uut._currentTracks.Count.Equals(2));
        }

        [Test]
        public void RemoveTrack_Add3TracksRemove1TrackWIthInvalidTag_ThrowsArgumentOutOfRangeException()
        {
            uut.AddTrack(new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10));
            uut.AddTrack(new TrackData("DEF", 10000, 10000, 1000, timestamp, 100, 10));
            uut.AddTrack(new TrackData("GHI", 10000, 10000, 1000, timestamp, 100, 10));

            Assert.That(() => uut.RemoveTrack("XYZ"), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void RemoveTrack_RemoveTrackFromEmptyList_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => uut.RemoveTrack("XYZ"), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }
        #endregion

        #region CheckForSeperationEvent
        [Test]
        public void CheckForSeperationEvent_TagsForTheTwoTracksAreTheSame_ThrowsException()
        {
            TrackData track1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 150, 50);

            string message = "Provided TrackDatas have the same Tag";

            Assert.That(() => uut.CheckForSeperationEvent(track1, track1), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo(message));
        }

        [Test]
        public void CheckForSeperationEvent_NoConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_AllConditionsMet_ReturnsTrue()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 30001, 30001, 1001, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(true));
        }

        [Test]
        public void CheckForSeperationEvent_OnlyXConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000 - 1, 10000, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_OnlyYConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 50000 - 1, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_OnlyZConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 10000, 5000 - 1, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_XandYConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000 - 1, 50000 - 1, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_YandZConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 50000 - 1, 5000 - 1, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }


        [Test]
        public void CheckForSeperationEvent_XandZConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000 - 1, 10000, 5000 - 1, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }
        #endregion

        #region CheckIfSeperationEventExists
        [Test]
        public void CheckIfSeperationEventExistsBetween_TrackDatasInSameOrderAsForSeperationEvent_returnsTrue()
        {
            List<TrackData> trackDatas = new List<TrackData>()
            {
                new TrackData("ABC",1,2,3,"time",5,6),
                new TrackData("DEF",1,2,3,"time",5,6)
            };
            SeperationEvent seperationEvent1 = new SeperationEvent("time", trackDatas, true);
            uut._currentSeperationEvents.Add(seperationEvent1);

            Assert.That(() => uut.CheckIfSeperationEventExistsFor(trackDatas[0], trackDatas[1]).Equals(true));
        }

        [Test]
        public void CheckIfSeperationEventExistsBetween_TrackDatasInDifferentOrderAsForSeperationEvent_returnsTrue()
        {
            List<TrackData> trackDatas = new List<TrackData>()
            {
                new TrackData("ABC",1,2,3,"time",5,6),
                new TrackData("DEF",1,2,3,"time",5,6)
            };
            SeperationEvent seperationEvent1 = new SeperationEvent("time", trackDatas, true);
            uut._currentSeperationEvents.Add(seperationEvent1);

            Assert.That(() => uut.CheckIfSeperationEventExistsFor(trackDatas[1], trackDatas[0]).Equals(true));
        }
        #endregion
    }
}
