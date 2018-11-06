using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    public class ATM_Unit_Tests
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

        #region Logging

        #region ActiveSeparationEvent logging

        [Test]
        public void active_logging_nothingCalled_MethodHasNotBeenCalled()
        {
            Assert.That(logger.LogActiveSeparationEvent_timesCalled.Equals(0));
        }

        [Test]
        public void active_logging_logActiveSeparationEvent_MethodHasBeenCalled()
        {
            TrackData trackData1 = new TrackData("ABC", 10000, 20000, 3000, timestamp, 100, 10);
            TrackData trackData2 = new TrackData("DEF", 10000, 20000, 3000, timestamp, 100, 10);
          
            uut.CheckForSeperationEvent(trackData1,trackData2);
            Assert.That(logger.LogActiveSeparationEvent_timesCalled.Equals(1));
        }

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
            Assert.That(logger.ParametersList[0]._OccurrenceTime.Equals(seperationEvent._OccurrenceTime));
        }

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
            Assert.That(logger.ParametersList[0]._IsRaised.Equals(seperationEvent._IsRaised));
        }

        #endregion

        //Inactive mangler at blive lavet
        #region InActiveSeparationEvent logging

        [Test]
        public void logging_nothingCalled_LogInactiveSeperationEventMethodHasNotBeenCalled()
        {
            Assert.That(logger.LogInactiveSeparationEvent_timesCalled.Equals(0));
        }

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
            Assert.That(logger.ParametersList[0]._OccurrenceTime.Equals(seperationEvent._OccurrenceTime));
        }

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
            Assert.That(logger.ParametersList[0]._IsRaised.Equals(seperationEvent._IsRaised));
        }

        #endregion

        #endregion

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

        #region Airspace
        [Test]
        public void airspace_coordinateInAirspace_returnsTrue()
        {
            Assert.That(()=>airspace.CheckIfInMonitoredArea(50000, 50000, 1000).Equals(true));
        }

        #region CoordinatesTooLow
        [Test]
        public void airspace_xCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMin-1, 50000, 1000).Equals(false));
        }

        [Test]
        public void airspace_yCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMin-1, 1000).Equals(false));
        }

        [Test]
        public void airspace_zCoordinateTooLow_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMin-1).Equals(false));
        }
        #endregion

        #region CoordinatesTooHigh
        [Test]
        public void airspace_xCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMax + 1, 50000, 1000).Equals(false));
        }

        [Test]
        public void airspace_yCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMax + 1, 1000).Equals(false));
        }

        [Test]
        public void airspace_zCoordinateTooHigh_returnsFalse()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMax + 1).Equals(false));
        }
        #endregion

        #region CoordinatesLowerBoundary
        [Test]
        public void airspace_xCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMin, 50000, 1000).Equals(true));
        }

        [Test]
        public void airspace_yCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMin, 1000).Equals(true));
        }

        [Test]
        public void airspace_zCoordinateLowerBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMin).Equals(true));
        }
        #endregion

        #region CoordinatesUpperBoundary
        [Test]
        public void airspace_xCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(xMax, 50000, 1000).Equals(true));
        }

        [Test]
        public void airspace_yCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, yMax, 1000).Equals(true));
        }

        [Test]
        public void airspace_zCoordinateUpperBoundary_returnsTrue()
        {
            Assert.That(() => airspace.CheckIfInMonitoredArea(50000, 50000, zMax).Equals(true));
        }
        #endregion
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
            for (int i=0; i<10;i++)
            {
                uut.AddTrack(new TrackData("ABC"+i, 10000, 10000, 1000, timestamp, 100, 10));
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

            Assert.That(() => uut.RemoveTrack("XYZ"),Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
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

            Assert.That(() => uut.CheckForSeperationEvent(track1,track1), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo(message));
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
            TrackData track1 = new TrackData("ABC", 50000-1, 10000, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_OnlyYConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 50000-1, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_OnlyZConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 10000, 5000-1, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_XandYConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000-1, 50000-1, 1000, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEvent_YandZConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 50000 - 1, 5000-1, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }


        [Test]
        public void CheckForSeperationEvent_XandZConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000-1, 10000, 5000 - 1, timestamp, 150, 50);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp,150, 50);

            Assert.That(() => uut.CheckForSeperationEvent(track1, track2).Equals(false));
        }
        #endregion

        #region CheckIfSeperationEventExistsBetween
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
            
            Assert.That(() => uut.CheckIfSeperationEventExistsFor(trackDatas[0],trackDatas[1]).Equals(true));
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
            TrackData trackData1 = new TrackData("ABC", 100, 200, 300, "180320180854", 100, 100);
            TrackData trackData2 = new TrackData("ABC", 200, 300, 400, "180320180954", 200, 200);

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
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "180320180954", 200, 200);

            //Track data with same tag, butf coordinates outside airspace.
            TrackData trackData2 = new TrackData("ABC", xMin-1, yMin-1, zMin-1, "180320180954", 200, 200);

            uut.HandleNewTrackData(trackData1);
            uut.HandleNewTrackData(trackData2);

            Assert.That(uut._currentTracks.Count.Equals(0));
        }

        [Test]
        public void ATMclass_TrackDatasAreInvolvedInSeperationEvent_IsInvolvedInSeperationEventReturnsTrue()
        {
            //Create 2 trackDatas that are in seperation event.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "180320180954", 200, 200);
            TrackData trackData2 = new TrackData("DEF", xMin +2, yMin +2, zMin +2, "180320180954", 200, 200);

            List<TrackData> trackDatas = new List<TrackData>()
            {
                trackData1,
                trackData2
            };

            //Create seperation event from the two trackDatas and add to current seperation events.
            SeperationEvent seperationEvent = new SeperationEvent(trackData1._TimeStamp, trackDatas, true);
            uut._currentSeperationEvents.Add(seperationEvent);

            Assert.That( () => uut.CheckIfSeperationEventExistsFor(trackData1, trackData2).Equals(true));
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

        #region TransponderReceiver
        [Test]
        public void TransponderReceiver_AttachATM_observersCountIs1()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver);

            system.Attach(uut);

            Assert.That(() => system.getObserverCount().Equals(1));
        }

        [Test]
        public void TransponderReceiver_AttachATMthenDetach_observersCountIs0()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver);

            system.Attach(uut);
            system.Detach(uut);

            Assert.That(() => system.getObserverCount().Equals(0));
        }

        [Test]
        public void TransponderReceiver_DetachATMwithoutAttachingIt_observersCountIs0()
        {
            var receiver = TransponderReceiverFactory.CreateTransponderDataReceiver();
            var system = new ATM.TransponderReceiver(receiver);

            system.Detach(uut);

            Assert.That(() => system.getObserverCount().Equals(0));
        }

        [Test]
        public void TransponderReceiver_3NewTracksWithDifferentTags_TrackCountIs3()
        {
            //Test inspired by "TransponderReceiverUser" by FRABJ.
            // Make a fake Transponder Data Receiver
            var _fakeTransponderReceiver = Substitute.For<ITransponderReceiver>();
            // Inject the fake TDR
            var transponderReceiver = new TransponderReceiver(_fakeTransponderReceiver);

            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(logger, renderer, airspace);

            // Setup test data
            List<string> testData = new List<string>();
            testData.Add("ATR423;39045;12932;14000;20151006213456789");
            testData.Add("BCD123;10005;85890;12000;20151006213456789");
            testData.Add("XYZ987;25059;75654;4000;20151006213456789");
            
            //Attach uut to receiver
            transponderReceiver.Attach(uut);

            // Act: Trigger the fake object to execute event invocation
            _fakeTransponderReceiver.TransponderDataReady
                += Raise.EventWith(this, new RawTransponderDataEventArgs(testData));

            Assert.That(uut._currentTracks.Count.Equals(3));
        }

        [Test]
        public void TransponderReceiver_3NewTracksWithSameTags_TrackCountIs1()
        {
            //Test inspired by "TransponderReceiverUser" by FRABJ.
            // Make a fake Transponder Data Receiver
            var _fakeTransponderReceiver = Substitute.For<ITransponderReceiver>();
            // Inject the fake TDR
            var transponderReceiver = new TransponderReceiver(_fakeTransponderReceiver);

            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(logger, renderer, airspace);

            // Setup test data
            List<string> testData = new List<string>();
            testData.Add("ATR423;39045;12932;14000;20151006213456789");
            testData.Add("ATR423;10005;85890;12000;20151006213456789");
            testData.Add("ATR423;25059;75654;4000;20151006213456789");

            //Attach uut to receiver
            transponderReceiver.Attach(uut);

            // Act: Trigger the fake object to execute event invocation
            _fakeTransponderReceiver.TransponderDataReady
                += Raise.EventWith(this, new RawTransponderDataEventArgs(testData));

            Assert.That(uut._currentTracks.Count.Equals(1));
        }
        #endregion
    }
}
