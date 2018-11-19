using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATM.Events;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ATM.Unit.Tests
{
    [TestFixture]
    class EventList_test_unit
    {
        private EventList eventList;
        private TrackData td1;
        private TrackData td2;
        private string timeStamp;

        [SetUp]
        public void setup()
        {
            timeStamp = "235928121999";
            eventList = new EventList();
			td1=new TrackData("ABC123",10000,10000,1000,timeStamp,100,45);
            td2 = new TrackData("DEF123", 10000, 10000, 1000, timeStamp, 100, 45);
        }

        #region CheckIfSeperationEventExistsFor

        public void CheckIfSeperationEventExistsFor_NoEventExists_ReturnFalse()
        {
			Assert.That(eventList.CheckIfSeperationEventExistsFor(td1,td2).Equals(false));
        }
        #endregion

        #region checkIfTrackEnteredEventExistsFor

        #endregion

        #region checkIfTrackLeftEventExistsFor

        #endregion
    }
}
