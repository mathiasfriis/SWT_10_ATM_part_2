using ATM.Events;
using ATM.Logger;
using ATM.Render;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class Events
    {
        private TrackData td1;
        private TrackData td2;
        private TrackData td3;
        private string timeStamp;
        private IFileOutput fakeFileOutput;
        private IConsoleOutput fakeConsoleOutput;
        SeperationEvent uut_se;
        TrackEnteredEvent uut_tee;
        TrackLeftEvent uut_tle;


        [SetUp]
        public void setup()
        {
            fakeFileOutput = Substitute.For<IFileOutput>();
            fakeConsoleOutput = Substitute.For<IConsoleOutput>();
            timeStamp = "235928121999";
            td1 = new TrackData("ABC123", 10000, 10000, 1000, timeStamp, 100, 45, fakeConsoleOutput);
            td2 = new TrackData("DEF123", 10000, 10000, 1000, timeStamp, 100, 45, fakeConsoleOutput);
            td3 = new TrackData("XYZ123", 10000, 10000, 1000, timeStamp, 100, 45, fakeConsoleOutput);

            List<TrackData> tracks = new List<TrackData>()
            {
                td1,
                td2
            };

            uut_se = new SeperationEvent(timeStamp, tracks, true, fakeConsoleOutput, fakeFileOutput);
            uut_tee = new TrackEnteredEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
            uut_tle = new TrackLeftEvent(timeStamp, td1, true, fakeConsoleOutput, fakeFileOutput);
        }

        #region TrackEnteredEvent
        [Test]
        public void TrackEnteredEventCheckIfStillValid_isRaisedIsTrue_ReturnsTrue()
        {
            uut_tee._isRaised = true;

            Assert.That(() => uut_tee.CheckIfStillValid().Equals(true));
        }

        [Test]
        public void TrackEnteredEventCheckIfStillValid_isRaisedIsFalse_ReturnsFalse()
        {
            uut_tee._isRaised = false;

            Assert.That(() => uut_tee.CheckIfStillValid().Equals(false));
        }

        [Test]
        public void TrackEnteredEventFormatData_isCalled_ReturnsCorrectString()
        {
            string expectedString = "Track entered airspace - Occurencetime: 235928121999 Involved track: ABC123";
            Assert.That(() => uut_tee.FormatData().Equals(expectedString));
        }

        [Test]
        public void TrackEnteredLog_isCalled_fileOutputReceivesCorrectString()
        {
            string expectedString = "Track entered airspace - Occurencetime: 235928121999 Involved track: ABC123";

            uut_tee.Log();

            fakeFileOutput.Received().Write(Arg.Is<string>(str => str.Contains(expectedString)));
        }

        [Test]
        public void TrackEnteredRender_isCalled_consoleOutputReceivesCorrectString()
        {
            string expectedString = "Track entered airspace - Occurencetime: 235928121999 Involved track: ABC123";

            uut_tee.Render();

            fakeConsoleOutput.Received().Print(Arg.Is<string>(str => str.Contains(expectedString)));
        }
        #endregion

        #region TrackLeftEvent
        [Test]
        public void TrackLeftEventCheckIfStillValid_isRaisedIsTrue_ReturnsTrue()
        {
            uut_tle._isRaised = true;

            Assert.That(() => uut_tle.CheckIfStillValid().Equals(true));
        }

        [Test]
        public void TrackLeftEventCheckIfStillValid_isRaisedIsFalse_ReturnsFalse()
        {
            uut_tle._isRaised = false;

            Assert.That(() => uut_tle.CheckIfStillValid().Equals(false));
        }

        [Test]
        public void TrackLeftEventFormatData_isCalled_ReturnsCorrectString()
        {
            string expectedString = "Track left airspace - Occurencetime: 235928121999 Involved track: ABC123";
            Assert.That(() => uut_tle.FormatData().Equals(expectedString));
        }

        [Test]
        public void TrackLeftLog_isCalled_fileOutputReceivesCorrectString()
        {
            string expectedString = "Track left airspace - Occurencetime: 235928121999 Involved track: ABC123";

            uut_tle.Log();

            fakeFileOutput.Received().Write(Arg.Is<string>(str => str.Contains(expectedString)));
        }

        [Test]
        public void TrackLeftRender_isCalled_consoleOutputReceivesCorrectString()
        {
            string expectedString = "Track left airspace - Occurencetime: 235928121999 Involved track: ABC123";

            uut_tle.Render();

            fakeConsoleOutput.Received().Print(Arg.Is<string>(str => str.Contains(expectedString)));
        }
        #endregion

        #region SeperationEvent
        [Test]
        public void SeperationEventCheckIfStillValid_isRaisedIsTrue_ReturnsTrue()
        {
            uut_se._isRaised = true;

            Assert.That(() => uut_se.CheckIfStillValid().Equals(true));
        }

        [Test]
        public void SeperationEventCheckIfStillValid_isRaisedIsFalse_ReturnsFalse()
        {
            uut_se._isRaised = false;

            Assert.That(() => uut_se.CheckIfStillValid().Equals(false));
        }

        [Test]
        public void SeperationEventFormatData_isCalled_ReturnsCorrectString()
        {
            string expectedString = "Separation event - Occurencetime: 235928121999 Involved tracks: ABC123, DEF123";
            Assert.That(() => uut_se.FormatData().Equals(expectedString));
        }

        [Test]
        public void SeperationEventLog_isCalled_fileOutputReceivesCorrectString()
        {
            string expectedString = "Separation event - Occurencetime: 235928121999 Involved tracks: ABC123, DEF123";

            uut_se.Log();

            fakeFileOutput.Received().Write(Arg.Is<string>(str => str.Contains(expectedString)));
        }

        [Test]
        public void SeperationRender_isCalled_consoleOutputReceivesCorrectString()
        {
            string expectedString = "Separation event - Occurencetime: 235928121999 Involved tracks: ABC123, DEF123";

            uut_se.Render();

            fakeConsoleOutput.Received().Print(Arg.Is<string>(str => str.Contains(expectedString)));
        }
        #endregion

    }
}
