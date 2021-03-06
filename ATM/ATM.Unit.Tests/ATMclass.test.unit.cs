﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using ATM.Logger;
using ATM.Render;
using NSubstitute;
using NUnit.Framework;
using TransponderReceiver;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class ATMclass_test_unit
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
        string timestamp;

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
            timestamp = "235928121999";

            uut = new ATMclass(consoleOutput, fileOutput, fakeAirspace, transponderReceiver);
        }

        #region ATMclass
        [Test]
        public void ATMclass_NothingCalled_IAirspaceCheckIfInMonitoredAreaIsCalledIs0()
        {
            //Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(0));
            fakeAirspace.DidNotReceiveWithAnyArgs().CheckIfInMonitoredArea(1, 1, 1);
        }

        [Test]
        public void ATMclass_HandleNewTrackDataCalledWithNoCurrentTracks_IAirspaceCheckIfInMonitoredAreaIsCalledIs1()
        {
            TrackData trackData = new TrackData("ABC", 100, 200, 300, "180320180854", 100, 100, consoleOutput);
            uut.HandleNewTrackData(trackData);

            //Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(1));
            fakeAirspace.ReceivedWithAnyArgs(1).CheckIfInMonitoredArea(1, 1, 1);
        }

        [Test]
        public void ATMclass_HandleNewTrackDataCalledTwiceWithSameTag_IAirspaceCheckIfInMonitoredAreaIsCalledIs2()
        {
            TrackData trackData1 = new TrackData("ABC", 100, 200, 300, "20181108102045200", 100, 100, consoleOutput);
            TrackData trackData2 = new TrackData("ABC", 200, 300, 400, "20181108102050100", 200, 200, consoleOutput);

            uut.HandleNewTrackData(trackData1);
            uut.HandleNewTrackData(trackData2);

            //Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(2));
            fakeAirspace.ReceivedWithAnyArgs(2).CheckIfInMonitoredArea(1, 1, 1);
        }

        [Test]
        public void ATMclass_HandleNewTrackDataCalledTwiceWithDifferentTags_IAirspaceCheckIfInMonitoredAreaIsCalledIs2()
        {
            TrackData trackData1 = new TrackData("ABC", 100, 200, 300, "180320180854", 100, 100, consoleOutput);
            TrackData trackData2 = new TrackData("DEF", 200, 300, 400, "180320180954", 200, 200, consoleOutput);

            uut.HandleNewTrackData(trackData1);
            uut.HandleNewTrackData(trackData2);

            //Assert.That(fakeAirspace.CheckIfInMonitoredArea_timesCalled.Equals(2));
            fakeAirspace.ReceivedWithAnyArgs(2).CheckIfInMonitoredArea(1, 1, 1);
        }

        [Test]
        public void ATMclass_HandleNewTrackDataTrackDataInRange_CurrentTracksCountIs1()
        {
            //Track data with coordinates inside airspace.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "180320180954", 200, 200, consoleOutput);

            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            uut.HandleNewTrackData(trackData1);

            Assert.That(uut.CurrentTracks.Count.Equals(1));
        }

        [Test]
        public void ATMclass_HandleNewTrackDataNewTrackDataComesOutOfRange_CurrentTracksCountIs0()
        {
            //We need uut with a REAL airspace, not a FAKE for this test.
            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);
            //Track data with coordinates inside airspace.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "20181108102045200", 200, 200, consoleOutput);

            //Track data with same tag, butf coordinates outside airspace.
            TrackData trackData2 = new TrackData("ABC", xMin - 1, yMin - 1, zMin - 1, "20181108102045200", 200, 200, consoleOutput);

            uut.HandleNewTrackData(trackData1);
            uut.HandleNewTrackData(trackData2);

            Assert.That(uut.CurrentTracks.Count.Equals(0));
        }

        [Test]
        public void ATMclass_TrackDatasAreInvolvedInSeperationEvent_IsInvolvedInSeperationEventReturnsTrue()
        {

            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            //Create 2 trackDatas that are in seperation event.
            TrackData trackData1 = new TrackData("ABC", 10001, 10001, 10001, "180320180954", 200, 200, consoleOutput);
            TrackData trackData2 = new TrackData("DEF", 10002, 10002, 10002, "180320180954", 200, 200, consoleOutput);

            List<TrackData> trackDatas = new List<TrackData>()
            {
                trackData1,
                trackData2
            };

            //Create seperation event from the two trackDatas and add to current seperation events.
            //SeperationEvent seperationEvent = new SeperationEvent(trackData1._TimeStamp, trackDatas, true, consoleOutput, fileOutput);
            //uut._currentEvents.Add(seperationEvent);
            uut.CurrentEvents.AddSeperationEventFor(trackData1, trackData2, fileOutput);

            Assert.That(() => uut.CurrentEvents.CheckIfSeperationEventExistsFor(trackData1, trackData2).Equals(true));
        }

        [Test]
        public void ATMclass_TrackDatasAreNotInvolvedInSeperationEvent_IsInvolvedInSeperationEventReturnsFalse()
        {
            //Create 2 trackDatas that are in seperation event.
            TrackData trackData1 = new TrackData("ABC", xMin + 1, yMin + 1, zMin + 1, "180320180954", 200, 200, consoleOutput);
            TrackData trackData2 = new TrackData("DEF", xMin + 2, yMin + 2, zMin + 2, "180320180954", 200, 200, consoleOutput);

            //No current seperation events.

            Assert.That(() => uut.CurrentEvents.CheckIfSeperationEventExistsFor(trackData1, trackData2).Equals(false));
        }
        #endregion

        #region AddTrack
        [Test]
        public void AddTrack_NoTracksAdded_CountIs0()
        {
            Assert.That(() => uut.CurrentTracks.Count.Equals(0));
        }

        [Test]
        public void AddTrack_TrackAdded_CountIs1()
        {
            uut.AddTrack(new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));
            Assert.That(() => uut.CurrentTracks.Count.Equals(1));
        }

        [Test]
        public void AddTrack_10TracksAdded_CountIs10()
        {
            for (int i = 0; i < 10; i++)
            {
                uut.AddTrack(new TrackData("ABC" + i, 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));
            }

            Assert.That(() => uut.CurrentTracks.Count.Equals(10));
        }

        [Test]
        public void AddTrack_TrackAdded_TagInFirstListObjectMatchesTagOfAddedTrack()
        {
            TrackData testTrack = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput);
            uut.AddTrack(testTrack);
            Assert.That(() => uut.CurrentTracks[0].Tag.Equals(testTrack.Tag));
        }

        [Test]
        public void AddTrack_AddTrackThenAddTrackWithSameTag_CountIs1()
        {
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 20000, 10000, 1000, timestamp, 100, 10, consoleOutput);
            uut.AddTrack(testTrack1);
            uut.AddTrack(testTrack2);
            Assert.That(() => uut.CurrentTracks.Count.Equals(1));
        }

        [Test]
        public void AddTrack_AddTrackThenAddTrackWithSameTag_XPositionOfObjectInListMatchesXPositionOfLastAddedTrack()
        {
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 20000, 10000, 1000, timestamp, 100, 10, consoleOutput);
            uut.AddTrack(testTrack1);
            uut.AddTrack(testTrack2);
            Assert.That(() => uut.CurrentTracks[0].CurrentXcord.Equals(testTrack2.CurrentXcord));
        }
        #endregion

        #region RemoveTrack
        [Test]
        public void RemoveTrack_Add3TracksRemove1TrackWIthValidTag_CountIs2()
        {
            uut.AddTrack(new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));
            uut.AddTrack(new TrackData("DEF", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));
            uut.AddTrack(new TrackData("GHI", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));

            uut.RemoveTrack("ABC");

            Assert.That(() => uut.CurrentTracks.Count.Equals(2));
        }

        [Test]
        public void RemoveTrack_Add3TracksRemove1TrackWIthInvalidTag_ThrowsArgumentOutOfRangeException()
        {
            uut.AddTrack(new TrackData("ABC", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));
            uut.AddTrack(new TrackData("DEF", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));
            uut.AddTrack(new TrackData("GHI", 10000, 10000, 1000, timestamp, 100, 10, consoleOutput));

            Assert.That(() => uut.RemoveTrack("XYZ"), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void RemoveTrack_RemoveTrackFromEmptyList_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => uut.RemoveTrack("XYZ"), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }
        #endregion

        #region CheckForSeperationEventConditions
        [Test]
        public void CheckForSeperationEventConditions_TagsForTheTwoTracksAreTheSame_ThrowsException()
        {
            TrackData track1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 150, 50, consoleOutput);

            string message = "Provided TrackDatas have the same Tag";

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track1), Throws.Exception.TypeOf<Exception>().With.Message.EqualTo(message));
        }

        [Test]
        public void CheckForSeperationEventConditions_NoConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEventConditions_AllConditionsMet_ReturnsTrue()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 30001, 30001, 1001, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(true));
        }

        [Test]
        public void CheckForSeperationEventConditions_OnlyXConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000 - 1, 10000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEventConditions_OnlyYConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 50000 - 1, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEventConditions_OnlyZConditionMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 10000, 5000 - 1, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEventConditions_XandYConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000 - 1, 50000 - 1, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(false));
        }

        [Test]
        public void CheckForSeperationEventConditions_YandZConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 10000, 50000 - 1, 5000 - 1, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(false));
        }


        [Test]
        public void CheckForSeperationEventConditions_XandZConditionsMet_ReturnsFalse()
        {
            TrackData track1 = new TrackData("ABC", 50000 - 1, 10000, 5000 - 1, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            Assert.That(() => uut.CheckForSeperationEventConditions(track1, track2).Equals(false));
        }
        #endregion

        #region CheckForSeperationEvent
        [Test]
        public void CheckForSeperationEvent_SeperationConditionsMetAndNoExistingEvent_EventIsCreated()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 30001, 30001, 1001, timestamp, 150, 50, consoleOutput);

            uut.AddTrack(track1); //track2 and this track will meet conditions for the seperation event

            //List of events is empty before checking
            Assert.That(uut.CurrentEvents.events.Count.Equals(0));

            uut.CheckForSeperationEvents(track2);

            //Seperation event has been added
            Assert.That(uut.CurrentEvents.events.Count.Equals(1));
        }

        [Test]
        public void CheckForSeperationEvent_SeperationConditionsMetAndAlreadyExistingEvent_NoNewEventIsCreated()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 30001, 30001, 1001, timestamp, 150, 50, consoleOutput);

            uut.AddTrack(track1); //track2 and this track will meet conditions for the seperation event

            //Add seperation event involving the 2 tracks already
            uut.CurrentEvents.AddSeperationEventFor(track1, track2, fileOutput);
            
            //List of events has 1 event before checking
            Assert.That(uut.CurrentEvents.events.Count.Equals(1));

            uut.CheckForSeperationEvents(track2);

            //List of events still only has 1 event
            Assert.That(uut.CurrentEvents.events.Count.Equals(1));
        }

        #region CheckIfSeperationEventExists
        [Test]
        public void CheckIfSeperationEventExistsBetween_TrackDatasInSameOrderAsForSeperationEvent_returnsTrue()
        {
            List<TrackData> trackDatas = new List<TrackData>()
            {
                new TrackData("ABC",1,2,3,"time",5,6, consoleOutput),
                new TrackData("DEF",1,2,3,"time",5,6, consoleOutput)
            };
            //SeperationEvent seperationEvent1 = new SeperationEvent("time", trackDatas, true, uut._outputConsole, uut._outputFile);
            //uut._currentEvents.Add(seperationEvent1);
            uut.CurrentEvents.AddSeperationEventFor(trackDatas[0], trackDatas[1], fileOutput);

            Assert.That(() => uut.CurrentEvents.CheckIfSeperationEventExistsFor(trackDatas[0], trackDatas[1]).Equals(true));
        }

        [Test]
        public void CheckIfSeperationEventExistsBetween_TrackDatasInDifferentOrderAsForSeperationEvent_returnsTrue()
        {
            List<TrackData> trackDatas = new List<TrackData>()
            {
                new TrackData("ABC",1,2,3,"time",5,6, consoleOutput),
                new TrackData("DEF",1,2,3,"time",5,6, consoleOutput)
            };
            //SeperationEvent seperationEvent1 = new SeperationEvent("time", trackDatas, true, uut._outputConsole, uut._outputFile);
            //uut._currentEvents.Add(seperationEvent1);
            uut.CurrentEvents.AddSeperationEventFor(trackDatas[0], trackDatas[1], fileOutput);

            Assert.That(() => uut.CurrentEvents.CheckIfSeperationEventExistsFor(trackDatas[1], trackDatas[0]).Equals(true));
        }
        #endregion

        #region CalculateSpeed

        [Test]
        public void CalculateSpeed_TrackMoved1Xin1Sec_Returns1()
        {
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10001, 10000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackSpeed(testTrack2, testTrack1).Equals(1));
        }

        [Test]
        public void CalculateSpeed_TrackMovedMinus1Xin1Sec_Returns1()
        {
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 9999, 10000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackSpeed(testTrack2, testTrack1).Equals(1));
        }

        [Test]
        public void CalculateSpeed_TrackMoved1Yin1Sec_Returns1()
        {
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10001, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackSpeed(testTrack2, testTrack1).Equals(1));
        }

        [Test]
        public void CalculateSpeed_TrackMovedMinus1Yin1Sec_Returns1()
        {
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 9999, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackSpeed(testTrack2, testTrack1).Equals(1));
        }

        [Test]
        public void CalculateSpeed_TrackMoved1Zin1Sec_Returns0()
        {
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1001, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackSpeed(testTrack2, testTrack1).Equals(0));
        }

        [Test]
        public void CalculateSpeed_TrackMovedMinus1Zin1Sec_Returns0()
        {
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 999, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackSpeed(testTrack2, testTrack1).Equals(0));
        }

        [Test]
        public void CalculateSpeed_TrackMoved1X1Yin1Sec_Returns1p414()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10001, 10001, 1000, timestampNew, 0, 0, consoleOutput);

           

            double result = Math.Sqrt(Math.Pow(1, 2) + Math.Pow(1, 2));

            Assert.That(uut.CalculateTrackSpeed(testTrack2,testTrack1).Equals(result));
        }

        [Test]
        public void CalculateSpeed_MismatchingTags_ThrowsError()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("XYZ", 10001, 10001, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.Throws<ArgumentException>(
                () => uut.CalculateTrackSpeed(testTrack2, testTrack1), "Tags for the supplied TrackData-objects do not match.");
        }

        [Test]
        public void CalculateSpeed_NewTimestampIsOlderThanOldTimestamp_ThrowsError()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10001, 10001, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.Throws<ArgumentException>(
                () => uut.CalculateTrackSpeed(testTrack1, testTrack2), "Timestamp of new data is older than Timestamp of old data");
        }
        #endregion

        #region CalculateTrackCourse

        [Test]
        public void CalculateTrackCourse_MoveOnlyInPositiveX_Returns0()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 20000, 10000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2,testTrack1).Equals(0));
        }

        [Test]
        public void CalculateTrackCourse_MoveOnlyInNegativeX_Returns180()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 20000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2, testTrack1).Equals(180));
        }

        [Test]
        public void CalculateTrackCourse_MoveOnlyInPositiveY_Returns90()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 20000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2, testTrack1).Equals(90));
        }

        [Test]
        public void CalculateTrackCourse_MoveOnlyInNegativeY_ReturnsMinus90()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 20000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2, testTrack1).Equals(-90));
        }

        [Test]
        public void CalculateTrackCourse_MoveEqualInPositiveXPositiveY_Returns45()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 20000, 20000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2, testTrack1).Equals(45));
        }

        [Test]
        public void CalculateTrackCourse_MoveEqualInPositiveXNegativeY_ReturnsMinus45()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 20000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 20000, 10000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2, testTrack1).Equals(-45));
        }

        [Test]
        public void CalculateTrackCourse_MoveEqualInNegativeXNegativeY_ReturnsMinus135()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 20000, 20000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2, testTrack1).Equals(-135));
        }

        [Test]
        public void CalculateTrackCourse_MoveEqualInNegativeXPositiveY_Returns135()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 20000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 20000, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.That(uut.CalculateTrackCourse(testTrack2, testTrack1).Equals(135));
        }

        [Test]
        public void CalculateTrackCourse_MismatchingTags_ThrowsError()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("XYZ", 10001, 10001, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.Throws<ArgumentException>(
                () => uut.CalculateTrackCourse(testTrack2, testTrack1), "Tags for the supplied TrackData-objects do not match.");
        }

        [Test]
        public void CalculateTrackCourse_NewTimestampIsOlderThanOldTimestamp_ThrowsError()
        {
            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10001, 10001, 1000, timestampNew, 0, 0, consoleOutput);

            Assert.Throws<ArgumentException>(
                () => uut.CalculateTrackCourse(testTrack1, testTrack2), "Timestamp of new data is older than Timestamp of old data");
        }

        #endregion

        #region HandleNewTrackData
        [Test]
        public void HandleNewTrackData_TrackJustAdded_SpeedIs0()
        {
            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestamp, 0, 10, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(0));
        }

        [Test]
        public void HandleNewTrackData_TrackMoved1Xin1Sec_SpeedIsUpdatedTo1()
        {
            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10001, 10000, 1000, timestampNew, 0, 0, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            uut.HandleNewTrackData(testTrack2);

            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(1));
        }

        [Test]
        public void HandleNewTrackData_TrackMovedMinus1Xin1Sec_SpeedIsUpdatedTo1()
        {

            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10001, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1000, timestampNew, 0, 0, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            uut.HandleNewTrackData(testTrack2);

            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(1));
        }

        [Test]
        public void HandleNewTrackData_TrackMoved1Yin1Sec_SpeedIsUpdatedTo1()
        {
            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10001, 1000, timestampNew, 0, 0, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            uut.HandleNewTrackData(testTrack2);

            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(1));
        }

        [Test]
        public void HandleNewTrackData_TrackMovedMinus1Yin1Sec_SpeedIsUpdatedTo1()
        {

            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10001, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1000, timestampNew, 0, 0, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            uut.HandleNewTrackData(testTrack2);

            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(1));
        }

        [Test]
        public void HandleNewTrackData_TrackMoved1Zin1Sec_SpeedIsUpdatedTo0()
        {

            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1001, timestampNew, 0, 0, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            uut.HandleNewTrackData(testTrack2);

            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(0));
        }

        [Test]
        public void HandleNewTrackData_TrackMovedMinus1Zin1Sec_SpeedIsUpdatedTo0()
        {
            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1001, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10000, 10000, 1000, timestampNew, 0, 0, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            uut.HandleNewTrackData(testTrack2);

            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(0));
        }

        [Test]
        public void HandleNewTrackData_TrackMoved1X1Yin1Sec_SpeedIsUpdatedTo1p414()
        {

            uut = new ATMclass(consoleOutput, fileOutput, airspace, transponderReceiver);

            //sqrt(3^2+3^2)=3
            string timestampOld = "20181109103800000";
            string timestampNew = "20181109103801000"; //Timestamp 1 second later
            TrackData testTrack1 = new TrackData("ABC", 10000, 10000, 1000, timestampOld, 0, 0, consoleOutput);
            TrackData testTrack2 = new TrackData("ABC", 10001, 10001, 1000, timestampNew, 0, 0, consoleOutput);
            uut.HandleNewTrackData(testTrack1);
            uut.HandleNewTrackData(testTrack2);

            double result = Math.Sqrt(Math.Pow(1, 2) + Math.Pow(1, 2));

            Assert.That(uut.CurrentTracks[0].CurrentHorzVel.Equals(result));
        }
        #endregion
        #endregion

        #region UpdateSeperationEventStatus
        [Test]
        public void UpdateSeperationEventStatus_NewEventCreated_isRaisedIsTrue()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            //Add tracks in order to have some data to update from
            uut.AddTrack(track1);
            uut.AddTrack(track2);

            //Add seperation event involving the 2 tracks already
            uut.CurrentEvents.AddSeperationEventFor(track1, track2, fileOutput);

            //Check that status is true before update
            Assert.That(uut.CurrentEvents.events[0].isRaised.Equals(true));
        }

        [Test]
        public void UpdateSeperationEventStatus_SeperationEventConditionsStillMet_isRaisedIsStillTrue()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 30001, 30001, 1001, timestamp, 150, 50, consoleOutput);

            //Add tracks in order to have some data to update from
            uut.AddTrack(track1);
            uut.AddTrack(track2);

            //Add seperation event involving the 2 tracks already
            uut.CurrentEvents.AddSeperationEventFor(track1, track2, fileOutput);

            uut.UpdateSeperationEventStatus();

            //Check that status is true after update
            Assert.That(uut.CurrentEvents.events[0].isRaised.Equals(true));
        }

        [Test]
        public void UpdateSeperationEventStatus_SeperationEventConditionsNoLongerMet_isRaisedIsFalse()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            //Add tracks in order to have some data to update from
            uut.AddTrack(track1);
            uut.AddTrack(track2);

            //Add seperation event involving the 2 tracks already
            uut.CurrentEvents.AddSeperationEventFor(track1, track2, fileOutput);

            uut.UpdateSeperationEventStatus();

            //Check that status is true after update
            Assert.That(uut.CurrentEvents.events[0].isRaised.Equals(false));
        }

        [Test]
        public void UpdateSeperationEventStatus_OneTrackLeftAirspace_isRaisedIsFalse()
        {
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            //Add tracks in order to have some data to update from
            uut.AddTrack(track1);
            uut.AddTrack(track2);

            //Add seperation event involving the 2 tracks already
            uut.CurrentEvents.AddSeperationEventFor(track1, track2, fileOutput);

            //Simulate that one track has left airspace
            uut.RemoveTrack(track2.Tag);

            uut.UpdateSeperationEventStatus();

            //Check that status is true after update
            Assert.That(uut.CurrentEvents.events[0].isRaised.Equals(false));
        }

        [Test]
        public void UpdateSeperationEventStatus_SeveralEventsUpdate_isRaisedIsUpdatedCorrectly()
        {
            //Tracks that will meet seperation event conditions upon check
            TrackData track1 = new TrackData("ABC", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track2 = new TrackData("DEF", 30001, 30001, 1001, timestamp, 150, 50, consoleOutput);

            //Tracks that will not meet seperation event conditions upon check
            TrackData track3 = new TrackData("GHI", 30000, 30000, 1000, timestamp, 150, 50, consoleOutput);
            TrackData track4 = new TrackData("JKL", 50000, 50000, 5000, timestamp, 150, 50, consoleOutput);

            //Add tracks in order to have some data to update from
            uut.AddTrack(track1);
            uut.AddTrack(track2);
            uut.AddTrack(track3);
            uut.AddTrack(track4);

            //Add seperation event involving the 2 tracks already
            uut.CurrentEvents.AddSeperationEventFor(track1, track2, fileOutput);
            uut.CurrentEvents.AddSeperationEventFor(track3, track4, fileOutput);

            //Update status
            uut.UpdateSeperationEventStatus();

            //Check that status is true after update
            Assert.That(uut.CurrentEvents.events[0].isRaised.Equals(true));
            Assert.That(uut.CurrentEvents.events[1].isRaised.Equals(false));
        }
        #endregion
    }
}